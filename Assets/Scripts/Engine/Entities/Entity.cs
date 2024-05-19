using System;
using System.Collections.Generic;
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

            LawnRigid = new LawnRigidbody(level);
            LawnRigid.OnContactGround += onContactGround;
            LawnRigid.OnLeaveGround += onLeaveGround;
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
        public virtual void OnEntityCollisionEnter(Entity other, bool actively)
        {

        }
        public virtual void OnEntityCollisionStay(Entity other, bool actively)
        {

        }
        public virtual void OnEntityCollisionExit(Entity other, bool actively)
        {

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
        #endregion

        public bool Exists()
        {
            return !WaitingDestroy;
        }
        public virtual void Remove()
        {
            if (!WaitingDestroy)
            {
                WaitingDestroy = true;
                Game.RemoveEntity(this);
            }
        }

        public Bounds GetBounds()
        {
            return LawnRigid.GetBounds();
        }
        public float GetRelativeY()
        {
            return LawnRigid.GetRelativeY();
        }
        public void SetRelativeY(float value)
        {
            LawnRigid.SetRelativeY(value);
        }
        public Vector3 GetScaledSize()
        {
            return LawnRigid.GetScaledSize();
        }
        public Vector3 GetScaledBoundsOffset()
        {
            return LawnRigid.GetScaledBoundsOffset();
        }
        public float GetGroundHeight()
        {
            return LawnRigid.GetGroundHeight();
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
        public float GetDamage()
        {
            return GetProperty<float>(EntityProperties.DAMAGE);
        }
        public float GetAttackSpeed()
        {
            return GetProperty<float>(EntityProperties.ATTACK_SPEED);
        }
        public float GetFriction()
        {
            return GetProperty<float>(EntityProperties.FRICTION);
        }
        public float GetHealth()
        {
            return GetProperty<float>(EntityProperties.HEALTH);
        }
        public void SetHealth(float value)
        {
            SetProperty(EntityProperties.HEALTH, value);
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
            var health = entity.GetHealth();
            health -= info.Amount;
            entity.SetHealth(health);

            if (health <= 0)
            {
                entity.Die(info.Effects);
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


        public void Die(DamageEffectList effects = null)
        {
            IsDead = true;
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
            return (!IsDead || includeDead) && !WaitingDestroy;
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
                    result = buff.ModifyProperty(result);
                }
            }
            return result;
        }
        public T GetProperty<T>(string name, bool ignoreDefinition = false, bool ignoreBuffs = false)
        {
            if (GetProperty(name, ignoreDefinition, ignoreBuffs) is T tProp)
            {
                return tProp;
            }
            return default;
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
        public virtual bool IsFacingLeft() => FlipX;

        #endregion

        #region 私有方法
        protected virtual void OnInit(Entity spawner)
        {
            SpawnerReference = new EntityReference(spawner);
            LawnRigid.Gravity = GetGravity();
            LawnRigid.Friction = GetFriction();
        }
        protected virtual void OnUpdate()
        {
            UpdateGridBelow();
            LawnRigid.Gravity = GetGravity();
            LawnRigid.Friction = GetFriction();
            LawnRigid.Update();
            WarpLanesUpdate();
        }
        private void onContactGround()
        {
            HitGround(Velocity);
            Definition.PostContactGround(this);
            Callbacks.OnEntityContactGround.Run(this);
        }
        private void onLeaveGround()
        {
            Definition.PostLeaveGround(this);
            Callbacks.OnEntityLeaveGround.Run(this);
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
        #endregion

        #region 属性字段
        public int ID { get; private set; }
        public RandomGenerator RNG { get; private set; }
        public bool WaitingDestroy { get; private set; }
        public EntityDefinition Definition { get; set; }
        public EntityReference SpawnerReference { get; private set; }
        public Game Game { get; private set; }
        public LawnRigidbody LawnRigid { get; private set; }
        public Grid GridBelow { get; private set; }
        public Armor EquipedArmor { get; private set; }
        public Vector3 Velocity { get => LawnRigid.Velocity; set => LawnRigid.Velocity = value; }
        public Vector3 Pos { get => LawnRigid.Pos; set => LawnRigid.Pos = value; }
        public Vector3 Size { get => LawnRigid.Size; set => LawnRigid.Size = value; }
        public Vector3 Scale { get => LawnRigid.Scale; set => LawnRigid.Scale = value; }
        public Vector3 RenderScale { get; set; }
        public bool FlipX => Scale.x >= 0;
        public bool CanUnderGround { get => LawnRigid.CanUnderGround; set => LawnRigid.CanUnderGround = value; }
        #region Warp Lane
        public bool IsWarpingLane { get; private set; }
        public int WarpTargetLane { get; private set; }
        public int WarpFromLane { get; private set; }
        public float WarpLaneSpeed { get; set; } = 2.5f;
        #endregion

        #region 影子
        public bool ShadowVisible { get; set; }
        public float ShadowAlpha { get; set; }
        public Vector3 ShadowScale { get; set; }
        public Vector3 ShadowOffset { get; set; }
        #endregion
        public bool IsDead { get; set; }
        public abstract int Type { get; }

        private Dictionary<string, object> propertyDict = new Dictionary<string, object>();

        private List<Buff> buffs = new List<Buff>();
        #endregion
    }
}