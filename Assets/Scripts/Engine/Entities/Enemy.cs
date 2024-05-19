using UnityEngine;

namespace PVZEngine
{
    public class Enemy : Entity
    {
        #region 公有方法
        #region 构造方法
        public Enemy(Game level, int id, int seed) : base(level, id, seed)
        {
            dropRNG = new RandomGenerator(RNG.Next());
            SetFaction(Game.Option.RightFaction);
        }
        #endregion
        protected override void OnInit(Entity spawner)
        {
            base.OnInit(spawner);
            SetFallDamage(22.5f);
        }
        public override bool IsFacingLeft()
        {
            return !FlipX;
        }
        /// <summary>
        /// 是否有害。如果为true，则无法进屋，也不会被计数。
        /// </summary>
        public bool IsNotHarmful()
        {
            return false;
        }
        public void SetPreviewMode()
        {
            IsPreview = true;
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();

            var velocity = Velocity;
            var speed = GetSpeed();
            if (Mathf.Abs(velocity.x) < speed)
            {
                float min = Mathf.Min(speed, -speed);
                float max = Mathf.Max(speed, -speed);
                float direciton = IsFacingLeft() ? -1 : 1;
                velocity.x += speed * direciton;
                velocity.x = Mathf.Clamp(velocity.x, min, max);
            }
            Velocity = velocity;
        }
        #region 原版属性
        public float GetSpeed()
        {
            return GetProperty<float>(EnemyProperties.SPEED);
        }
        public float GetMaxAttackHeight()
        {
            return GetProperty<float>(EnemyProperties.MAX_ATTACK_HEIGHT);
        }
        #endregion

        #endregion

        #region 私有方法

        private void Attack(Entity target)
        {
            if (target == null)
                return;
            target.TakeDamage(GetDamage() * GetAttackSpeed() / Game.Option.TPS, new DamageEffectList(), new EntityReference(this));
        }

        #endregion

        #region 属性字段
        public override int Type => EntityTypes.ENEMY;
        public int ActionState { get; set; }
        public Entity AttackTarget { get; set; }
        public bool HasStarshard { get; set; }
        public bool HasRedstone { get; set; }
        public bool IsPreview { get; private set; }
        protected bool isEnteredHouse;
        private RandomGenerator dropRNG;
        #endregion
    }
}