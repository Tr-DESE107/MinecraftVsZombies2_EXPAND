using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.PackageManager;
using UnityEngine;

namespace PVZEngine
{
    public abstract class Entity : IBuffTarget
    {
        #region 公有方法
        public Entity(Game level, int id, EntityDefinition definition, int seed)
        {
            Game = level;
            ID = id;

            Definition = definition;
            ModelID = definition.GetModelID();
            RNG = new RandomGenerator(seed);
            SetTint(Color.white);
        }
        public void Init(Entity spawner)
        {
            OnInit(spawner);
            Definition.Init(this);
            Callbacks.PostEntityInit.RunFiltered(Type, this);
            PostInit?.Invoke();
        }
        public void Update()
        {
            OnUpdate();
            Definition.Update(this);
            if (EquipedArmor != null)
                EquipedArmor.Update();
            Callbacks.PostEntityUpdate.RunFiltered(Type, this);
        }
        public void SetParent(Entity parent)
        {
            var oldParent = Parent?.GetEntity(Game);
            if (oldParent != null)
            {
                oldParent.children.RemoveAll(r => r.ID == ID);
            }
            if (parent == null)
            {
                Parent = new EntityReference(null);
            }
            else
            {
                Parent = new EntityReference(parent);
                parent.children.Add(new EntityReference(this));
            }
        }
        public Entity[] GetChildren()
        {
            return children.Select(c => c.GetEntity(Game)).ToArray();
        }
        public bool Exists()
        {
            return !Removed;
        }
        public virtual void Remove()
        {
            if (!Removed)
            {
                Removed = true;
                Game.RemoveEntity(this);
                Definition.PostRemove(this);
                Callbacks.PostEntityRemove.Run(this);
            }
        }

        #region Warp Lane
        public void StartWarpingLane(int target)
        {
            if (target < 1 || target > Game.GetMaxLaneCount())
                return;
            IsWarpingLane = true;
            WarpTargetLane = target;
            WarpFromLane = GetLane();
        }
        public virtual void StopWarpingLane()
        {
            if (!IsWarpingLane)
                return;
            IsWarpingLane = false;
            WarpTargetLane = GetLane();
            WarpFromLane = GetLane();
        }
        #endregion Warp Lane

        #region 子类转换
        public Contraption ToContraption()
        {
            return this as Contraption;
        }
        public Enemy ToEnemy()
        {
            return this as Enemy;
        }
        public Cart ToCart()
        {
            return this as Cart;
        }
        public Effect ToEffect()
        {
            return this as Effect;
        }
        public Pickup ToPickup()
        {
            return this as Pickup;
        }
        public Projectile ToProjectile()
        {
            return this as Projectile;
        }
        #endregion

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
            return GetProperty<float>(EntityProperties.DAMAGE, ignoreBuffs : ignoreBuffs);
        }
        public void SetDamage(float value)
        {
            SetProperty(EntityProperties.DAMAGE, value);
        }
        public float GetAttackSpeed()
        {
            return GetProperty<float>(EntityProperties.ATTACK_SPEED);
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
            var shell = entity.Game.GetShellDefinition(shellRef);
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
        public DamageResult TakeDamage(float amount, DamageEffectList effects, EntityReference source)
        {
            return TakeDamage(amount, effects, source, out _);
        }
        public DamageResult TakeDamage(float amount, DamageEffectList effects, EntityReference source, out DamageResult armorResult)
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
            info = info ?? new DamageInfo(0, new DamageEffectList(), this, new EntityReference(null));
            IsDead = true;
            Definition.PostDeath(this, info);
            Callbacks.PostEntityDeath.Run(this, info);
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
            if (propertyDict.TryGetValue(name, out var prop))
                result = prop;
            else if (!ignoreDefinition)
                result = Definition.GetProperty<object>(name);

            if (!ignoreBuffs)
            {
                foreach (var buff in buffs)
                {
                    foreach (var modi in buff.GetModifiers())
                    {
                        if (modi.PropertyName != name)
                            continue;
                        result = modi.CalculateProperty(buff, result);
                    }
                }
            }
            return result;
        }
        public T GetProperty<T>(string name, bool ignoreDefinition = false, bool ignoreBuffs = false)
        {
            return PropertyDictionary.ToGeneric<T>(GetProperty(name, ignoreDefinition, ignoreBuffs));
        }
        public void SetProperty(string name, object value)
        {
            propertyDict[name] = value;
        }
        #endregion

