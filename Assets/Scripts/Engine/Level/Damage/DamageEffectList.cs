using System.Linq;
using PVZEngine.Armors;
using PVZEngine.Entities;

namespace PVZEngine.Damages
{
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
