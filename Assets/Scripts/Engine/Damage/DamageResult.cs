using System.Linq;

namespace PVZEngine
{
    public class DamageResult
    {
        public EntityReference Source { get; set; }
        public Entity Entity { get; set; }
        public float OriginalDamage { get; set; }
        public float UsedDamage { get; set; }
        public DamageEffectList Effects { get; set; }
        public bool OnArmor { get; set; }
        public bool Fatal { get; set; }
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
