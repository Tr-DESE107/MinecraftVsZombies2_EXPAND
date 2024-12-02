using System;
using PVZEngine.Entities;

namespace PVZEngine.Damages
{
    public class DamageInput
    {
        public float OriginalAmount { get; private set; }
        public float Amount { get; private set; }
        public DamageEffectList Effects { get; private set; }
        public Entity Entity { get; private set; }
        public EntityReferenceChain Source { get; set; }
        public bool Canceled { get; private set; }

        public DamageInput(float amount, DamageEffectList effects, Entity entity, EntityReferenceChain source)
        {
            OriginalAmount = amount;
            Amount = amount;
            Effects = effects;
            Entity = entity;
            Source = source;
        }
        public void Add(float value)
        {
            Amount += value;
        }
        public void Multiply(float value)
        {
            Amount *= value;
        }
        public void Cancel()
        {
            Canceled = true;
        }
    }
}
