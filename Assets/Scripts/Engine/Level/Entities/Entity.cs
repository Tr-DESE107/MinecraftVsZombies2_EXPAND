using System;
using System.Collections.Generic;
using System.Linq;
using PVZEngine.Armors;
using PVZEngine.Buffs;
using PVZEngine.Callbacks;
using PVZEngine.Damages;
using PVZEngine.Grids;
using PVZEngine.Level;
using Tools;
using UnityEngine;

namespace PVZEngine.Entities
{
    public sealed class Entity : IBuffTarget
    {
        #region 公有方法
        internal Entity(LevelEngine level, int type, long id, EntityReferenceChain spawnerReference)
        {
            Level = level;
            Type = type;
            TypeCollisionFlag = EntityCollision.GetTypeMask(type);
            ID = id;
            SpawnerReference = spawnerReference;
            Cache = new EntityCache();
        }
        public Entity(LevelEngine level, long id, EntityReferenceChain spawnerReference, EntityDefinition definition, int seed) : this(level, definition.Type, id, spawnerReference)
        {
            Definition = definition;
            ModelID = definition.GetModelID();
            RNG = new RandomGenerator(seed);
            DropRNG = new RandomGenerator(RNG.Next());
        }
        public void Init(Entity spawner)
        {
            OnInit(spawner);
            Definition.Init(this);
            Level.Triggers.RunCallbackFiltered(LevelCallbacks.POST_ENTITY_INIT, Type, this);
            PostInit?.Invoke();
        }
        public void Update()
        {
            OnUpdate();
            Definition.Update(this);
            if (EquipedArmor != null)
                EquipedArmor.Update();
            foreach (var buff in buffs.GetAllBuffs())
            {
                buff.Update();
            }
            Level.Triggers.RunCallbackFiltered(LevelCallbacks.POST_ENTITY_UPDATE, Type, this);
        }
        private void UpdateCache()
        {
            Cache.Update(this);
            UpdateHitbox();
        }

