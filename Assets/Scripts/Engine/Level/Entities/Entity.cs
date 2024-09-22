using System;
using System.Collections.Generic;
using System.Linq;
using PVZEngine.Definitions;
using PVZEngine.Serialization;
using Tools;
using UnityEngine;

namespace PVZEngine.Level
{
    public sealed class Entity : IBuffTarget
    {
        #region 公有方法
        internal Entity(LevelEngine level, int type, long id, EntityReferenceChain spawnerReference)
        {
            Level = level;
            Type = type;
            ID = id;
            SpawnerReference = spawnerReference;
        }
        public Entity(LevelEngine level, long id, EntityReferenceChain spawnerReference, EntityDefinition definition, int seed) : this(level, definition.Type, id, spawnerReference)
        {
            Definition = definition;
            ModelID = definition.GetModelID();
            RNG = new RandomGenerator(seed);
            DropRNG = new RandomGenerator(RNG.Next());
            SetTint(Color.white);
        }
        public void Init(Entity spawner)
        {
            OnInit(spawner);
            Definition.Init(this);
            LevelCallbacks.PostEntityInit.RunFiltered(Type, this);
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
            LevelCallbacks.PostEntityUpdate.RunFiltered(Type, this);
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
                LevelCallbacks.PostEntityRemove.Run(this);
            }
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

        #region 原版属性
        public bool IsInvincible()
        {
            return GetProperty<bool>(EntityProperties.INVINCIBLE);
        }
        public bool IsInvisible()
        {
            return GetProperty<bool>(EntityProperties.INVISIBLE);
        }
        public bool IsEthereal()
        {
            return GetProperty<bool>(EntityProperties.ETHEREAL);
        }
        public float GetGravity()
        {
            return GetProperty<float>(EntityProperties.GRAVITY);
        }
        public float GetFallDamage()
        {
            return GetProperty<float>(EntityProperties.FALL_DAMAGE);
        }
        public void SetFallDamage(float value)
        {
            SetProperty(EntityProperties.FALL_DAMAGE, value);
        }
        public float GetDamage(bool ignoreBuffs = false)
        {
            return GetProperty<float>(EntityProperties.DAMAGE, ignoreBuffs: ignoreBuffs);
        }
        public void SetDamage(float value)
        {
            SetProperty(EntityProperties.DAMAGE, value);
        }
        public float GetAttackSpeed()
        {
            return GetProperty<float>(EntityProperties.ATTACK_SPEED);
        }
        public float GetProduceSpeed()
        {
            return GetProperty<float>(EntityProperties.PRODUCE_SPEED);
        }
        public float GetFriction()
        {
            return GetProperty<float>(EntityProperties.FRICTION);
        }
        public void SetFriction(float value)
        {
            SetProperty(EntityProperties.FRICTION, value);
        }
        public Color GetTint(bool ignoreBuffs = false)
        {
            return GetProperty<Color>(EntityProperties.TINT, ignoreBuffs: ignoreBuffs);
        }
        public void SetTint(Color value)
        {
            SetProperty(EntityProperties.TINT, value);
        }
        public Color GetColorOffset(bool ignoreBuffs = false)
        {
            return GetProperty<Color>(EntityProperties.COLOR_OFFSET, ignoreBuffs: ignoreBuffs);
        }
        public void SetColorOffset(Color value)
        {
            SetProperty(EntityProperties.COLOR_OFFSET, value);
        }
        public int GetFaction(bool ignoreBuffs = false)
        {
            return GetProperty<int>(EntityProperties.FACTION, ignoreBuffs: ignoreBuffs);
        }
        public void SetFaction(int value)
        {
            SetProperty(EntityProperties.FACTION, value);
        }
        public Vector3 GetSize(bool ignoreBuffs = false)
        {
            return GetProperty<Vector3>(EntityProperties.SIZE, ignoreBuffs: ignoreBuffs);
        }
        public void SetSize(Vector3 value)
        {
            SetProperty(EntityProperties.SIZE, value);
        }
        public float GetMaxHealth(bool ignoreBuffs = false)
        {
            return GetProperty<float>(EntityProperties.MAX_HEALTH, ignoreBuffs: ignoreBuffs);
        }
        public void SetMaxHealth(float value)
        {
            SetProperty(EntityProperties.MAX_HEALTH, value);
        }
        public NamespaceID GetShellID(bool ignoreBuffs = false)
        {
            return GetProperty<NamespaceID>(EntityProperties.SHELL, ignoreBuffs: ignoreBuffs);
        }
        public void SetShellID(NamespaceID value)
        {
            SetProperty(EntityProperties.SHELL, value);
        }
        #endregion

