using PVZEngine.Entities;

namespace PVZEngine.Damages
{
    public class DamageInput
    {
        public float OriginalAmount { get; private set; }
        public float Amount { get; private set; }
        public DamageEffectList Effects { get; private set; }
        public Entity Entity { get; private set; }
        public NamespaceID ShieldTarget { get; private set; }
        public EntityReferenceChain Source { get; set; }

        public DamageInput(float amount, DamageEffectList effects, Entity entity, EntityReferenceChain source, NamespaceID shieldTarget = null)
        {
            OriginalAmount = amount;
            Amount = amount;
            Effects = effects;
            Entity = entity;
            Source = source;
            ShieldTarget = shieldTarget;
        }
        public void Add(float value)
        {
            Amount += value;
        }
        public void SetAmount(float value)
        {
            Amount = value;
        }
        public void Multiply(float value)
        {
            Amount *= value;
        }
        public bool HasEffect(NamespaceID effect)
        {
            return Effects?.HasEffect(effect) ?? false;
        }
    }
}
