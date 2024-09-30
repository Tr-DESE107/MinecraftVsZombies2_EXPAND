using MVZ2.Definitions;
using PVZEngine;
using PVZEngine.Definitions;

namespace MVZ2.Games
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

            lists.Add(new DefinitionList<HeldItemDefinition>());
            lists.Add(new DefinitionList<NoteDefinition>());
        }
    }
}
