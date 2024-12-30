using System;
using System.Collections.Generic;
using System.Linq;
using PVZEngine.Armors;
using PVZEngine.Auras;
using PVZEngine.Buffs;
using PVZEngine.Callbacks;
using PVZEngine.Damages;
using PVZEngine.Grids;
using PVZEngine.Level;
using PVZEngine.Models;
using Tools;
using UnityEngine;

namespace PVZEngine.Entities
{
    public sealed class Entity : IBuffTarget, IAuraSource
    {
        #region 公有方法

        public Entity(LevelEngine level, long id, EntityReferenceChain spawnerReference, EntityDefinition definition, int seed) : this(level, definition.Type, id, spawnerReference)
        {
            Definition = definition;
            ModelID = definition.GetModelID();
            InitSeed = seed;
            RNG = new RandomGenerator(seed);
            DropRNG = new RandomGenerator(RNG.Next());

            var auraDefs = definition.GetAuras();
            for (int i = 0; i < auraDefs.Length; i++)
            {
                var auraDef = auraDefs[i];
                auras.Add(level, new AuraEffect(auraDef, i, this));
            }
        }
        private Entity(LevelEngine level, int type, long id, EntityReferenceChain spawnerReference)
        {
            Level = level;
            Type = type;
            TypeCollisionFlag = EntityCollisionHelper.GetTypeMask(type);
            ID = id;
            SpawnerReference = spawnerReference;
            MainHitbox = new EntityHitbox(this);
            buffs.OnPropertyChanged += UpdateBuffedProperty;
            buffs.OnModelInsertionAdded += OnBuffModelAddCallback;
            buffs.OnModelInsertionRemoved += OnBuffModelRemoveCallback;
            Cache = new EntityCache();
        }
        public void Init(Entity spawner)
        {
            OnInit(spawner);
            Definition.Init(this);
            auras.PostAdd();
            Level.Triggers.RunCallbackFiltered(LevelCallbacks.POST_ENTITY_INIT, Type, this);
            PostInit?.Invoke();
        }
        public void Update()
        {
            try
            {
                OnUpdate();
                Definition.Update(this);
                if (EquipedArmor != null)
                    EquipedArmor.Update();
                auras.Update();
                foreach (var buff in buffs.GetAllBuffs())
                {
                    buff.Update();
                }
                Level.Triggers.RunCallbackFiltered(LevelCallbacks.POST_ENTITY_UPDATE, Type, this);
            }
            catch (Exception ex)
            {
                Debug.LogError($"更新实体时出现错误：{ex}");
            }
        }

        private void UpdateColliders()
        {
            enabledColliders.Clear();
            MainHitbox.Update();
            foreach (var collider in colliders)
            {
                if (collider.Enabled)
                {
                    collider.Update();
                    enabledColliders.Add(collider);
                }
            }
        }
        public void SetParent(Entity parent)
        {
            var oldParent = Parent;
            if (oldParent != null)
            {
                oldParent.children.RemoveAll(r => r.ID == ID);
            }
            Parent = parent;
            if (parent != null)
            {
                parent.children.Add(this);
            }
        }
        public Entity[] GetChildren()
        {
            return children.ToArray();
        }
        public bool Exists()
        {
            return !Removed;
        }
        public void Remove()
        {
            if (!Removed)
            {
                Removed = true;
                Level.RemoveEntity(this);
                foreach (var pair in takenConveyorSeeds)
                {
                    Level.PutSeedToConveyorPool(pair.Key, pair.Value);
                }
                takenConveyorSeeds.Clear();
                Definition.PostRemove(this);
                auras.PostRemove();
                Level.Triggers.RunCallbackFiltered(LevelCallbacks.POST_ENTITY_REMOVE, Type, this);
            }
        }
        public bool IsEntityOf(NamespaceID id)
        {
            return Definition.GetID() == id;
        }
        public EntityReferenceChain GetReference()
        {
            return new EntityReferenceChain(this);
        }

        #region 伤害

