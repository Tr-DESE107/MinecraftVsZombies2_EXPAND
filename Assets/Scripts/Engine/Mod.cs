using System.Collections.Generic;
using System.Linq;

namespace PVZEngine
{
    public class Mod
    {
        public Mod(string nsp)
        {
            Namespace = nsp;
        }
        public EntityDefinition[] GetEntityDefinitions()
        {
            return entityDefinitions.Values.ToArray();
        }
        public SeedDefinition[] GetSeedDefinitions()
        {
            return seedDefinitions.Values.ToArray();
        }
        public RechargeDefinition[] GetRechargeDefinitions()
        {
            return rechargeDefinitions.Values.ToArray();
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
        public BuffDefinition[] GetBuffDefinitions()
        {
            return buffDefinitions.Values.ToArray();
        }
        protected void AddDefinition<T>(Dictionary<string, T> list, string name, T definition) where T : Definition
        {
            definition.Namespace = Namespace;
            definition.Name = name;
            list.Add(name, definition);
        }
        protected void AddDefinitionByObject(object obj, string name)
        {
            switch (obj)
            {
                case EntityDefinition entityDef:
                    AddDefinition(entityDefinitions, name, entityDef);
                    break;
                case SeedDefinition seedDef:
                    AddDefinition(seedDefinitions, name, seedDef);
                    break;
                case RechargeDefinition rechargeDef:
                    AddDefinition(rechargeDefinitions, name, rechargeDef);
                    break;
                case ShellDefinition shellDef:
                    AddDefinition(shellDefinitions, name, shellDef);
                    break;
                case AreaDefinition areaDef:
                    AddDefinition(areaDefinitions, name, areaDef);
                    break;
                case StageDefinition stageDef:
                    AddDefinition(stageDefinitions, name, stageDef);
                    break;
                case GridDefinition gridDef:
                    AddDefinition(gridDefinitions, name, gridDef);
                    break;
                case BuffDefinition buffDef:
                    AddDefinition(buffDefinitions, name, buffDef);
                    break;
            }
        }
        public string Namespace { get; }
        protected Dictionary<string, EntityDefinition> entityDefinitions = new Dictionary<string, EntityDefinition>();
        protected Dictionary<string, SeedDefinition> seedDefinitions = new Dictionary<string, SeedDefinition>();
        protected Dictionary<string, RechargeDefinition> rechargeDefinitions = new Dictionary<string, RechargeDefinition>();
        protected Dictionary<string, ShellDefinition> shellDefinitions = new Dictionary<string, ShellDefinition>();
        protected Dictionary<string, AreaDefinition> areaDefinitions = new Dictionary<string, AreaDefinition>();
        protected Dictionary<string, StageDefinition> stageDefinitions = new Dictionary<string, StageDefinition>();
        protected Dictionary<string, GridDefinition> gridDefinitions = new Dictionary<string, GridDefinition>();
        protected Dictionary<string, BuffDefinition> buffDefinitions = new Dictionary<string, BuffDefinition>();
    }
}
