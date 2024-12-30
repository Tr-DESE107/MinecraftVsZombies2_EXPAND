using System.Linq;
using MVZ2.GameContent.Areas;
using MVZ2.GameContent.Buffs.Contraptions;
using MVZ2.GameContent.Contraptions;
using MVZ2.GameContent.Damages;
using MVZ2.GameContent.Effects;
using MVZ2.Vanilla.Callbacks;
using MVZ2.Vanilla.Contraptions;
using MVZ2.Vanilla.Grids;
using MVZ2.Vanilla.Level;
using PVZEngine.Damages;
using PVZEngine.Definitions;
using PVZEngine.Entities;

namespace MVZ2.Vanilla.Entities
{
    public abstract class ContraptionBehaviour : VanillaEntityBehaviour, IEvokableContraption, ITriggerableContraption
    {
        public ContraptionBehaviour(string nsp, string name) : base(nsp, name)
        {
            SetProperty(VanillaContraptionProps.FRAGMENT_ID, GetID());
        }
        public override void Init(Entity entity)
        {
            base.Init(entity);
            entity.SetFaction(entity.Level.Option.LeftFaction);
            UpdateTakenGrids(entity);
            entity.InitFragment();

            if (entity.IsNocturnal() && entity.Level.IsDay())
            {
                entity.AddBuff<NocturnalBuff>();
            }
        }
        public override sealed void Update(Entity entity)
        {
            base.Update(entity);
            if (!entity.IsAIFrozen() && !entity.IsDead)
            {
                UpdateAI(entity);
            }
            UpdateLogic(entity);
        }
        protected virtual void UpdateLogic(Entity entity)
        {
            UpdateTakenGrids(entity);
            entity.UpdateFragment();
        }
        protected virtual void UpdateAI(Entity entity)
        {
        }
        public override void PostRemove(Entity entity)
        {
            base.PostRemove(entity);
            entity.ClearTakenGrids();
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
                entity.PostFragmentDeath(damageInfo);

                entity.PlaySound(entity.GetDeathSound());
                entity.Remove();
            }
            if (!damageInfo.Effects.HasEffect(VanillaDamageEffects.SELF_DAMAGE))
            {
                entity.Level.Triggers.RunCallbackFiltered(VanillaLevelCallbacks.POST_CONTRAPTION_DESTROY, entity.GetDefinitionID(), entity);
            }
        }
        public override void PostTakeDamage(DamageOutput result)
        {
            base.PostTakeDamage(result);
            var bodyResult = result.BodyResult;
            if (bodyResult != null)
            {
                bodyResult.Entity.AddFragmentTickDamage(bodyResult.Amount);
            }
        }
        public virtual bool CanEvoke(Entity entity)
        {
            return !entity.IsEvoked() && !entity.IsAIFrozen();
        }
        public virtual void Evoke(Entity entity)
        {
            entity.Level.Spawn(VanillaEffectID.evocationStar, entity.GetCenter(), entity);
            OnEvoke(entity);
        }
        public virtual bool CanTrigger(Entity entity)
        {
            return entity.IsTriggerActive() && !entity.IsAIFrozen();
        }
        public virtual void Trigger(Entity entity)
        {
            OnTrigger(entity);
        }
        protected virtual void OnTrigger(Entity entity)
        {

        }
        protected virtual void OnEvoke(Entity entity)
        {

        }

    }
}