using System.Linq;
using PVZEngine.Definitions;

namespace PVZEngine.Level
{
    public class DamageResult
    {
        public EntityReferenceChain Source { get; set; }
        public Entity Entity { get; set; }
        public float OriginalDamage { get; set; }
        public float Amount { get; set; }
        public float UsedDamage { get; set; }
        public DamageEffectList Effects { get; set; }
        public Armor Armor { get; set; }
        public bool Fatal { get; set; }
        public ShellDefinition ShellDefinition { get; set; }
    }
    public class DamageEffectList
    {
        public DamageEffectList(params NamespaceID[] effects)
        {
            this.effects = effects;
        }
        public bool HasEffect(NamespaceID effect)
        {
            return effects.Contains(effect);
        }
        private NamespaceID[] effects;
    }
}
