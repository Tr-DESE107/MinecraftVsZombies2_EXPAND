using PVZEngine.Base;
using PVZEngine.Definitions;

namespace PVZEngine.Armors
{
    public class ArmorDefinition : Definition
    {
        public ArmorDefinition(string nsp, string name) : base(nsp, name)
        {
        }
        public virtual void PostUpdate(Armor armor) { }
        public NamespaceID GetModelID()
        {
            return GetID().ToModelID(EngineModelID.TYPE_ARMOR);
        }
        public sealed override string GetDefinitionType() => EngineDefinitionTypes.ARMOR;
    }
}