        public void Die(Entity source = null, BodyDamageResult damage = null)
        {
            Die(new DamageEffectList(), new EntityReferenceChain(source), damage);
        }
        public void Die(DamageEffectList effects, Entity source = null, BodyDamageResult damage = null)
        {
            Die(effects, new EntityReferenceChain(source), damage);
        }
        public void Die(DamageEffectList effects, EntityReferenceChain source, BodyDamageResult damage = null)
        {
            Die(new DeathInfo(this, effects, source, damage));
        }
        public void Die(DeathInfo info)
        {
            info = info ?? new DeathInfo(this, new DamageEffectList(), new EntityReferenceChain(null), null);
            IsDead = true;
            Definition.PostDeath(this, info);
            Level.Triggers.RunCallbackFiltered(LevelCallbacks.POST_ENTITY_DEATH, Type, this, info);
        }
        #endregion

        #region 阵营
        public bool IsFriendly(Entity entity)
        {
            if (entity == null)
                return false;
            return IsFriendly(entity.GetFaction());
        }
        public bool IsFriendly(int faction)
        {
            return Cache.Faction == faction;
        }
        public bool IsHostile(Entity entity)
        {
            if (entity == null)
                return false;
            return IsHostile(entity.GetFaction());
        }

        public bool IsHostile(int faction)
        {
            return Cache.Faction != faction;
        }
        public bool IsActiveEntity(bool includeDead = false)
        {
            return (!IsDead || includeDead) && !Removed;
        }

        #endregion 魅惑

        #region 属性
        public object GetProperty(string name, bool ignoreDefinition = false, bool ignoreBuffs = false)
        {
            if (!ignoreBuffs)
            {
                if (buffedProperties.TryGetProperty(name, out var value))
                    return value;
            }
            if (propertyDict.TryGetProperty(name, out var prop))
                return prop;

            if (ignoreDefinition)
                return null;

            if (Definition != null && Definition.TryGetProperty<object>(name, out var defProp))
                return defProp;

            var behaviours = Definition.GetBehaviours();
            foreach (var behaviour in behaviours)
            {
                if (behaviour.TryGetProperty<object>(name, out var behProp))
                    return behProp;
            }
            return null;
        }
        public T GetProperty<T>(string name, bool ignoreDefinition = false, bool ignoreBuffs = false)
        {
            return GetProperty(name, ignoreDefinition, ignoreBuffs).ToGeneric<T>();
        }
        public void SetProperty(string name, object value)
        {
            propertyDict.SetProperty(name, value);
            UpdateBuffedProperty(name);
        }
        private void UpdateAllBuffedProperties()
        {
            var propertyNames = buffs.GetModifierPropertyNames();
            foreach (var name in propertyNames)
            {
                UpdateBuffedProperty(name);
            }
        }
        private void UpdateBuffedProperty(string name)
        {
            var baseValue = GetProperty(name, ignoreBuffs: true);
            var value = buffs.CalculateProperty(name, baseValue);
            buffedProperties.SetProperty(name, value);
            Cache.UpdateProperty(this, name, value);
        }
        #endregion

        #region 增益
        public Buff CreateBuff<T>() where T : BuffDefinition
        {
            return Level.CreateBuff<T>(AllocBuffID());
        }
        public Buff CreateBuff(BuffDefinition buffDefinition)
        {
            return Level.CreateBuff(buffDefinition, AllocBuffID());
        }
        public Buff CreateBuff(NamespaceID id)
        {
            return Level.CreateBuff(id, AllocBuffID());
        }
        public bool AddBuff(Buff buff)
        {
            if (buffs.AddBuff(buff))
            {
                buff.AddToTarget(this);
                return true;
            }
            return false;
        }
        public Buff AddBuff<T>() where T : BuffDefinition
        {
            var buff = CreateBuff<T>();
            AddBuff(buff);
            return buff;
        }
        public bool RemoveBuff(Buff buff) => buffs.RemoveBuff(buff);
        public int RemoveBuffs(IEnumerable<Buff> buffs) => this.buffs.RemoveBuffs(buffs);
        public int RemoveBuffs<T>() where T : BuffDefinition => RemoveBuffs(GetBuffs<T>());
        public bool HasBuff<T>() where T : BuffDefinition => buffs.HasBuff<T>();
        public bool HasBuff(Buff buff) => buffs.HasBuff(buff);
        public Buff[] GetBuffs<T>() where T : BuffDefinition => buffs.GetBuffs<T>();
        public Buff[] GetAllBuffs() => buffs.GetAllBuffs();
        public BuffReference GetBuffReference(Buff buff) => new BuffReferenceEntity(ID, buff.ID);
        private long AllocBuffID()
        {
            return currentBuffID++;
        }
        #endregion

