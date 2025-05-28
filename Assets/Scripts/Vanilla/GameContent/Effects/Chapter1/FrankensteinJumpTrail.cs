using MVZ2.Vanilla.Entities;
using PVZEngine.Entities;
using PVZEngine.Level;

namespace MVZ2.GameContent.Effects
{
    [EntityBehaviourDefinition(VanillaEffectNames.frankensteinJumpTrail)]
    public class FrankensteinJumpTrail : EffectBehaviour
    {
        #region ���з���
        public FrankensteinJumpTrail(string nsp, string name) : base(nsp, name)
        {
        }
        public override void Init(Entity entity)
        {
            base.Init(entity);
            var tint = entity.GetTint();
            tint.a = 0.5f;
            entity.SetTint(tint);
        }
        public override void Update(Entity entity)
        {
            base.Update(entity);
            var tint = entity.GetTint();
            tint.a = entity.Timeout / (float)entity.GetMaxTimeout() * 0.5f;
            entity.SetTint(tint);
        }
        #endregion
    }
}