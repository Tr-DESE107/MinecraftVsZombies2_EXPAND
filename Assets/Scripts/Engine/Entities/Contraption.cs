namespace PVZEngine
{
    public class Contraption : Entity
    {
        #region 公有方法
        #region 构造方法
        public Contraption(Game level, int id, int seed) : base(level, id, seed)
        {
            SetFaction(Game.Option.LeftFaction);
        }
        #endregion

        #region 生命周期
        protected override void OnInit(Entity spawner)
        {
            base.OnInit(spawner);
            SetFallDamage(22.5f);
        }
        protected override void OnUpdate()
        {
            base.OnUpdate();
            if (GetRelativeY() > leaveGridHeight || WaitingDestroy)
            {
                ReleaseGrids();
            }
            else
            {
                TakeGrids();
            }
        }
        public override void Remove()
        {
            base.Remove();
            ReleaseGrids();
        }
        #endregion

        #region 原版属性
        public bool IsFloor()
        {
            return GetProperty<bool>(ContraptionProperties.IS_FLOOR);
        }
        #endregion
        public void DigUp()
        {
            Die(new DamageEffects());
        }
        #endregion

        #region 私有方法
        private void ReleaseGrids()
        {
            if (TakenGrid == null)
                return;
            TakenGrid = null;
        }
        private void TakeGrids()
        {
            TakenGrid = GridBelow;
        }
        #endregion

        #region Properties
        public override int Type => EntityTypes.CONTRAPTION;
        public int PoolCount { get; set; }
        public Grid TakenGrid { get; set; }
        private const float leaveGridHeight = 0.64f;
        #endregion
    }
}