        #region 物理

        #region 体积
        public Vector3 GetCenter()
        {
            return MainHitbox.GetBoundsCenter();
        }
        public Bounds GetBounds()
        {
            return MainHitbox.GetBounds();
        }
        public Vector3 GetScaledSize()
        {
            return MainHitbox.GetBoundsSize();
        }
        #endregion

        #region 相对高度
        public float GetGroundY()
        {
            return Level.GetGroundY(Position.x, Position.z);
        }
        public float GetRelativeY()
        {
            return Position.y - GetGroundY();
        }
        public void SetRelativeY(float value)
        {
            var pos = Position;
            pos.y = value + GetGroundY();
            Position = pos;
        }
        #endregion

        private Vector3 GetNextPosition(float simulationSpeed = 1)
        {
            Vector3 velocity = GetNextVelocity(simulationSpeed);
            velocity.Scale(Vector3.one - Cache.VelocityDampen);
            var nextPos = Position + velocity * simulationSpeed;
            return nextPos;
        }


        private Vector3 GetNextVelocity(float simulationSpeed = 1)
        {
            Vector3 velocity = Velocity;

            // Friction.
            var frictionMulti = Mathf.Pow(Mathf.Max(0, 1 - Cache.Friction), simulationSpeed);
            velocity = new Vector3(velocity.x * frictionMulti, velocity.y, velocity.z * frictionMulti);

            // Gravity.
            velocity.y -= Cache.Gravity * simulationSpeed;

            return velocity;
        }

        public void UpdatePhysics(float simulationSpeed = 1)
        {
            Vector3 nextVelocity = GetNextVelocity(simulationSpeed);
            Vector3 nextPos = GetNextPosition(simulationSpeed);

            // 地面限制。
            float groundY = Level.GetGroundY(nextPos.x, nextPos.z);
            var groundLimit = groundY + Cache.GroundLimitOffset;
            var contactingGround = nextPos.y <= groundY;
            if (nextPos.y <= groundLimit)
            {
                nextPos.y = groundLimit;
                nextVelocity.y = Mathf.Max(nextVelocity.y, 0);
            }

            Position = nextPos;
            Velocity = nextVelocity;

            if (contactingGround)
            {
                if (!IsOnGround)
                {
                    OnContactGround();
                    IsOnGround = true;
                }
            }
            else
            {
                if (IsOnGround)
                {
                    OnLeaveGround();
                    IsOnGround = false;
                }
            }
        }
        #endregion

        #region 网格
        public int GetColumn()
        {
            return Level.GetColumn(Position.x);
        }
        public int GetLane()
        {
            return Level.GetLane(Position.z);
        }
        public NamespaceID[] GetTakingGridLayers(LawnGrid grid)
        {
            var info = GetTakenGridInfo(grid);
            if (info == null)
                return null;
            return info.takenLayers.ToArray();
        }
        public bool IsTakingGridLayer(LawnGrid grid, NamespaceID layer)
        {
            var info = GetTakenGridInfo(grid);
            if (info == null)
                return false;
            return info.takenLayers.Contains(layer);
        }
        public void TakeGrid(LawnGrid grid, NamespaceID layer)
        {
            var info = GetOrCreateTakenGridInfo(grid);
            info.takenLayers.Add(layer);
            grid.AddLayerEntity(layer, this);
        }
        public bool ReleaseGrid(LawnGrid grid, NamespaceID layer)
        {
            var info = GetTakenGridInfo(grid);
            if (info == null)
                return false;
            if (info.takenLayers.Remove(layer))
            {
                grid.RemoveLayerEntity(layer);
                return true;
            }
            return false;
        }
        public void ClearTakenGrids()
        {
            foreach (var info in takenGrids)
            {
                foreach (var layer in info.takenLayers)
                {
                    info.grid.RemoveLayerEntity(layer);
                }
            }
            takenGrids.Clear();
        }
        public LawnGrid[] GetTakenGrids()
        {
            return takenGrids.Select(i => i.grid).ToArray();
        }
        public int GetGridIndex()
        {
            return Level.GetGridIndex(GetColumn(), GetLane());
        }
        public LawnGrid GetGrid()
        {
            return Level.GetGrid(GetColumn(), GetLane());
        }
        private TakenGridInfo GetOrCreateTakenGridInfo(LawnGrid grid)
        {
            var info = GetTakenGridInfo(grid);
            if (info == null)
            {
                info = new TakenGridInfo(grid);
                takenGrids.Add(info);
            }
            return info;
        }
        private TakenGridInfo GetTakenGridInfo(LawnGrid grid)
        {
            return takenGrids.FirstOrDefault(g => g.grid == grid);
        }
        #endregion

