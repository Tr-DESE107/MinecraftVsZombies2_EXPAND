using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PVZEngine
{
    public abstract class Entity
    {
        #region 公有方法
        public Entity(Game level, int id, int seed)
        {
            Game = level;
            ID = id;

            RNG = new RandomGenerator(seed);
            EquipedArmor = new Armor(this);
        }
        public void Init(Entity spawner)
        {
            OnInit(spawner);
            Definition.Init(this);
            Callbacks.PostEntityInit.RunFiltered(Type, this);
        }
        public void Update()
        {
            OnUpdate();
            Definition.Update(this);
            Callbacks.PostEntityUpdate.RunFiltered(Type, this);
        }
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
                if (entRef.Entity != null)
                {
                    TriggerCollision(entRef.Entity, EntityCollision.STATE_EXIT);
                }
                collisionList.Remove(entRef);
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
        public void TriggerAnimation(string name)
        {
            OnTriggerAnimation?.Invoke(name);
        }
        public void SetAnimationBool(string name, bool value)
        {
            OnSetAnimationBool?.Invoke(name, value);
        }
        public void SetAnimationInt(string name, int value)
        {
            OnSetAnimationInt?.Invoke(name, value);
        }
        public void SetAnimationFloat(string name, float value)
        {
            OnSetAnimationFloat?.Invoke(name, value);
        }
        #endregion

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
            }
        }
        public int GetColumn()
        {
            return Game.GetColumn(Pos.x);
        }
        public int GetLane()
        {
            return Game.GetLane(Pos.z);
        }

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
        #endregion

        #region 伤害
        private static DamageResult ArmorTakeDamage(DamageInfo info)
        {
            var entity = info.Entity;
            var armor = entity.EquipedArmor;
            var shellRef = armor.GetProperty<NamespaceID>(ArmorProperties.SHELL);
            var shell = entity.Game.GetShellDefinition(shellRef);
            if (shell != null)
            {
                shell.EvaluateDamage(info);
            }
            // Apply Damage
            if (info.Amount > 0)
            {
                armor.Health -= info.Amount;
            }
            return new DamageResult()
            {
                Entity = entity,
                Source = info.Source,
                OriginalDamage = info.OriginalDamage,
                Effects = info.Effects,
                OnArmor = true
            };
        }
        private static DamageResult[] ArmoringTakeDamage(DamageInfo info)
        {
            List<DamageResult> damageResults = new List<DamageResult>();
            var entity = info.Entity;
            var armorResult = ArmorTakeDamage(info);
            damageResults.Add(armorResult);
            if (info.Effects.HasEffect(DamageFlags.DAMAGE_BOTH_ARMOR_AND_BODY))
            {
                damageResults.Add(BodyTakeDamage(info));
            }
            else if (info.Effects.HasEffect(DamageFlags.DAMAGE_BODY_AFTER_ARMOR_BROKEN) && !entity.EquipedArmor.Exists())
            {
                float overkillDamage = armorResult.OriginalDamage - armorResult.UsedDamage;
                var overkillInfo = new DamageInfo(overkillDamage, info.Effects, entity, info.Source);
                damageResults.Add(BodyTakeDamage(overkillInfo));
            }
            return damageResults.ToArray();
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
            entity.Health -= info.Amount;

            if (entity.Health <= 0)
            {
                entity.Die(info.Effects, info.Source);
            }

            return new DamageResult()
            {
                OriginalDamage = info.OriginalDamage,
                Effects = info.Effects,
                Source = info.Source,
                Entity = entity,
                UsedDamage = info.GetUsedDamage()
            };
        }
        public DamageResult[] TakeDamage(float amount, DamageEffectList effects, EntityReference source)
        {
            return TakeDamage(new DamageInfo(amount, effects, this, source));
        }
        public DamageResult[] TakeDamage(DamageInfo info)
        {
            if (IsInvincible() || IsDead)
                return null;
            if (!PreTakeDamage(info))
                return null;
            if (info.Amount <= 0)
                return null;
            DamageResult[] results;
            if (EquipedArmor.Exists() && !info.Effects.HasEffect(DamageFlags.IGNORE_ARMOR))
            {
                results = ArmoringTakeDamage(info);
            }
            else
            {
                results = new DamageResult[] { BodyTakeDamage(info) };
            }
            PostTakeDamage(info);
            return results;
        }


        public void Die(DamageEffectList effects = null, EntityReference source = null)
        {
            effects = effects ?? new DamageEffectList();
            source = source ?? new EntityReference();
            IsDead = true;
            Definition.PostDeath(this, effects, source);
            Callbacks.PostEntityDeath.Run(this, effects, source);
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
                        result = modi.CalculateProperty(this, buff, result);
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
            buffs.Add(buff);
            buff.AddToEntity(this);
        }
        public bool RemoveBuff(Buff buff)
        {
            if (buffs.Remove(buff))
            {
                buff.RemoveFromEntity();
                return true;
            }
            return false;
        }
        public bool HasBuff(Buff buff)
        {
            return buffs.Contains(buff);
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
            return Pos + velocity * simulationSpeed;
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
            if (!CanUnderGround)
            {
                var pos = Pos;
                pos.y = Mathf.Max(GetGroundHeight(), pos.y);
                Pos = pos;

                var vel = Velocity;
                vel.y = Mathf.Max(0, vel.y);
                Velocity = vel;
            }
        }
        #endregion
        public virtual bool IsFacingLeft() => FlipX;

        #endregion

        #region 私有方法
        protected virtual void OnInit(Entity spawner)
        {
            SpawnerReference = new EntityReference(spawner);
            Health = GetMaxHealth();
        }
        protected virtual void OnUpdate()
        {
            UpdateGridBelow();
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

        private void UpdateGridBelow()
        {
            // Update grid below
            Grid grid = Game.GetGrid(GetColumn(), GetLane());
            if (GridBelow != grid)
            {
                Grid before = GridBelow;
                GridBelow = grid;
            }
        }
        private void WarpLanesUpdate()
        {
            if (!IsWarpingLane)
                return;

            float targetZ = Game.GetUnitLaneZ(WarpTargetLane);
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
        private void PostTakeDamage(DamageInfo damageInfo)
        {
        }

        #endregion

        #region 事件
        public event Action<string> OnTriggerAnimation;
        public event Action<string, bool> OnSetAnimationBool;
        public event Action<string, int> OnSetAnimationInt;
        public event Action<string, float> OnSetAnimationFloat;
        #endregion

        #region 属性字段
        public int ID { get; private set; }
        public RandomGenerator RNG { get; private set; }
        public bool Removed { get; private set; }
        public EntityDefinition Definition { get; set; }
        public EntityReference SpawnerReference { get; private set; }
        public Game Game { get; private set; }
        public Grid GridBelow { get; private set; }
        public Armor EquipedArmor { get; private set; }
        public Vector3 Pos { get; set; }
        public Vector3 Velocity { get; set; }
        public Vector3 Scale { get; set; } = Vector3.one;
        public int CollisionMask { get; set; }
        public Vector3 RenderScale { get; set; }
        public bool FlipX => Scale.x < 0;
        public bool CanUnderGround { get; set; }
        public Vector3 BoundsOffset { get; set; }
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
        #endregion
    }
}