using System;
using PVZEngine.Entities;

namespace PVZEngine.Damages
{
    public class DamageInfo
    {
        public float OriginalDamage { get; private set; }
        public float Amount { get; private set; }
        public DamageEffectList Effects { get; private set; }
        public Entity Entity { get; private set; }
        public EntityReferenceChain Source { get; set; }
        public bool Canceled { get; private set; }
        private float usedDamage;

        public DamageInfo(float amount, DamageEffectList effects, Entity entity, EntityReferenceChain source)
        {
            OriginalDamage = amount;
            Amount = amount;
            Effects = effects;
            Entity = entity;
            Source = source;
            usedDamage = Entity.Health;
        }
        public void Add(float value)
        {
            Amount += value;
            usedDamage -= value;
        }
        public void Multiply(float value)
        {
            Amount *= value;
            if (value == 0)
            {
                usedDamage = OriginalDamage;
            }
            else
            {
                usedDamage /= value;
            }
        }
        public void Cancel()
        {
            Canceled = true;
        }
        public float GetUsedDamage()
        {
            return Math.Min(OriginalDamage, usedDamage);
        }
    }
}
