using System.Linq;

namespace PVZEngine
{
    public class DamageResult
    {
        public EntityReference Source { get; set; }
        public Entity Entity { get; set; }
        public float OriginalDamage { get; set; }
        public float UsedDamage { get; set; }
        public DamageEffects Effects { get; set; }
        public bool OnArmor { get; set; }
    }
    public class DamageEffects
    {
        public DamageEffects(params string[] effects)
        {
            this.effects = effects;
        }
        public bool HasEffect(string effect)
        {
            return effects.Contains(effect);
        }
        private string[] effects;
    }
}
