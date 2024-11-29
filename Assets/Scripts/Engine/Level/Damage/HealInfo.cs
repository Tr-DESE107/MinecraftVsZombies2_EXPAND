using System;
using PVZEngine.Armors;
using PVZEngine.Entities;

namespace PVZEngine.Damages
{
    public class HealInfo
    {
        public float OriginalAmount { get; private set; }
        public float Amount { get; private set; }
        public Entity Entity { get; private set; }
        public Armor Armor { get; private set; }
        public EntityReferenceChain Source { get; set; }
        public bool Canceled { get; private set; }
        public bool ToArmor { get; private set; }

        public HealInfo(float amount, Entity entity, EntityReferenceChain source)
        {
            OriginalAmount = amount;
            Amount = amount;
            Entity = entity;
            Source = source;
            ToArmor = false;
        }
        public HealInfo(float amount, Entity entity, Armor armor, EntityReferenceChain source)
        {
            OriginalAmount = amount;
            Amount = amount;
            Entity = entity;
            Source = source;
            Armor = armor;
            ToArmor = true;
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
