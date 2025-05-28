using MVZ2.Vanilla.Entities;
using PVZEngine.Entities;
using PVZEngine.Level;
using UnityEngine;

namespace MVZ2.GameContent.Effects
{
    [EntityBehaviourDefinition(VanillaEffectNames.electricArc)]
    public class ElectricArc : EffectBehaviour
    {
        #region ���з���
        public ElectricArc(string nsp, string name) : base(nsp, name)
        {
        }
        public override void Init(Entity entity)
        {
            base.Init(entity);
            entity.SetModelProperty("Source", entity.Position);
            entity.SetModelProperty("PointCount", 50);
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
            arc.SetModelProperty("Source", arc.Position);
            arc.SetModelProperty("Dest", position);
        }
        public static void SetPointCount(Entity arc, int count)
        {
            arc.SetModelProperty("PointCount", count);
        }
        public static void UpdateArc(Entity arc)
        {
            arc.TriggerModel("Update");
        }
        #endregion
    }
}