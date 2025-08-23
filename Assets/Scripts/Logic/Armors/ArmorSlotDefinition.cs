using PVZEngine.Base;

namespace MVZ2Logic.Armors
{
    public class ArmorSlotDefinition : Definition
    {
        public ArmorSlotDefinition(string nsp, string name) : base(nsp, name)
        {
        }
        public string Anchor { get; set; }
        public override string GetDefinitionType() => LogicDefinitionTypes.ARMOR_SLOT;
    }
}
