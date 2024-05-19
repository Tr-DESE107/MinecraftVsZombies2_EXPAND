using System.Collections.Generic;
using System.Linq;

namespace PVZEngine
{
    public class Mod
    {
        public EntityDefinition[] GetEntityDefinitions()
        {
            return entityDefinitions.Values.ToArray();
        }
        public ShellDefinition[] GetShellDefinitions()
        {
            return shellDefinitions.Values.ToArray();
        }
        public AreaDefinition[] GetAreaDefinitions()
        {
            return areaDefinitions.Values.ToArray();
        }
        public StageDefinition[] GetStageDefinitions()
        {
            return stageDefinitions.Values.ToArray();
        }
        public GridDefinition[] GetGridDefinitions()
        {
            return gridDefinitions.Values.ToArray();
        }
        public Mod(string nsp)
        {
            Namespace = nsp;
        }
        public string Namespace { get; }
        protected Dictionary<string, EntityDefinition> entityDefinitions = new Dictionary<string, EntityDefinition>();
        protected Dictionary<string, ShellDefinition> shellDefinitions = new Dictionary<string, ShellDefinition>();
        protected Dictionary<string, AreaDefinition> areaDefinitions = new Dictionary<string, AreaDefinition>();
        protected Dictionary<string, StageDefinition> stageDefinitions = new Dictionary<string, StageDefinition>();
        protected Dictionary<string, GridDefinition> gridDefinitions = new Dictionary<string, GridDefinition>();
    }
}
