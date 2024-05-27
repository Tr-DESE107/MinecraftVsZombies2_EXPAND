using System.Collections.Generic;
using System.Linq;

namespace PVZEngine
{
    public partial class Level
    {
        public EntityDefinition GetEntityDefinition(NamespaceID defRef)
        {
            return entityDefinitions.FirstOrDefault(d => d.GetID() == defRef);
        }
        public T GetEntityDefinition<T>() where T : EntityDefinition
        {
            return entityDefinitions.OfType<T>().FirstOrDefault();
        }
        public SeedDefinition GetSeedDefinition(NamespaceID seedRef)
        {
            return seedDefinitions.FirstOrDefault(d => d.GetID() == seedRef);
        }
        public RechargeDefinition GetRechargeDefinition(NamespaceID seedRef)
        {
            return rechargeDefinitions.FirstOrDefault(d => d.GetID() == seedRef);
        }
        public ShellDefinition GetShellDefinition(NamespaceID defRef)
        {
            return shellDefinitions.FirstOrDefault(d => d.GetID() == defRef);
        }
        public AreaDefinition GetAreaDefinition(NamespaceID defRef)
        {
            return areaDefinitions.FirstOrDefault(d => d.GetID() == defRef);
        }
        public StageDefinition GetStageDefinition(NamespaceID defRef)
        {
            return stageDefinitions.FirstOrDefault(d => d.GetID() == defRef);
        }
        public GridDefinition GetGridDefinition(NamespaceID defRef)
        {
            return gridDefinitions.FirstOrDefault(d => d.GetID() == defRef);
        }
        public BuffDefinition GetBuffDefinition(NamespaceID defRef)
        {
            return buffDefinitions.FirstOrDefault(d => d.GetID() == defRef);
        }
        public T GetBuffDefinition<T>() where T : BuffDefinition
        {
            return buffDefinitions.OfType<T>().FirstOrDefault();
        }
        public ArmorDefinition GetArmorDefinition(NamespaceID defRef)
        {
            return armorDefinitions.FirstOrDefault(d => d.GetID() == defRef);
        }
        public T GetArmorDefinition<T>() where T : ArmorDefinition
        {
            return armorDefinitions.OfType<T>().FirstOrDefault();
        }
        public SpawnDefinition GetSpawnDefinition(NamespaceID defRef)
        {
            return spawnDefinitions.FirstOrDefault(d => d.GetID() == defRef);
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
            armorDefinitions.AddRange(mod.GetArmorDefinitions());
            spawnDefinitions.AddRange(mod.GetSpawnDefintions());
        }
        private List<EntityDefinition> entityDefinitions = new List<EntityDefinition>();
        private List<SeedDefinition> seedDefinitions = new List<SeedDefinition>();
        private List<RechargeDefinition> rechargeDefinitions = new List<RechargeDefinition>();
        private List<ShellDefinition> shellDefinitions = new List<ShellDefinition>();
        private List<AreaDefinition> areaDefinitions = new List<AreaDefinition>();
        private List<StageDefinition> stageDefinitions = new List<StageDefinition>();
        private List<GridDefinition> gridDefinitions = new List<GridDefinition>();
        private List<BuffDefinition> buffDefinitions = new List<BuffDefinition>();
        private List<ArmorDefinition> armorDefinitions = new List<ArmorDefinition>();
        private List<SpawnDefinition> spawnDefinitions = new List<SpawnDefinition>();
    }
}