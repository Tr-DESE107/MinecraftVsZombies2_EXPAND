using PVZEngine.Armors;
using PVZEngine.Base;
using PVZEngine.Definitions;

namespace PVZEngine.Entities
{
    public abstract class ArmorBehaviourDefinition : Definition
    {
        protected ArmorBehaviourDefinition(string nsp, string name) : base(nsp, name)
        {
        }
        public virtual void PostUpdate(Armor armor) { }
        public sealed override string GetDefinitionType() => EngineDefinitionTypes.ARMOR_BEHAVIOUR;
    }
}