        #region 碰撞
        public void AddCollider(EntityCollider collider)
        {
            if (GetCollider(collider.Name) != null)
                throw new ArgumentException($"Attempting to add a collider with name \"{collider.Name}\" to an entity while it already has a collider with the same name.");
            colliders.Add(collider);
            collider.PreCollision += PreCollisionCallback;
            collider.PostCollision += PostCollisionCallback;
        }
        public bool RemoveCollider(EntityCollider collider)
        {
            if (colliders.Remove(collider))
            {
                collider.PreCollision -= PreCollisionCallback;
                collider.PostCollision -= PostCollisionCallback;
                return true;
            }
            return false;
        }
        public EntityCollider GetCollider(string name)
        {
            return colliders.FirstOrDefault(h => h.Name == name);
        }
        public EntityCollider[] GetAllColliders()
        {
            return colliders.ToArray();
        }
        public EntityCollider[] GetEnabledColliders()
        {
            return enabledColliders.ToArray();
        }
        public bool CheckCollisionWith(Entity other)
        {
            foreach (var group1 in GetEnabledColliders())
            {
                foreach (var group2 in other.GetEnabledColliders())
                {
                    if (group1.Intersects(group2))
                        return true;
                }
            }
            return false;
        }
        public int CheckContacts(Entity other, EntityCollision[] buffer)
        {
            int index = 0;
            foreach (var collider1 in enabledColliders)
            {
                foreach (var collider2 in other.enabledColliders)
                {
                    if (collider1.Intersects(collider2))
                    {
                        var collision = new EntityCollision(collider1, collider2);
                        buffer[index] = collision;
                        index++;
                        if (index >= buffer.Length)
                            return index;
                    }
                }
            }
            return index;
        }
        public IEnumerable<EntityCollision> GetCurrentCollisions()
        {
            foreach (var unit in colliders)
            {
                foreach (var reference in unit.GetCollisions())
                {
                    yield return reference;
                }
            }
        }
        public void ExitCollision(LevelEngine level)
        {
            foreach (var unit in colliders)
            {
                unit.ExitCollision(level);
            }
        }
        #endregion

        #region 护甲
        public void EquipArmor<T>() where T : ArmorDefinition
        {
            EquipArmor(new Armor(this, Level.Content.GetArmorDefinition<T>()));
        }
        public void EquipArmor(ArmorDefinition definition)
        {
            if (definition == null)
                return;
            EquipArmor(new Armor(this, definition));
        }
        public void EquipArmor(Armor armor)
        {
            if (armor == null)
                return;
            if (EquipedArmor != null)
                EquipedArmor.Destroy(null);
            EquipedArmor = armor;

            Definition.PostEquipArmor(this, armor);
            Level.Triggers.RunCallback(LevelCallbacks.POST_EQUIP_ARMOR, this, armor);
            OnEquipArmor?.Invoke(armor);
        }
        public void DestroyArmor(Armor armor, ArmorDamageResult result)
        {
            Definition.PostDestroyArmor(this, armor, result);
            Level.Triggers.RunCallback(LevelCallbacks.POST_DESTROY_ARMOR, this, armor, result);
            OnDestroyArmor?.Invoke(armor, result);
        }
        public void RemoveArmor()
        {
            var armor = EquipedArmor;
            if (armor == null)
                return;
            EquipedArmor = null;
            Definition.PostRemoveArmor(this, armor);
            Level.Triggers.RunCallback(LevelCallbacks.POST_REMOVE_ARMOR, this, armor);
            OnRemoveArmor?.Invoke(armor);
        }
        public Armor GetShield()
        {
            return null;
        }
        #endregion
        public bool IsFacingLeft() => this.FaceLeftAtDefault() != FlipX;

