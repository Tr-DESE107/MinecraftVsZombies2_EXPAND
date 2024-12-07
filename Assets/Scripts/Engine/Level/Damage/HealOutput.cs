using PVZEngine.Armors;
using PVZEngine.Entities;

namespace PVZEngine.Damages
{
    public class HealOutput
    {
        public EntityReferenceChain Source { get; set; }
        public Entity Entity { get; set; }
        public float OriginalAmount { get; set; }
        public float Amount { get; set; }
        public float RealAmount { get; set; }
        public Armor Armor { get; set; }
        public bool ToArmor { get; set; }
        public ShellDefinition ShellDefinition { get; set; }
    }
}
