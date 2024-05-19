using System.Collections.Generic;
using System.Linq;

namespace PVZEngine
{
    public partial class Game
    {
        public EntityDefinition GetEntityDefinition(NamespaceID entityRef)
        {
            return entityDefinitions.FirstOrDefault(d => d.GetReference() == entityRef);
        }
        public T GetEntityDefinition<T>() where T : EntityDefinition
        {
            return entityDefinitions.OfType<T>().FirstOrDefault();
        }
        public ShellDefinition GetShellDefinition(NamespaceID entityRef)
        {
            return shellDefinitions.FirstOrDefault(d => d.GetReference() == entityRef);
        }
        public AreaDefinition GetAreaDefinition(NamespaceID entityRef)
        {
            return areaDefinitions.FirstOrDefault(d => d.GetReference() == entityRef);
        }
        public StageDefinition GetStageDefinition(NamespaceID entityRef)
        {
            return stageDefinitions.FirstOrDefault(d => d.GetReference() == entityRef);
        }
        public GridDefinition GetGridDefinition(NamespaceID entityRef)
        {
            return gridDefinitions.FirstOrDefault(d => d.GetReference() == entityRef);
        }
        private void AddMod(Mod mod)
        {
            entityDefinitions.AddRange(mod.GetEntityDefinitions());
            shellDefinitions.AddRange(mod.GetShellDefinitions());
            areaDefinitions.AddRange(mod.GetAreaDefinitions());
            stageDefinitions.AddRange(mod.GetStageDefinitions());
            gridDefinitions.AddRange(mod.GetGridDefinitions());
        }
        private List<EntityDefinition> entityDefinitions = new List<EntityDefinition>();
        private List<ShellDefinition> shellDefinitions = new List<ShellDefinition>();
        private List<AreaDefinition> areaDefinitions = new List<AreaDefinition>();
        private List<StageDefinition> stageDefinitions = new List<StageDefinition>();
        private List<GridDefinition> gridDefinitions = new List<GridDefinition>();
    }
}