        #region 模型
        public void SetModelInterface(IModelInterface model, IModelInterface armorModel)
        {
            modelInterface = model;
            armorModelInterface = armorModel;
        }
        public void ChangeModel(NamespaceID id)
        {
            ModelID = id;
            OnChangeModel?.Invoke(id);
        }
        public void SetModelProperty(string name, object value)
        {
            modelInterface.SetModelProperty(name, value);
        }
        public void TriggerModel(string name)
        {
            modelInterface.TriggerModel(name);
        }
        public void SetShaderInt(string name, int value)
        {
            modelInterface.SetShaderInt(name, value);
        }
        public void SetShaderFloat(string name, float value)
        {
            modelInterface.SetShaderFloat(name, value);
        }
        public void SetShaderColor(string name, Color value)
        {
            modelInterface.SetShaderColor(name, value);
        }
        public IModelInterface CreateChildModel(string anchorName, NamespaceID key, NamespaceID modelID)
        {
            return modelInterface.CreateChildModel(anchorName, key, modelID);
        }
        public bool RemoveChildModel(NamespaceID key)
        {
            return modelInterface.RemoveChildModel(key);
        }
        public IModelInterface GetChildModel(NamespaceID key)
        {
            return modelInterface.GetChildModel(key);
        }
        public void TriggerAnimation(string name, EntityAnimationTarget target = EntityAnimationTarget.Entity)
        {
            GetModelTarget(target).TriggerAnimation(name);
        }
        public void SetAnimationBool(string name, bool value, EntityAnimationTarget target = EntityAnimationTarget.Entity)
        {
            GetModelTarget(target).SetAnimationBool(name, value);
        }
        public void SetAnimationInt(string name, int value, EntityAnimationTarget target = EntityAnimationTarget.Entity)
        {
            GetModelTarget(target).SetAnimationInt(name, value);
        }
        public void SetAnimationFloat(string name, float value, EntityAnimationTarget target = EntityAnimationTarget.Entity)
        {
            GetModelTarget(target).SetAnimationFloat(name, value);
        }
        private IModelInterface GetModelTarget(EntityAnimationTarget target)
        {
            return target == EntityAnimationTarget.Armor ? armorModelInterface : modelInterface;
        }
        #endregion

        #region 传送带
        public void AddTakenConveyorSeed(NamespaceID id)
        {
            if (takenConveyorSeeds.ContainsKey(id))
            {
                takenConveyorSeeds[id]++;
            }
            else
            {
                takenConveyorSeeds[id] = 1;
            }
        }
        public bool RemoveTakenConveyorSeed(NamespaceID id)
        {
            if (!takenConveyorSeeds.ContainsKey(id))
            {
                return false;
            }
            takenConveyorSeeds[id]--;
            if (takenConveyorSeeds[id] <= 0)
            {
                takenConveyorSeeds.Remove(id);
            }
            return true;
        }
        #endregion

