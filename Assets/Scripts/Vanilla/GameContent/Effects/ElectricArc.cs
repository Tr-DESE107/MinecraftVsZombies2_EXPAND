using MVZ2.Vanilla;
using MVZ2.Vanilla.Entities;
using PVZEngine.Entities;
using UnityEngine;

namespace MVZ2.GameContent.Effects
{
    [Definition(VanillaEffectNames.electricArc)]
    public class ElectricArc : EffectBehaviour
    {
        #region 公有方法
        public ElectricArc(string nsp, string name) : base(nsp, name)
        {
        }
        public override void Init(Entity entity)
        {
            base.Init(entity);
            entity.SetModelProperty("Source", entity.Position);
        }
        public override void Update(Entity entity)
        {
            base.Update(entity);
            entity.SetModelProperty("Source", entity.Position);
            var tint = entity.GetTint();
            tint.a = entity.Timeout / (float)entity.GetMaxTimeout();
            entity.SetTint(tint);
        }
        public static void Connect(Entity arc, Vector3 position)
        {
            arc.SetModelProperty("Dest", position);
        }
        #endregion
    }
}