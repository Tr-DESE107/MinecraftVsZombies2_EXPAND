using System.Collections.Generic;
using System.Linq;

namespace PVZEngine
{
    public partial class Game
    {
        public EntityDefinition GetEntityDefinition(NamespaceID defRef)
        {
            return entityDefinitions.FirstOrDefault(d => d.GetReference() == defRef);
        }
        public T GetEntityDefinition<T>() where T : EntityDefinition
        {
            return entityDefinitions.OfType<T>().FirstOrDefault();
        }
        public SeedDefinition GetSeedDefinition(NamespaceID seedRef)
        {
            return seedDefinitions.FirstOrDefault(d => d.GetReference() == seedRef);
        }
        public RechargeDefinition GetRechargeDefinition(NamespaceID seedRef)
        {
            return rechargeDefinitions.FirstOrDefault(d => d.GetReference() == seedRef);
        }
        public ShellDefinition GetShellDefinition(NamespaceID defRef)
        {
            return shellDefinitions.FirstOrDefault(d => d.GetReference() == defRef);
        }
        public AreaDefinition GetAreaDefinition(NamespaceID defRef)
        {
            return areaDefinitions.FirstOrDefault(d => d.GetReference() == defRef);
        }
        public StageDefinition GetStageDefinition(NamespaceID defRef)
        {
            return stageDefinitions.FirstOrDefault(d => d.GetReference() == defRef);
        }
        public GridDefinition GetGridDefinition(NamespaceID defRef)
        {
            return gridDefinitions.FirstOrDefault(d => d.GetReference() == defRef);
        }
        public BuffDefinition GetBuffDefinition(NamespaceID defRef)
        {
            return buffDefinitions.FirstOrDefault(d => d.GetReference() == defRef);
        }
        public T GetBuffDefinition<T>() where T : BuffDefinition
        {
            return buffDefinitions.OfType<T>().FirstOrDefault();
        }
        private void AddMod(Mod mod)
        {
            entityDefinitions.AddRange(mod.GetEntityDefinitions());
            seedDefinitions.AddRange(mod.GetSeedDefinitions());
            rechargeDefinitions.AddRange(mod.GetRechargeDefinitions());
            shellDefinitions.AddRange(mod.GetShellDefinitions());
            areaDefinitions.AddRange(mod.GetAreaDefinitions());
            stageDefinitions.AddRange(mod.GetStageDefinitions());
            gridDefinitions.AddRange(mod.GetGridDefinitions());
            buffDefinitions.AddRange(mod.GetBuffDefinitions());
        }
        private List<EntityDefinition> entityDefinitions = new List<EntityDefinition>();
        private List<SeedDefinition> seedDefinitions = new List<SeedDefinition>();
        private List<RechargeDefinition> rechargeDefinitions = new List<RechargeDefinition>();
        private List<ShellDefinition> shellDefinitions = new List<ShellDefinition>();
        private List<AreaDefinition> areaDefinitions = new List<AreaDefinition>();
        private List<StageDefinition> stageDefinitions = new List<StageDefinition>();
        private List<GridDefinition> gridDefinitions = new List<GridDefinition>();
        private List<BuffDefinition> buffDefinitions = new List<BuffDefinition>();
    }
}