        #region 序列化
        public SerializableEntity Serialize()
        {
            var seri = new SerializableEntity();
            seri.id = ID;
            seri.initSeed = InitSeed;
            seri.spawnerReference = SpawnerReference;
            seri.type = Type;
            seri.state = State;
            seri.rng = RNG.ToSerializable();
            seri.dropRng = DropRNG.ToSerializable();
            seri.target = Target?.ID ?? 0;

            seri.definitionID = Definition.GetID();
            seri.modelID = ModelID;
            seri.parent = Parent?.ID ?? 0;
            seri.EquipedArmor = EquipedArmor?.Serialize();
            seri.position = Position;
            seri.velocity = Velocity;
            seri.collisionMaskHostile = CollisionMaskHostile;
            seri.collisionMaskFriendly = CollisionMaskFriendly;
            seri.renderRotation = RenderRotation;
            seri.takenConveyorSeeds = takenConveyorSeeds.ToDictionary(p => p.ToString(), p => p.Value);
            seri.timeout = Timeout;
            seri.colliders = colliders.ConvertAll(g => g.ToSerializable()).ToArray();

            seri.isDead = IsDead;
            seri.health = Health;
            seri.isOnGround = IsOnGround;
            seri.currentBuffID = currentBuffID;
            seri.propertyDict = propertyDict.Serialize();
            seri.buffs = buffs.ToSerializable();
            seri.children = children.ConvertAll(e => e?.ID ?? 0);
            seri.takenGrids = takenGrids.ConvertAll(i => new SerializableEntity.TakenGridInfo() { grid = i.grid.GetIndex(), layers = i.takenLayers.ToArray() });

            seri.auras = auras.GetAll().Select(a => a.ToSerializable()).ToArray();
            return seri;
        }
        public static Entity Deserialize(SerializableEntity seri, LevelEngine level)
        {
            Entity entity = CreateDeserializingEntity(seri, level);
            entity.ApplyDeserialize(seri);
            return entity;
        }
        public void ApplyDeserialize(SerializableEntity seri)
        {
            InitSeed = seri.initSeed;
            RNG = RandomGenerator.FromSerializable(seri.rng);
            DropRNG = RandomGenerator.FromSerializable(seri.dropRng);
            State = seri.state;
            Target = Level.FindEntityByID(seri.target);

            Definition = Level.Content.GetEntityDefinition(seri.definitionID);
            ModelID = seri.modelID;
            Parent = Level.FindEntityByID(seri.parent);
            EquipedArmor = seri.EquipedArmor != null ? Armor.Deserialize(seri.EquipedArmor, this) : null;
            Position = seri.position;
            Velocity = seri.velocity;
            CollisionMaskHostile = seri.collisionMaskHostile;
            CollisionMaskFriendly = seri.collisionMaskFriendly;
            RenderRotation = seri.renderRotation;
            takenConveyorSeeds = seri.takenConveyorSeeds.ToDictionary(p => NamespaceID.ParseStrict(p.Key), p => p.Value);
            Timeout = seri.timeout;

            IsDead = seri.isDead;
            Health = seri.health;
            IsOnGround = seri.isOnGround;
            currentBuffID = seri.currentBuffID;
            propertyDict = PropertyDictionary.Deserialize(seri.propertyDict);

            buffs = BuffList.FromSerializable(seri.buffs, Level, this);
            buffs.OnPropertyChanged += UpdateBuffedProperty;
            buffs.OnModelInsertionAdded += OnBuffModelAddCallback;
            buffs.OnModelInsertionRemoved += OnBuffModelRemoveCallback;

            children = seri.children.ConvertAll(e => Level.FindEntityByID(e));
            takenGrids = seri.takenGrids.Select(i => new TakenGridInfo(Level.GetGrid(i.grid)) { takenLayers = i.layers.ToHashSet() }).ToList();
            for (int i = 0; i < colliders.Count; i++)
            {
                var collider = colliders[i];
                var seriCollider = seri.colliders[i];
                collider.LoadCollisions(Level, seriCollider);
            }
            auras.LoadFromSerializable(Level, seri.auras);
            UpdateAllBuffedProperties();
            Cache.UpdateAll(this);
            UpdateColliders();
        }
        public static Entity CreateDeserializingEntity(SerializableEntity seri, LevelEngine level)
        {
            var entity = new Entity(level, seri.type, seri.id, seri.spawnerReference);

            foreach (var collider in seri.colliders.Select(s => EntityCollider.FromSerializable(s, entity)))
            {
                entity.AddCollider(collider);
            }
            return entity;
        }
        #endregion
        public override string ToString()
        {
            return $"{ID}({this.Definition.GetID()})";
        }
        #endregion

