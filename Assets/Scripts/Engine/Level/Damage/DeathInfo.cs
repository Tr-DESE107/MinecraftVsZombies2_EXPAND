using PVZEngine.Entities;

namespace PVZEngine.Damages
{
    public class DeathInfo
    {
        public DamageEffectList Effects { get; private set; }
        public Entity Entity { get; private set; }
        public EntityReferenceChain Source { get; set; }
        public BodyDamageResult Damage { get; private set; }

        public DeathInfo(Entity entity, DamageEffectList effects, EntityReferenceChain source, BodyDamageResult damage)
        {
            Effects = effects;
            Entity = entity;
            Source = source;
            Damage = damage;
        }
        public bool HasEffect(NamespaceID effect)
        {
            if (Effects == null)
                return false;
            return Effects.HasEffect(effect);
        }
    }
}
