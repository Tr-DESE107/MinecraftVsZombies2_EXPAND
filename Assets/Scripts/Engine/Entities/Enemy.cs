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
        }
        #endregion
        public override bool IsFacingLeft()
        {
            return !FlipX;
        }
        public void SetPreviewMode()
        {
            IsPreview = true;
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