        #region 私有方法
        private void OnInit(Entity spawner)
        {
            Health = this.GetMaxHealth();
            var collider = new EntityCollider(this, EntityCollisionHelper.NAME_MAIN, new EntityHitbox(this));
            AddCollider(collider);
            UpdateAllBuffedProperties();
            Cache.UpdateAll(this);
            UpdateColliders();
        }
        private void OnUpdate()
        {
            UpdatePhysics(1);
            UpdateColliders();
            Health = Mathf.Min(Health, this.GetMaxHealth());
        }
        private void OnContactGround()
        {
            var velocity = Velocity;
            Definition.PostContactGround(this, velocity);
            Level.Triggers.RunCallback(LevelCallbacks.POST_ENTITY_CONTACT_GROUND, this, velocity);
        }
        private void OnLeaveGround()
        {
            Definition.PostLeaveGround(this);
            Level.Triggers.RunCallback(LevelCallbacks.POST_ENTITY_LEAVE_GROUND, this);
        }
        private bool PreCollisionCallback(EntityCollision collision)
        {
            bool canCollide = Definition.PreCollision(collision);
            if (!canCollide)
                return false;
            foreach (var trigger in Level.Triggers.GetTriggers(LevelCallbacks.PRE_ENTITY_COLLISION))
            {
                var result = trigger.Invoke(collision);
                if (result is bool boolValue && !boolValue)
                    return false;
            }
            return true;
        }
        private void PostCollisionCallback(EntityCollision collision, int state)
        {
            Definition.PostCollision(collision, state);
            Level.Triggers.RunCallback(LevelCallbacks.POST_ENTITY_COLLISION, collision, state);
        }
        private void OnBuffModelAddCallback(string anchorName, NamespaceID key, NamespaceID modelID)
        {
            CreateChildModel(anchorName, key, modelID);
        }
        private void OnBuffModelRemoveCallback(NamespaceID key)
        {
            RemoveChildModel(key);
        }

        Entity IBuffTarget.GetEntity() => this;
        IEnumerable<Buff> IBuffTarget.GetBuffs() => buffs.GetAllBuffs();
        Entity IAuraSource.GetEntity() => this;
        LevelEngine IAuraSource.GetLevel() => Level;
        #endregion

        #region 事件
        public event Action PostInit;
        public event Action<NamespaceID> OnChangeModel;
        public event Action<Armor> OnEquipArmor;
        public event Action<Armor, ArmorDamageResult> OnDestroyArmor;
        public event Action<Armor> OnRemoveArmor;
        #endregion

        #region 属性字段
        public long ID { get; }
        public int InitSeed { get; private set; }
        public RandomGenerator RNG { get; private set; }
        public RandomGenerator DropRNG { get; private set; }
        public bool Removed { get; private set; }
        public EntityDefinition Definition { get; private set; }
        public NamespaceID ModelID { get; private set; }
        public EntityReferenceChain SpawnerReference { get; private set; }
        public Entity Parent { get; private set; }
        public LevelEngine Level { get; private set; }
        public Armor EquipedArmor { get; private set; }
        public Vector3 Position { get; set; }
        public Vector3 Velocity { get; set; }
        public Vector3 RenderRotation { get; set; } = Vector3.zero;
        public bool FlipX => this.GetScale().x < 0;
        #region Collision
        public int CollisionMaskHostile { get; set; }
        public int CollisionMaskFriendly { get; set; }
        public Hitbox MainHitbox { get; private set; }
        private List<EntityCollider> colliders = new List<EntityCollider>();
        private List<EntityCollider> enabledColliders = new List<EntityCollider>();
        private IModelInterface modelInterface;
        private IModelInterface armorModelInterface;
        #endregion
        public int Timeout { get; set; } = -1;
        public bool IsDead { get; set; }
        public float Health { get; set; }
        public int Type { get; }
        public int State { get; set; }
        public Entity Target { get; set; }
        public bool IsOnGround { get; private set; } = true;
        internal int TypeCollisionFlag { get; }
        internal EntityCache Cache { get; }

        private PropertyDictionary propertyDict = new PropertyDictionary();
        private PropertyDictionary buffedProperties = new PropertyDictionary();
        private long currentBuffID = 1;
        private BuffList buffs = new BuffList();
        private AuraEffectList auras = new AuraEffectList();
        private List<TakenGridInfo> takenGrids = new List<TakenGridInfo>();
        private List<Entity> children = new List<Entity>();
        private Dictionary<NamespaceID, int> takenConveyorSeeds = new Dictionary<NamespaceID, int>();
        #endregion

        private class TakenGridInfo
        {
            public TakenGridInfo(LawnGrid grid)
            {
                this.grid = grid;
            }
            public LawnGrid grid;
            public HashSet<NamespaceID> takenLayers = new HashSet<NamespaceID>();
        }
    }
}

namespace PVZEngine.Level
{
    public enum EntityAnimationTarget
    {
        Entity,
        Armor
    }
}