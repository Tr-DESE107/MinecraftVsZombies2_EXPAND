using System;

namespace PVZEngine
{
    public class DamageInfo
    {
        public float OriginalDamage { get; private set; }
        public float Amount { get; private set; }
        public DamageEffects Effects { get; private set; }
        public Entity Entity { get; set; }
        public EntityReference Source { get; set; }
        private float usedDamage;

        public DamageInfo(float amount, DamageEffects effects, Entity entity, EntityReference source)
        {
            OriginalDamage = amount;
            Amount = amount;
            Effects = effects;
            Entity = entity;
            Source = source;
            usedDamage = Entity.GetHealth();
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
        public float GetUsedDamage()
        {
            return Math.Min(OriginalDamage, usedDamage);
        }
    }
}