        #region 伤害
        private static DamageResult ArmoredTakeDamage(DamageInfo info, out DamageResult armorResult)
        {
            var entity = info.Entity;
            armorResult = Armor.TakeDamage(info);
            if (info.Effects.HasEffect(DamageFlags.DAMAGE_BOTH_ARMOR_AND_BODY))
            {
                return BodyTakeDamage(info);
            }
            else if (info.Effects.HasEffect(DamageFlags.DAMAGE_BODY_AFTER_ARMOR_BROKEN) && !Armor.Exists(entity.EquipedArmor))
            {
                float overkillDamage = armorResult != null ? info.Amount - armorResult.UsedDamage : info.Amount;
                if (overkillDamage > 0)
                {
                    var overkillInfo = new DamageInfo(overkillDamage, info.Effects, entity, info.Source);
                    return BodyTakeDamage(overkillInfo);
                }
            }
            return null;
        }
        private static DamageResult BodyTakeDamage(DamageInfo info)
        {
            var entity = info.Entity;
            var shellRef = entity.GetProperty<NamespaceID>(EntityProperties.SHELL);
            var shell = entity.Level.ContentProvider.GetShellDefinition(shellRef);
            if (shell != null)
            {
                shell.EvaluateDamage(info);
            }


            // Calculate used damage.
            float usedDamage = info.GetUsedDamage();

            // Apply Damage.
            float hpBefore = entity.Health;
            entity.Health -= info.Amount;
            if (entity.Health <= 0)
            {
                entity.Die(info);
            }

            return new DamageResult()
            {
                OriginalDamage = info.OriginalDamage,
                Amount = info.Amount,
                UsedDamage = info.GetUsedDamage(),
                Entity = entity,
                Effects = info.Effects,
                Source = info.Source,
                ShellDefinition = shell,
                Fatal = hpBefore > 0 && entity.Health <= 0
            };
        }
        public DamageResult TakeDamage(float amount, DamageEffectList effects, EntityReferenceChain source)
        {
            return TakeDamage(amount, effects, source, out _);
        }
        public DamageResult TakeDamage(float amount, DamageEffectList effects, EntityReferenceChain source, out DamageResult armorResult)
        {
            return TakeDamage(new DamageInfo(amount, effects, this, source), out armorResult);
        }
        public static DamageResult TakeDamage(DamageInfo info)
        {
            return TakeDamage(info, out _);
        }
        public static DamageResult TakeDamage(DamageInfo info, out DamageResult armorResult)
        {
            armorResult = null;
            if (info.Entity.IsInvincible() || info.Entity.IsDead)
                return null;
            if (!info.Entity.PreTakeDamage(info))
                return null;
            if (info.Amount <= 0)
                return null;
            DamageResult bodyResult;
            if (Armor.Exists(info.Entity.EquipedArmor) && !info.Effects.HasEffect(DamageFlags.IGNORE_ARMOR))
            {
                bodyResult = ArmoredTakeDamage(info, out armorResult);
            }
            else
            {
                bodyResult = BodyTakeDamage(info);
            }
            info.Entity.PostTakeDamage(bodyResult, armorResult);
            return bodyResult;
        }


        public void Die(DamageInfo info = null)
        {
            info = info ?? new DamageInfo(0, new DamageEffectList(), this, new EntityReferenceChain(null));
            IsDead = true;
            Definition.PostDeath(this, info);
            LevelCallbacks.PostEntityDeath.Run(this, info);
        }
        #endregion

