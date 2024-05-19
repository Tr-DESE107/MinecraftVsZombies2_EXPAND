using PVZEngine;

namespace MVZ2.Vanilla
{
    public abstract class VanillaContraption : EntityDefinition, IEvokableContraption
    {
        public VanillaContraption()
        {
            SetProperty(EntityProperties.ATTACK_SPEED, 1f);
            SetProperty(EntityProperties.DAMAGE, 100f);
            SetProperty(EntityProperties.MAX_HEALTH, 300f);
            SetProperty(EntityProperties.FRICTION, 0.2f);
        }
        public override void Init(Entity entity)
        {
            base.Init(entity);
            entity.SetFaction(entity.Game.Option.LeftFaction);
        }
        public override void PostDeath(Entity entity, DamageEffectList effects, EntityReference source)
        {
            base.PostDeath(entity, effects, source);
            entity.Remove();
        }
        public virtual void Evoke(Contraption contraption)
        {
        }
        public override int Type => EntityTypes.CONTRAPTION;

    }
    public interface IEvokableContraption
    {
        void Evoke(Contraption contraption);
    }
}