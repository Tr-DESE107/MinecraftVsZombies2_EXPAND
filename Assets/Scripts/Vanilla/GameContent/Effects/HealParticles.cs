using MVZ2.Vanilla;
using MVZ2.Vanilla.Entities;
using PVZEngine;
using PVZEngine.Entities;

namespace MVZ2.GameContent.Effects
{
    [Definition(VanillaEffectNames.healParticles)]
    public class HealParticles : EffectBehaviour
    {
        #region 公有方法
        public HealParticles(string nsp, string name) : base(nsp, name)
        {
        }
        public override void Update(Entity entity)
        {
            base.Update(entity);
            var parent = entity.Parent;
            if (parent != null && parent.Exists())
            {
                entity.Timeout = entity.GetMaxTimeout();
                entity.Position = parent.Position;
            }
            entity.SetModelProperty("EmitSpeed", GetEmitSpeed(entity));
            entity.SetModelProperty("Size", entity.GetScaledSize());
            SetEmitSpeed(entity, 0);
        }
        public static float GetEmitSpeed(Entity entity)
        {
            return entity.GetBehaviourField<float>(ID, "EmitSpeed");
        }
        public static void SetEmitSpeed(Entity entity, float value)
        {
            entity.SetBehaviourField(ID, "EmitSpeed", value);
        }
        public static void AddEmitSpeed(Entity entity, float value)
        {
            SetEmitSpeed(entity, GetEmitSpeed(entity) + value);
        }
        #endregion

        public static readonly NamespaceID ID = VanillaEffectID.healParticles;
    }
}