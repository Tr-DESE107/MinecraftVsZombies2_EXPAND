using MVZ2.GameContent.Effects;
using MVZ2.Vanilla.Contraptions;
using PVZEngine.Damages;
using PVZEngine.Entities;

namespace MVZ2.Vanilla.Entities
{
    public abstract class VanillaContraption : VanillaEntity, IEvokableContraption
    {
        public VanillaContraption(string nsp, string name) : base(nsp, name)
        {
            SetProperty(VanillaContraptionProps.FRAGMENT_ID, GetID());
        }
        public override void Init(Entity entity)
        {
            base.Init(entity);
            entity.SetFaction(entity.Level.Option.LeftFaction);

            entity.InitFragment();
        }
        public override sealed void Update(Entity entity)
        {
            base.Update(entity);
            if (!entity.IsAIFrozen())
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
        public override void PostDeath(Entity entity, DamageInfo damageInfo)
        {
            base.PostDeath(entity, damageInfo);
            entity.PostFragmentDeath(damageInfo);

            entity.PlaySound(entity.GetDeathSound());
            entity.Remove();
        }
        public override void PostTakeDamage(DamageResult bodyResult, DamageResult armorResult)
        {
            base.PostTakeDamage(bodyResult, armorResult);
            if (bodyResult != null)
            {
                bodyResult.Entity.AddFragmentTickDamage(bodyResult.Amount);
            }
        }
        public virtual bool CanEvoke(Entity entity)
        {
            return !entity.IsEvoked();
        }
        public virtual void Evoke(Entity entity)
        {
            var bounds = entity.GetBounds();
            var pos = bounds.center;
            pos.z = entity.Position.z;
            entity.Level.Spawn(VanillaEffectID.evocationStar, pos, entity);
            OnEvoke(entity);
        }
        protected virtual void OnEvoke(Entity entity)
        {

        }
        public override int Type => EntityTypes.PLANT;

    }
}