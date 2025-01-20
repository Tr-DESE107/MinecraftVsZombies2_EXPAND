using PVZEngine.Armors;
using PVZEngine.Entities;
using PVZEngine.Triggers;

namespace PVZEngine.Damages
{
    public class HealInput : IInterruptSource
    {
        public float OriginalAmount { get; private set; }
        public float Amount { get; private set; }
        public Entity Entity { get; private set; }
        public Armor Armor { get; private set; }
        public EntityReferenceChain Source { get; set; }
        public bool IsInterrupted { get; private set; }
        public bool ToArmor { get; private set; }

        public HealInput(float amount, Entity entity, EntityReferenceChain source)
        {
            OriginalAmount = amount;
            Amount = amount;
            Entity = entity;
            Source = source;
            ToArmor = false;
        }
        public HealInput(float amount, Entity entity, Armor armor, EntityReferenceChain source)
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
            IsInterrupted = true;
        }
    }
}
