using MVZ2.Vanilla;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Entities;
using MVZ2Logic.Level;
using MVZ2Logic.Models;
using PVZEngine.Entities;

namespace MVZ2.GameContent.Effects
{
    [Definition(VanillaEffectNames.rain)]
    public class Rain : EffectBehaviour
    {

        #region 公有方法
        public Rain(string nsp, string name) : base(nsp, name)
        {
        }
        public override void Init(Entity entity)
        {
            base.Init(entity);
            entity.Level.AddLoopSoundEntity(VanillaSoundID.rain, entity.ID);
            entity.SetSortingLayer(SortingLayers.foreground);
            entity.SetSortingOrder(9999);
        }
        #endregion
    }
}