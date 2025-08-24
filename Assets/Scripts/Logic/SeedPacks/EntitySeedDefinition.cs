using PVZEngine.Base;

namespace MVZ2Logic.SeedPacks
{
    public class EntitySeedDefinition : Definition
    {
        public EntitySeedDefinition(string nsp, string name) : base(nsp, name)
        {
        }
        public sealed override string GetDefinitionType() => LogicDefinitionTypes.ENTITY_SEED;
        public string BlueprintName { get; set; }
        public string BlueprintTooltip { get; set; }
    }
}