        #region 阵营
        public bool IsEnemy(Entity entity)
        {
            if (entity == null)
                return false;
            return IsEnemy(entity.GetFaction());
        }

        public bool IsEnemy(int faction)
        {
            return GetFaction() != faction;
        }
        public bool IsActiveEntity(bool includeDead = false)
        {
            return (!IsDead || includeDead) && !Removed;
        }

        #endregion 魅惑

        #region 属性
        public object GetProperty(string name, bool ignoreDefinition = false, bool ignoreBuffs = false)
        {
            object result = null;
            if (propertyDict.TryGetProperty(name, out var prop))
                result = prop;
            else if (!ignoreDefinition)
                result = Definition.GetProperty<object>(name);

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
        public bool AddBuff(Buff buff)
        {
            if (buffs.AddBuff(buff))
            {
                buff.AddToTarget(this);
                return true;
            }
            return false;
        }
        public void AddBuff<T>() where T : BuffDefinition
        {
            AddBuff(Level.CreateBuff<T>());
        }
        public bool RemoveBuff(Buff buff) => buffs.RemoveBuff(buff);
        public int RemoveBuffs(IEnumerable<Buff> buffs) => this.buffs.RemoveBuffs(buffs);
        public bool HasBuff<T>() where T : BuffDefinition => buffs.HasBuff<T>();
        public bool HasBuff(Buff buff) => buffs.HasBuff(buff);
        public Buff[] GetBuffs<T>() where T : BuffDefinition => buffs.GetBuffs<T>();
        public Buff[] GetAllBuffs() => buffs.GetAllBuffs();
        #endregion

        #region 物理

        #region 体积
        public Vector3 GetBoundsCenter()
        {
            return Pos + GetScaledBoundsOffset() + 0.5f * GetScaledSize().y * Vector3.up;
        }
        public Bounds GetBounds()
        {
            return new Bounds(GetBoundsCenter(), GetSize());
        }
        public Bounds GetCachedBounds()
        {
            return new Bounds(GetBoundsCenter(), GetSize());
        }
        public Vector3 GetScaledSize()
        {
            Vector3 size = GetSize();
            size.Scale(Scale);
            return size;
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
            return Level.GetGroundY(Pos.x, Pos.z);
        }
        public float GetRelativeY()
        {
            return Pos.y - GetGroundHeight();
        }
        public void SetRelativeY(float value)
        {
            var pos = Pos;
            pos.y = value + GetGroundHeight();
            Pos = pos;
        }
        #endregion

        public Vector3 GetNextPosition(float simulationSpeed = 1)
        {
            Vector3 velocity = GetNextVelocity(simulationSpeed);
            var nextPos = Pos + velocity * simulationSpeed;
            if (!CanUnderGround)
            {
                nextPos.y = Mathf.Max(GetGroundHeight(), nextPos.y);
            }
            return nextPos;
        }


        private Vector3 GetNextVelocity(float simulationSpeed = 1)
        {
            Vector3 velocity = Velocity;

            // Friction.
            float magnitude = velocity.magnitude;
            Vector3 normalized = velocity.normalized;
            velocity = normalized * Math.Max(0, magnitude * (1 - simulationSpeed * GetFriction()));

            // Gravity.
            velocity.y -= GetGravity() * simulationSpeed;

            return velocity;
        }
        public void UpdatePhysics(float simulationSpeed = 1)
        {
            Vector3 nextVelocity = GetNextVelocity(simulationSpeed);
            Vector3 nextPos = GetNextPosition(simulationSpeed);
            Pos = nextPos;
            Velocity = nextVelocity;

            float groundHeight = GetGroundHeight();
            float relativeY = nextPos.y - groundHeight;
            bool leavingGround = relativeY > 0 || (relativeY == 0 && nextVelocity.y >= 0);
            if (leavingGround)
            {
                if (isOnGround)
                {
                    OnLeaveGround();
                    isOnGround = false;
                }
            }
            else
            {
                if (!isOnGround)
                {
                    OnContactGround();
                    isOnGround = true;
                }
            }
        }
        #endregion

