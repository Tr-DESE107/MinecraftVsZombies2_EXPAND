using PVZEngine.Entities;

namespace PVZEngine.Damages
{
    public abstract class DamageResult
    {
        public EntityReferenceChain Source { get; set; }
        public DamageEffectList Effects { get; set; }
        public float OriginalAmount { get; set; }
        public float Amount { get; set; }
        public float SpendAmount { get; set; }
        public bool Fatal { get; set; }
        public ShellDefinition ShellDefinition { get; set; }
        public Entity Entity { get; set; }
    }
}
