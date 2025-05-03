using MVZ2.Vanilla.Entities;
using PVZEngine.Entities;
using PVZEngine.Level;

namespace MVZ2.GameContent.Effects
{
    [EntityBehaviourDefinition(VanillaEffectNames.magneticLine)]
    public class MagneticLine : EffectBehaviour
    {
        #region 公有方法
        public MagneticLine(string nsp, string name) : base(nsp, name)
        {
        }
        public override void Init(Entity entity)
        {
            base.Init(entity);
            entity.SetModelProperty("Source", entity.Position);
            entity.SetModelProperty("Dest", entity.Position);
        }
        public override void Update(Entity entity)
        {
            base.Update(entity);
            if (entity.Parent == null || !entity.Parent.Exists() || entity.Parent.IsDead)
            {
                entity.Remove();
            }
            if (entity.Target == null || !entity.Target.Exists() || entity.Target.IsDead)
            {
                entity.Remove();
            }
            entity.Position = entity.Parent.Position;
            entity.SetModelProperty("Source", entity.Position);
            entity.SetModelProperty("Dest", entity.Target.GetCenter());
        }
        #endregion
    }
}