        #region 增益
        public void AddBuff(Buff buff)
        {
            if (buff == null)
                return;
            buffs.Add(buff);
            buff.AddToTarget(this);
        }
        public void AddBuff<T>() where T: BuffDefinition
        {
            AddBuff(Game.CreateBuff<T>());
        }
        public bool RemoveBuff(Buff buff)
        {
            if (buff == null)
                return false;
            if (buffs.Remove(buff))
            {
                buff.RemoveFromTarget();
                return true;
            }
            return false;
        }
        public int RemoveBuffs(IEnumerable<Buff> buffs)
        {
            if (buffs == null)
                return 0;
            int count = 0;
            foreach (var buff in buffs)
            {
                count += RemoveBuff(buff) ? 1 : 0;
            }
            return count;
        }
        public bool HasBuff<T>() where T : BuffDefinition
        {
            return buffs.Any(b => b.Definition is T);
        }
        public bool HasBuff(Buff buff)
        {
            return buffs.Contains(buff);
        }
        public Buff[] GetBuffs<T>() where T: BuffDefinition
        {
            return buffs.Where(b => b.Definition is T).ToArray();
        }
        public Buff[] GetAllBuffs()
        {
            return buffs.ToArray();
        }
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
            return Game.GetGroundHeight(Pos.x, Pos.z);
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
            return Game.GetColumn(Pos.x);
        }
        public int GetLane()
        {
            return Game.GetLane(Pos.z);
        }
        public void TakeGrid(int index)
        {
            takenGrids.Add(index);
            var grid = Game.GetGrid(index);
            grid.AddEntity(this);
        }
        public bool ReleaseGrid(int index)
        {
            if (takenGrids.Remove(index))
            {
                var grid = Game.GetGrid(index);
                grid.RemoveEntity(this);
                return true;
            }
            return false;
        }
        public void ClearTakenGrids()
        {
            foreach (var gridIndex in takenGrids)
            {
                var grid = Game.GetGrid(gridIndex);
                grid.RemoveEntity(this);
            }
            takenGrids.Clear();
        }
        public int[] GetTakenGrids()
        {
            return takenGrids.ToArray();
        }
        public int GetGridIndex()
        {
            return Game.GetGridIndex(GetColumn(), GetLane());
        }
        public Grid GetGrid()
        {
            return Game.GetGrid(GetColumn(), GetLane());
        }
        #endregion

        #region 碰撞
        public void Collide(Entity other)
        {
            var reference = new EntityReference(other);
            if (collisionList.Contains(reference))
            {
                TriggerCollision(other, EntityCollision.STATE_ENTER);
            }
            else
            {
                TriggerCollision(other, EntityCollision.STATE_STAY);
                collisionList.Add(reference);
            }
            collisionThisTick.Add(reference);
        }
        public void ClearCollision()
        {
            var notCollided = collisionList.Except(collisionThisTick).ToArray();
            foreach (var entRef in notCollided)
            {
                var ent = entRef?.GetEntity(Game);
                if (ent != null)
                {
                    TriggerCollision(ent, EntityCollision.STATE_EXIT);
                }
                collisionList.Remove(entRef);
            }
        }
        #endregion

        #region 护甲
        public void EquipArmor<T>() where T : ArmorDefinition
        {
            EquipArmor(new Armor(this, Game.GetArmorDefinition<T>()));
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
            Callbacks.PostEquipArmor.Run(this, armor);
            OnEquipArmor?.Invoke(armor);
        }
        public void DestroyArmor(Armor armor, DamageResult result)
        {
            Definition.PostDestroyArmor(this, armor, result);
            Callbacks.PostDestroyArmor.Run(this, armor, result);
            OnDestroyArmor?.Invoke(armor, result);
        }
        public void RemoveArmor()
        {
            var armor = EquipedArmor;
            if (armor == null)
                return;
            EquipedArmor = null;
            Definition.PostRemoveArmor(this, armor);
            Callbacks.PostRemoveArmor.Run(this, armor);
            OnRemoveArmor?.Invoke(armor);
        }
        #endregion
        public virtual bool IsFacingLeft() => FlipX;

        public void ChangeModel(NamespaceID id)
        {
            ModelID = id;
            OnModelChanged?.Invoke(id);
        }
        public void SetModelProperty(string name, object value)
        {
            OnSetModelProperty?.Invoke(name, value);
        }

        #endregion

        #region 私有方法
        protected virtual void OnInit(Entity spawner)
        {
            SpawnerReference = new EntityReference(spawner);
            Health = GetMaxHealth();
        }
        protected virtual void OnUpdate()
        {
            UpdatePhysics(1);
            WarpLanesUpdate();
        }
        protected virtual void OnCollision(Entity other, int state)
        {

        }
        private void OnContactGround()
        {
            HitGround(Velocity);
            Definition.PostContactGround(this);
            Callbacks.PostEntityContactGround.Run(this);
        }
        private void OnLeaveGround()
        {
            Definition.PostLeaveGround(this);
            Callbacks.PostEntityLeaveGround.Run(this);
        }
        protected void HitGround(Vector3 velocity)
        {
            if (!EntityTypes.IsDamagable(Type))
                return;
            float fallHeight = Mathf.Max(0, GetFallDamage() - velocity.y * 5);
            float fallDamage = Mathf.Pow(fallHeight, 2);
            if (fallDamage > 0)
            {
                var effects = new DamageEffectList(DamageFlags.IGNORE_ARMOR, DamageFlags.FALL_DAMAGE);
                TakeDamage(fallDamage, effects, new EntityReference(null));
            }
        }
        protected void TriggerCollision(Entity other, int state)
        {
            OnCollision(other, state);
            Definition.PostCollision(this, other, state);
            Callbacks.PostEntityCollision.Run(this, other, state);
        }

        private void WarpLanesUpdate()
        {
            if (!IsWarpingLane)
                return;

            float targetZ = Game.GetEntityLaneZ(WarpTargetLane);
            bool passed;
            // Warp upwards.
            if (WarpFromLane > WarpTargetLane)
            {
                passed = Pos.z >= targetZ - 0.03f;
            }
            // Warp downwards.
            else
            {
                passed = Pos.z <= targetZ + 0.03f;
            }

            if (passed)
            {
                if (Mathf.Abs(Pos.z - targetZ) <= 0.05f)
                {
                    var pos = Pos;
                    pos.z = targetZ;
                    Pos = pos;
                }
                StopWarpingLane();
                return;
            }

            Vector3 velocity = Velocity;
            float warpSpeed = WarpLaneSpeed;

            // Warp upwards.
            if (WarpFromLane > WarpTargetLane)
            {
                velocity.z = Mathf.Max(warpSpeed, Velocity.z);
            }
            // Warp downwards.
            else
            {
                velocity.z = Mathf.Min(-warpSpeed, Velocity.z);
            }
            Velocity = velocity;
        }
        private bool PreTakeDamage(DamageInfo damageInfo)
        {
            return true;
        }
        private void PostTakeDamage(DamageResult bodyResult, DamageResult armorResult)
        {
            Definition.PostTakeDamage(bodyResult, armorResult);
            Callbacks.PostEntityTakeDamage.Run(bodyResult, armorResult);
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
        public int ID { get; private set; }
        public RandomGenerator RNG { get; private set; }
        public bool Removed { get; private set; }
        public EntityDefinition Definition { get; private set; }
        public NamespaceID ModelID { get; private set; }
        public EntityReference SpawnerReference { get; private set; }
        public EntityReference Parent { get; private set; }
        public Game Game { get; private set; }
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
        #region Warp Lane
        public bool IsWarpingLane { get; private set; }
        public int WarpTargetLane { get; private set; }
        public int WarpFromLane { get; private set; }
        public float WarpLaneSpeed { get; set; } = 2.5f;
        #endregion

        #region 影子
        public bool ShadowVisible { get; set; } = true;
        public float ShadowAlpha { get; set; } = 1;
        public Vector3 ShadowScale { get; set; } = Vector3.one;
        public Vector3 ShadowOffset { get; set; }
        #endregion
        public bool IsDead { get; set; }
        public float Health { get; set; }
        public abstract int Type { get; }

        private Dictionary<string, object> propertyDict = new Dictionary<string, object>();

        private bool isOnGround = true;
        private List<Buff> buffs = new List<Buff>();
        private List<EntityReference> collisionThisTick = new List<EntityReference>();
        private List<EntityReference> collisionList = new List<EntityReference>();
        private List<int> takenGrids = new List<int>();
        private List<EntityReference> children = new List<EntityReference>();
        #endregion
    }
    public enum EntityAnimationTarget
    {
        Entity,
        Armor
    }
}