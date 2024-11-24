using System.Linq;
using PVZEngine.Armors;
using PVZEngine.Base;
using PVZEngine.Buffs;
using PVZEngine.Damages;
using PVZEngine.Definitions;
using PVZEngine.Entities;
using PVZEngine.Grids;

namespace PVZEngine
{
    public static class ContentProviderHelper
    {
        public static T GetDefinitionByType<T>(this IGameContent provider) where T : Definition
        {
            return provider.GetDefinitions<T>().FirstOrDefault();
        }
        public static T GetBuffDefinition<T>(this IGameContent provider) where T : BuffDefinition
        {
            return provider.GetDefinitionByType<T>();
        }
        public static T GetArmorDefinition<T>(this IGameContent provider) where T : ArmorDefinition
        {
            return provider.GetDefinitionByType<T>();
        }
        public static T GetEntityDefinition<T>(this IGameContent provider) where T : EntityDefinition
        {
            return provider.GetDefinitionByType<T>();
        }
        public static EntityDefinition GetEntityDefinition(this IGameContent provider, NamespaceID defRef)
        {
            return provider.GetDefinition<EntityDefinition>(defRef);
        }
        public static SeedDefinition GetSeedDefinition(this IGameContent provider, NamespaceID defRef)
        {
            return provider.GetDefinition<SeedDefinition>(defRef);
        }
        public static RechargeDefinition GetRechargeDefinition(this IGameContent provider, NamespaceID defRef)
        {
            return provider.GetDefinition<RechargeDefinition>(defRef);
        }
        public static ShellDefinition GetShellDefinition(this IGameContent provider, NamespaceID defRef)
        {
            return provider.GetDefinition<ShellDefinition>(defRef);
        }
        public static AreaDefinition GetAreaDefinition(this IGameContent provider, NamespaceID defRef)
        {
            return provider.GetDefinition<AreaDefinition>(defRef);
        }
        public static StageDefinition GetStageDefinition(this IGameContent provider, NamespaceID defRef)
        {
            return provider.GetDefinition<StageDefinition>(defRef);
        }
        public static GridDefinition GetGridDefinition(this IGameContent provider, NamespaceID defRef)
        {
            return provider.GetDefinition<GridDefinition>(defRef);
        }
        public static BuffDefinition GetBuffDefinition(this IGameContent provider, NamespaceID defRef)
        {
            return provider.GetDefinition<BuffDefinition>(defRef);
        }
        public static ArmorDefinition GetArmorDefinition(this IGameContent provider, NamespaceID defRef)
        {
            return provider.GetDefinition<ArmorDefinition>(defRef);
        }
        public static SpawnDefinition GetSpawnDefinition(this IGameContent provider, NamespaceID defRef)
        {
            return provider.GetDefinition<SpawnDefinition>(defRef);
        }
    }
}