        private void UpdateHitbox()
        {
            var center = Position + GetScaledBoundsOffset() + 0.5f * GetScaledSize().y * Vector3.up;
            Vector3 size = this.GetSize();
            size.Scale(Scale);
            HitboxCache = new Bounds(center, size);
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
                Definition.PostRemove(this);
                Level.Triggers.RunCallback(LevelCallbacks.POST_ENTITY_REMOVE, this);
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

        #region 动画
        public void TriggerAnimation(string name, EntityAnimationTarget target = EntityAnimationTarget.Entity)
        {
            OnTriggerAnimation?.Invoke(name, target);
        }
        public void SetAnimationBool(string name, bool value, EntityAnimationTarget target = EntityAnimationTarget.Entity)
        {
            OnSetAnimationBool?.Invoke(name, target, value);
        }
        public void SetAnimationInt(string name, int value, EntityAnimationTarget target = EntityAnimationTarget.Entity)
        {
            OnSetAnimationInt?.Invoke(name, target, value);
        }
        public void SetAnimationFloat(string name, float value, EntityAnimationTarget target = EntityAnimationTarget.Entity)
        {
            OnSetAnimationFloat?.Invoke(name, target, value);
        }
        #endregion

        #region 伤害

        public void Die(DamageInput info = null)
        {
            info = info ?? new DamageInput(0, new DamageEffectList(), this, new EntityReferenceChain(null));
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
            return this.GetFaction() == faction;
        }
        public bool IsHostile(Entity entity, bool cache = false)
        {
            if (entity == null)
                return false;
            return IsHostile(entity.GetFaction(cache), cache);
        }

        public bool IsHostile(int faction, bool cache = false)
        {
            var selfFaction = cache ? Cache.Faction : this.GetFaction();
            return selfFaction != faction;
        }
        public bool IsActiveEntity(bool includeDead = false)
        {
            return (!IsDead || includeDead) && !Removed;
        }

        #endregion 魅惑

        #region 属性
        private object GetBaseProperty(string name, bool ignoreDefinition = false)
        {
            if (propertyDict.TryGetProperty(name, out var prop))
                return prop;

            if (ignoreDefinition)
                return null;

            if (Definition != null && Definition.TryGetProperty<object>(name, out var defProp))
                return defProp;

            var behaviour = Definition.GetBehaviour();
            if (behaviour == null)
                return null;

            return behaviour.GetProperty<object>(name);
        }
        public object GetProperty(string name, bool ignoreDefinition = false, bool ignoreBuffs = false)
        {
            object result = GetBaseProperty(name, ignoreDefinition);
            if (!ignoreBuffs)
            {
                result = buffs.CalculateProperty(name, result);
            }
            return result;
        }
        public T GetProperty<T>(string name, bool ignoreDefinition = false, bool ignoreBuffs = false)
        {
            return GetProperty(name, ignoreDefinition, ignoreBuffs).ToGeneric<T>();
        }
        public void SetProperty(string name, object value)
        {
            propertyDict.SetProperty(name, value);
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
        public bool HasBuff<T>() where T : BuffDefinition => buffs.HasBuff<T>();
        public bool HasBuff(Buff buff) => buffs.HasBuff(buff);
        public Buff[] GetBuffs<T>() where T : BuffDefinition => buffs.GetBuffs<T>();
        public Buff[] GetAllBuffs() => buffs.GetAllBuffs();
        public BuffReference GetBuffReference(Buff buff) => new BuffReferenceEntity(ID);
        private long AllocBuffID()
        {
            return currentBuffID++;
        }
        #endregion

        #region 物理

        #region 体积
        public Vector3 GetBoundsCenter()
        {
            return HitboxCache.center;
        }
        public Bounds GetBounds()
        {
            return HitboxCache;
        }
        public Vector3 GetScaledSize()
        {
            return HitboxCache.size;
        }
        public Vector3 GetScaledBoundsOffset()
        {
            Vector3 offset = BoundsOffset;
            offset.Scale(Scale);
            return offset;
        }
        #endregion

        #region 相对高度
        public float GetGroundHeight()
        {
            return Level.GetGroundY(Position.x, Position.z);
        }
        public float GetRelativeY()
        {
            return Position.y - GetGroundHeight();
        }
        public void SetRelativeY(float value)
        {
            var pos = Position;
            pos.y = value + GetGroundHeight();
            Position = pos;
        }
        #endregion

        public Vector3 GetNextPosition(float simulationSpeed = 1)
        {
            Vector3 velocity = GetNextVelocity(simulationSpeed);
            velocity.Scale(Vector3.one - this.GetVelocityDampen());
            var nextPos = Position + velocity * simulationSpeed;
            if (!GetProperty<bool>(EngineEntityProps.CAN_UNDER_GROUND))
            {
                nextPos.y = Mathf.Max(GetGroundHeight(), nextPos.y);
            }
            return nextPos;
        }


        private Vector3 GetNextVelocity(float simulationSpeed = 1)
        {
            Vector3 velocity = Velocity;

            // Friction.
            var frictionMulti = Mathf.Pow(Mathf.Max(0, 1 - this.GetFriction()), simulationSpeed);
            velocity = new Vector3(velocity.x * frictionMulti, velocity.y, velocity.z * frictionMulti);

            // Gravity.
            velocity.y -= this.GetGravity() * simulationSpeed;

            return velocity;
        }
        public void UpdatePhysics(float simulationSpeed = 1)
        {
            Vector3 nextVelocity = GetNextVelocity(simulationSpeed);
            Vector3 nextPos = GetNextPosition(simulationSpeed);

            float groundHeight = Level.GetGroundY(nextPos.x, nextPos.z);
            float relativeY = nextPos.y - groundHeight;
            bool leavingGround = relativeY > 0 || (relativeY == 0 && nextVelocity.y >= 0);

            if (!GetProperty<bool>(EngineEntityProps.CAN_UNDER_GROUND))
            {
                if (nextPos.y <= groundHeight && nextVelocity.y < 0)
                {
                    nextVelocity.y = 0;
                }
            }
            Position = nextPos;
            Velocity = nextVelocity;
            if (leavingGround)
            {
                if (IsOnGround)
                {
                    OnLeaveGround();
                    IsOnGround = false;
                }
            }
            else
            {
                if (!IsOnGround)
                {
                    OnContactGround();
                    IsOnGround = true;
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
        public void TakeGrid(LawnGrid grid)
        {
            takenGrids.Add(grid);
            grid.AddEntity(this);
        }
        public bool ReleaseGrid(LawnGrid grid)
        {
            if (takenGrids.Remove(grid))
            {
                grid.RemoveEntity(this);
                return true;
            }
            return false;
        }
        public void ClearTakenGrids()
        {
            foreach (var grid in takenGrids)
            {
                grid.RemoveEntity(this);
            }
            takenGrids.Clear();
        }
        public LawnGrid[] GetTakenGrids()
        {
            return takenGrids.ToArray();
        }
        public int GetGridIndex()
        {
            return Level.GetGridIndex(GetColumn(), GetLane());
        }
        public LawnGrid GetGrid()
        {
            return Level.GetGrid(GetColumn(), GetLane());
        }
        #endregion

        #region 碰撞
        public void Collide(Entity other)
        {
            if (!collisionList.Contains(other))
            {
                PostCollision(other, EntityCollision.STATE_ENTER);
                collisionList.Add(other);
            }
            else
            {
                PostCollision(other, EntityCollision.STATE_STAY);
            }
            collisionThisTick.Add(other);
        }
        public Entity[] GetCollisionEntities()
        {
            return collisionList.ToArray();
        }
        public void ClearCollision()
        {
            var notCollided = collisionList.Except(collisionThisTick).ToArray();
            foreach (var ent in notCollided)
            {
                if (ent != null)
                {
                    PostCollision(ent, EntityCollision.STATE_EXIT);
                }
                collisionList.Remove(ent);
            }
            collisionThisTick.Clear();
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
        #endregion
        public bool IsFacingLeft() => GetProperty<bool>(EngineEntityProps.FACE_LEFT_AT_DEFAULT) != FlipX;

        public void ChangeModel(NamespaceID id)
        {
            ModelID = id;
            OnModelChanged?.Invoke(id);
        }
        public void SetModelProperty(string name, object value)
        {
            OnSetModelProperty?.Invoke(name, value);
        }
        public void SetShaderInt(string name, int value)
        {
            OnSetShaderInt?.Invoke(name, value);
        }
        public void SetShaderFloat(string name, float value)
        {
            OnSetShaderFloat?.Invoke(name, value);
        }
        public void SetShaderColor(string name, Color value)
        {
            OnSetShaderColor?.Invoke(name, value);
        }
        public SerializableEntity Serialize()
        {
            var seri = new SerializableEntity();
            seri.id = ID;
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
            seri.scale = Scale;
            seri.collisionMaskHostile = CollisionMaskHostile;
            seri.collisionMaskFriendly = CollisionMaskFriendly;
            seri.renderRotation = RenderRotation;
            seri.renderScale = RenderScale;
            seri.boundsOffset = BoundsOffset;
            seri.poolCount = PoolCount;
            seri.timeout = Timeout;

            seri.isDead = IsDead;
            seri.health = Health;
            seri.isOnGround = IsOnGround;
            seri.currentBuffID = currentBuffID;
            seri.propertyDict = propertyDict.Serialize();
            seri.buffs = buffs.ToSerializable();
            seri.collisionThisTick = collisionThisTick.ConvertAll(e => e?.ID ?? 0);
            seri.collisionList = collisionList.ConvertAll(e => e?.ID ?? 0);
            seri.children = children.ConvertAll(e => e?.ID ?? 0);
            seri.takenGrids = takenGrids.ConvertAll(g => g.GetIndex());
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
            Scale = seri.scale;
            CollisionMaskHostile = seri.collisionMaskHostile;
            CollisionMaskFriendly = seri.collisionMaskFriendly;
            RenderRotation = seri.renderRotation;
            RenderScale = seri.renderScale;
            BoundsOffset = seri.boundsOffset;
            PoolCount = seri.poolCount;
            Timeout = seri.timeout;

            IsDead = seri.isDead;
            Health = seri.health;
            IsOnGround = seri.isOnGround;
            currentBuffID = seri.currentBuffID;
            propertyDict = PropertyDictionary.Deserialize(seri.propertyDict);
            buffs = BuffList.FromSerializable(seri.buffs, Level, this);
            collisionThisTick = seri.collisionThisTick.ConvertAll(e => Level.FindEntityByID(e));
            collisionList = seri.collisionList.ConvertAll(e => Level.FindEntityByID(e));
            children = seri.children.ConvertAll(e => Level.FindEntityByID(e));
            takenGrids = seri.takenGrids.ConvertAll(g => Level.GetGrid(g));
            UpdateCache();
        }
        public static Entity CreateDeserializingEntity(SerializableEntity seri, LevelEngine level)
        {
            return new Entity(level, seri.type, seri.id, seri.spawnerReference);
        }
        public override string ToString()
        {
            return $"{ID}({this.Definition.GetID()})";
        }
        #endregion

        #region 私有方法
        private void OnInit(Entity spawner)
        {
            Health = this.GetMaxHealth();
            UpdateCache();
        }
        private void OnUpdate()
        {
            UpdatePhysics(1);
            UpdateCache();
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
        private void PostCollision(Entity other, int state)
        {
            Definition.PostCollision(this, other, state);
            Level.Triggers.RunCallback(LevelCallbacks.POST_ENTITY_COLLISION, this, other, state);
        }

        Entity IBuffTarget.GetEntity() => this;
        IEnumerable<Buff> IBuffTarget.GetBuffs() => buffs.GetAllBuffs();
        #endregion

        #region 事件
        public event Action PostInit;

        public event Action<string, EntityAnimationTarget> OnTriggerAnimation;
        public event Action<string, EntityAnimationTarget, bool> OnSetAnimationBool;
        public event Action<string, EntityAnimationTarget, int> OnSetAnimationInt;
        public event Action<string, EntityAnimationTarget, float> OnSetAnimationFloat;

        public event Action<Armor> OnEquipArmor;
        public event Action<Armor, ArmorDamageResult> OnDestroyArmor;
        public event Action<Armor> OnRemoveArmor;

        public event Action<NamespaceID> OnModelChanged;
        public event Action<string, object> OnSetModelProperty;
        public event Action<string, int> OnSetShaderInt;
        public event Action<string, float> OnSetShaderFloat;
        public event Action<string, Color> OnSetShaderColor;
        #endregion

        #region 属性字段
        public long ID { get; }
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
        public Vector3 Scale { get; set; } = Vector3.one;
        public Bounds HitboxCache { get; private set; }
        public int CollisionMask { get; set; }
        public Vector3 RenderRotation { get; set; } = Vector3.zero;
        public Vector3 RenderScale { get; set; } = Vector3.one;
        public bool FlipX => Scale.x < 0;
        public Vector3 BoundsOffset { get; set; }
        public int CollisionMaskHostile { get; set; }
        public int CollisionMaskFriendly { get; set; }
        public int PoolCount { get; set; }
        public int Timeout { get; set; } = -1;
        public bool IsDead { get; set; }
        public float Health { get; set; }
        public int Type { get; }
        public int State { get; set; }
        public Entity Target { get; set; }
        public bool IsOnGround { get; private set; } = true;
        public EntityCache Cache { get; }
        internal int TypeCollisionFlag { get; }

        private PropertyDictionary propertyDict = new PropertyDictionary();
        private long currentBuffID = 1;
        private BuffList buffs = new BuffList();
        private List<Entity> collisionThisTick = new List<Entity>();
        private List<Entity> collisionList = new List<Entity>();
        private List<LawnGrid> takenGrids = new List<LawnGrid>();
        private List<Entity> children = new List<Entity>();
        #endregion
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