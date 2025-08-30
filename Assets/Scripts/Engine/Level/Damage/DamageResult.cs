using PVZEngine.Entities;
using PVZEngine.Level;
using UnityEngine;

namespace PVZEngine.Damages
{
    public abstract class DamageResult
    {
        public DamageResult(DamageInput input)
        {
            OriginalAmount = input.OriginalAmount;
            Amount = input.Amount;
            Entity = input.Entity;
            Effects = input.Effects;
            Source = input.Source;
        }
        public ILevelSourceReference Source { get; set; }
        public DamageEffectList Effects { get; set; }
        public float OriginalAmount { get; set; }
        public float Amount { get; set; }
        public float SpendAmount { get; set; }
        public bool Fatal { get; set; }
        public ShellDefinition ShellDefinition { get; set; }
        public Entity Entity { get; set; }
        public abstract Vector3 GetPosition();
        public bool HasEffect(NamespaceID effect)
        {
            return Effects?.HasEffect(effect) ?? false;
        }
        public bool IsValid()
        {
            return Amount > 0;
        }
    }
}
