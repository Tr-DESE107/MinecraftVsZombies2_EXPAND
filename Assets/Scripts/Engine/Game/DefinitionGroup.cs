using PVZEngine.Definitions;

namespace PVZEngine.Game
{
    public class GameDefinitionGroup : DefinitionGroup
    {
        public GameDefinitionGroup()
        {
            lists.Add(new DefinitionList<EntityDefinition>());
            lists.Add(new DefinitionList<SeedDefinition>());
            lists.Add(new DefinitionList<RechargeDefinition>());
            lists.Add(new DefinitionList<ShellDefinition>());
            lists.Add(new DefinitionList<AreaDefinition>());
            lists.Add(new DefinitionList<StageDefinition>());
            lists.Add(new DefinitionList<GridDefinition>());
            lists.Add(new DefinitionList<BuffDefinition>());
            lists.Add(new DefinitionList<ArmorDefinition>());
            lists.Add(new DefinitionList<SpawnDefinition>());

            lists.Add(new DefinitionList<TalkEndDefinition>());
        }
    }
}
