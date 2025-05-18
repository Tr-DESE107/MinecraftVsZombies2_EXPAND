using MVZ2.GameContent.Buffs.Contraptions;
using MVZ2.GameContent.Damages;
using MVZ2.Vanilla.Callbacks;
using MVZ2.Vanilla.Contraptions;
using MVZ2.Vanilla.Level;
using PVZEngine.Callbacks;
using PVZEngine.Damages;
using PVZEngine.Entities;
using PVZEngine.Level;

namespace MVZ2.Vanilla.Entities
{
    [EntityBehaviourDefinition(VanillaEntityBehaviourNames.contraptionCommon)]
    public class ContraptionCommonBehaviour : EntityBehaviourDefinition
    {
        public ContraptionCommonBehaviour(string nsp, string name) : base(nsp, name)
        {
        }
        public override void Init(Entity entity)
        {
            base.Init(entity);
            if (entity.IsNocturnal() && entity.Level.IsDay())
            {
                entity.AddBuff<NocturnalBuff>();
            }
        }
        public override void PostDeath(Entity entity, DeathInfo damageInfo)
        {
            base.PostDeath(entity, damageInfo);
            if (damageInfo.Effects.HasEffect(VanillaDamageEffects.SACRIFICE))
            {
                entity.AddBuff<SacrificedBuff>();
            }
            else
            {
                entity.PlaySound(entity.GetDeathSound(), entity.GetCryPitch());
                entity.Remove();
            }
            if (!damageInfo.Effects.HasEffect(VanillaDamageEffects.SELF_DAMAGE))
            {
                entity.Level.Triggers.RunCallbackFiltered(VanillaLevelCallbacks.POST_CONTRAPTION_DESTROY, new EntityCallbackParams(entity), entity.GetDefinitionID());
            }
        }

    }
}