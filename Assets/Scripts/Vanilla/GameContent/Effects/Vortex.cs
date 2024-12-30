using MVZ2.Vanilla;
using MVZ2.Vanilla.Entities;
using MVZ2Logic.Models;
using PVZEngine.Entities;

namespace MVZ2.GameContent.Effects
{
    [Definition(VanillaEffectNames.vortex)]
    public class Vortex : EffectBehaviour
    {
        #region 公有方法
        public Vortex(string nsp, string name) : base(nsp, name)
        {
        }
        public override void Init(Entity entity)
        {
            base.Init(entity);
            entity.SetSortingLayer(SortingLayers.pool);
            entity.SetSortingOrder(100);
        }
        #endregion
    }
}