        #region 网格
        public int GetColumn()
        {
            return Level.GetColumn(Pos.x);
        }
        public int GetLane()
        {
            return Level.GetLane(Pos.z);
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
            if (collisionList.Contains(other))
            {
                PostCollision(other, EntityCollision.STATE_ENTER);
            }
            else
            {
                PostCollision(other, EntityCollision.STATE_STAY);
                collisionList.Add(other);
            }
            collisionThisTick.Add(other);
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
        }
        #endregion

        #region 护甲
        public void EquipArmor<T>() where T : ArmorDefinition
        {
            EquipArmor(new Armor(this, Level.ContentProvider.GetArmorDefinition<T>()));
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
            LevelCallbacks.PostEquipArmor.Run(this, armor);
            OnEquipArmor?.Invoke(armor);
        }
        public void DestroyArmor(Armor armor, DamageResult result)
        {
            Definition.PostDestroyArmor(this, armor, result);
            LevelCallbacks.PostDestroyArmor.Run(this, armor, result);
            OnDestroyArmor?.Invoke(armor, result);
        }
        public void RemoveArmor()
        {
            var armor = EquipedArmor;
            if (armor == null)
                return;
            EquipedArmor = null;
            Definition.PostRemoveArmor(this, armor);
            LevelCallbacks.PostRemoveArmor.Run(this, armor);
            OnRemoveArmor?.Invoke(armor);
        }
        #endregion
        public bool IsFacingLeft() => GetProperty<bool>(EntityProperties.FACE_LEFT_AT_DEFAULT) != FlipX;

        public void ChangeModel(NamespaceID id)
        {
            ModelID = id;
            OnModelChanged?.Invoke(id);
        }
        public void SetModelProperty(string name, object value)
        {
            OnSetModelProperty?.Invoke(name, value);
        }
        public SerializableEntity Serialize()
        {
            var seri = new SerializableEntity();
            seri.id = ID;
            seri.spawnerReference = SpawnerReference;
            seri.type = Type;
            seri.state = State;
            seri.rng = RNG.Serialize();
            seri.dropRng = DropRNG.Serialize();
            seri.target = Target?.ID ?? 0;

            seri.definitionID = Definition.GetID();
            seri.modelID = ModelID;
            seri.parent = Parent?.ID ?? 0;
            seri.EquipedArmor = EquipedArmor?.Serialize();
            seri.position = Pos;
            seri.velocity = Velocity;
            seri.scale = Scale;
            seri.collisionMask = CollisionMask;
            seri.renderRotation = RenderRotation;
            seri.renderScale = RenderScale;
            seri.canUnderGround = CanUnderGround;
            seri.boundsOffset = BoundsOffset;
            seri.poolCount = PoolCount;
            seri.timeout = Timeout;

            seri.isDead = IsDead;
            seri.health = Health;
            seri.isOnGround = isOnGround;
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
            RNG = RandomGenerator.Deserialize(seri.rng);
            DropRNG = RandomGenerator.Deserialize(seri.dropRng);
            State = seri.state;
            Target = Level.FindEntityByID(seri.target);

            Definition = Level.ContentProvider.GetEntityDefinition(seri.definitionID);
            ModelID = seri.modelID;
            Parent = Level.FindEntityByID(seri.parent);
            EquipedArmor = seri.EquipedArmor != null ? Armor.Deserialize(seri.EquipedArmor, this) : null;
            Pos = seri.position;
            Velocity = seri.velocity;
            Scale = seri.scale;
            CollisionMask = seri.collisionMask;
            RenderRotation = seri.renderRotation;
            RenderScale = seri.renderScale;
            CanUnderGround = seri.canUnderGround;
            BoundsOffset = seri.boundsOffset;
            PoolCount = seri.poolCount;
            Timeout = seri.timeout;

            IsDead = seri.isDead;
            Health = seri.health;
            isOnGround = seri.isOnGround;
            propertyDict = PropertyDictionary.Deserialize(seri.propertyDict);
            buffs = BuffList.FromSerializable(seri.buffs, Level.ContentProvider, this);
            collisionThisTick = seri.collisionThisTick.ConvertAll(e => Level.FindEntityByID(e));
            collisionList = seri.collisionList.ConvertAll(e => Level.FindEntityByID(e));
            children = seri.children.ConvertAll(e => Level.FindEntityByID(e));
            takenGrids = seri.takenGrids.ConvertAll(g => Level.GetGrid(g));
        }
        public static Entity CreateDeserializingEntity(SerializableEntity seri, LevelEngine level)
        {
            return new Entity(level, seri.type, seri.id, seri.spawnerReference);
        }
        #endregion

