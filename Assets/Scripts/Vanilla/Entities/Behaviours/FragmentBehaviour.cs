using MVZ2.GameContent.Damages;
using MVZ2.GameContent.Effects;
using MVZ2.Vanilla.Contraptions;
using PVZEngine.Damages;
using PVZEngine.Entities;
using PVZEngine.Level;

namespace MVZ2.Vanilla.Entities
{
    [EntityBehaviourDefinition(VanillaEntityBehaviourNames.fragmented)]
    public class FragmentBehaviour : EntityBehaviourDefinition
    {
        public FragmentBehaviour(string nsp, string name) : base(nsp, name)
        {
        }
        public override void Init(Entity entity)
        {
            base.Init(entity);
            var fragment = entity.CreateFragment();
            var fragmentRef = new EntityID(fragment);
            entity.SetFragment(fragmentRef);
        }
        public override sealed void Update(Entity entity)
        {
            base.Update(entity);
            var fragment = entity.GetOrCreateFragment();
            Fragment.AddEmitSpeed(fragment, entity.GetFragmentTickDamage());
            entity.SetFragmentTickDamage(0);
        }
        public override void PostDeath(Entity entity, DeathInfo damageInfo)
        {
            base.PostDeath(entity, damageInfo);
            if (!damageInfo.HasEffect(VanillaDamageEffects.SACRIFICE) && !damageInfo.HasEffect(VanillaDamageEffects.REMOVE_ON_DEATH))
            {
                var fragment = entity.GetOrCreateFragment();
                Fragment.AddEmitSpeed(fragment, 500);
            }
        }
        public override void PostTakeDamage(DamageOutput result)
        {
            base.PostTakeDamage(result);
            var bodyResult = result.BodyResult;
            if (bodyResult != null && !bodyResult.Entity.NoDamageFragments())
            {
                bodyResult.Entity.AddFragmentTickDamage(bodyResult.Amount);
            }
        }
    }
}