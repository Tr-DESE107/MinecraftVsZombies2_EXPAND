using UnityEngine;
using UnityEngine.EventSystems;

namespace PVZEngine
{
    public class Pickup : Entity
    {
        #region 公有方法
        public Pickup(Game level, int id, int seed) : base(level, id, seed)
        {
        }
        protected override void OnInit(Entity spawner)
        {
            Timeout = GetMaxTimeout();
        }
        protected override void OnUpdate()
        {
            base.OnUpdate();
            if (!IsCollected)
            {
                if (!IsImportant())
                {
                    Timeout--;
                    if (Timeout <= 0)
                    {
                        Remove();
                    }
                }
            }
            else
            {
                CollectedTime++;
            }
        }
        public int GetMaxTimeout()
        {
            return GetProperty<int>(PickupProperties.MAX_TIMEOUT);
        }
        public bool IsImportant()
        {
            return GetProperty<bool>(PickupProperties.IMPORTANT);
        }
        #endregion

        #region 属性字段
        public override int Type => EntityTypes.PICKUP;
        public int Timeout { get; set; }
        public int CollectedTime { get; set; }
        public bool IsCollected { get; set; }
        #endregion
    }

}