        #region 私有方法
        private void OnInit(Entity spawner)
        {
            Health = GetMaxHealth();
        }
        private void OnUpdate()
        {
            UpdatePhysics(1);
        }
        private void OnContactGround()
        {
            HitGround(Velocity);
            Definition.PostContactGround(this);
            LevelCallbacks.PostEntityContactGround.Run(this);
        }
        private void OnLeaveGround()
        {
            Definition.PostLeaveGround(this);
            LevelCallbacks.PostEntityLeaveGround.Run(this);
        }
        private void HitGround(Vector3 velocity)
        {
            if (!EntityTypes.IsDamagable(Type))
                return;
            float fallHeight = Mathf.Max(0, GetFallDamage() - velocity.y * 5);
            float fallDamage = Mathf.Pow(fallHeight, 2);
            if (fallDamage > 0)
            {
                var effects = new DamageEffectList(DamageFlags.IGNORE_ARMOR, DamageFlags.FALL_DAMAGE);
                TakeDamage(fallDamage, effects, new EntityReferenceChain(null));
            }
        }
        private void PostCollision(Entity other, int state)
        {
            Definition.PostCollision(this, other, state);
            LevelCallbacks.PostEntityCollision.Run(this, other, state);
        }
        private bool PreTakeDamage(DamageInfo damageInfo)
        {
            return true;
        }
        private void PostTakeDamage(DamageResult bodyResult, DamageResult armorResult)
        {
            Definition.PostTakeDamage(bodyResult, armorResult);
            LevelCallbacks.PostEntityTakeDamage.Run(bodyResult, armorResult);
        }

        #endregion

        #region 事件
        public event Action PostInit;

        public event Action<string, EntityAnimationTarget> OnTriggerAnimation;
        public event Action<string, EntityAnimationTarget, bool> OnSetAnimationBool;
        public event Action<string, EntityAnimationTarget, int> OnSetAnimationInt;
        public event Action<string, EntityAnimationTarget, float> OnSetAnimationFloat;

        public event Action<Armor> OnEquipArmor;
        public event Action<Armor, DamageResult> OnDestroyArmor;
        public event Action<Armor> OnRemoveArmor;

        public event Action<NamespaceID> OnModelChanged;
        public event Action<string, object> OnSetModelProperty;
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
        public Vector3 Pos { get; set; }
        public Vector3 Velocity { get; set; }
        public Vector3 Scale { get; set; } = Vector3.one;
        public int CollisionMask { get; set; }
        public Vector3 RenderRotation { get; set; } = Vector3.zero;
        public Vector3 RenderScale { get; set; } = Vector3.one;
        public bool FlipX => Scale.x < 0;
        public bool CanUnderGround { get; set; }
        public Vector3 BoundsOffset { get; set; }
        public int PoolCount { get; set; }
        public int Timeout { get; set; }
        public bool IsDead { get; set; }
        public float Health { get; set; }
        public int Type { get; }
        public int State { get; set; }
        public Entity Target { get; set; }

        private PropertyDictionary propertyDict = new PropertyDictionary();

        private bool isOnGround = true;
        private BuffList buffs = new BuffList();
        private List<Entity> collisionThisTick = new List<Entity>();
        private List<Entity> collisionList = new List<Entity>();
        private List<LawnGrid> takenGrids = new List<LawnGrid>();
        private List<Entity> children = new List<Entity>();
        #endregion
    }
    public enum EntityAnimationTarget
    {
        Entity,
        Armor
    }
}