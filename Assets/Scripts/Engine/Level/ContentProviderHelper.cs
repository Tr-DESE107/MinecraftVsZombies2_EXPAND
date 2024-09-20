using System.Linq;
using PVZEngine.Base;
using PVZEngine.Definitions;

namespace PVZEngine.Level
{
    public static class ContentProviderHelper
    {
        public static T GetDefinitionByType<T>(this IContentProvider provider) where T : Definition
        {
            return provider.GetDefinitions<T>().FirstOrDefault();
        }
        public static T GetBuffDefinition<T>(this IContentProvider provider) where T : BuffDefinition
        {
            return provider.GetDefinitionByType<T>();
        }
        public static T GetArmorDefinition<T>(this IContentProvider provider) where T : ArmorDefinition
        {
            return provider.GetDefinitionByType<T>();
        }
        public static T GetEntityDefinition<T>(this IContentProvider provider) where T : EntityDefinition
        {
            return provider.GetDefinitionByType<T>();
        }
        public static EntityDefinition GetEntityDefinition(this IContentProvider provider, NamespaceID defRef)
        {
            return provider.GetDefinition<EntityDefinition>(defRef);
        }
        public static SeedDefinition GetSeedDefinition(this IContentProvider provider, NamespaceID defRef)
        {
            return provider.GetDefinition<SeedDefinition>(defRef);
        }
        public static RechargeDefinition GetRechargeDefinition(this IContentProvider provider, NamespaceID defRef)
        {
            return provider.GetDefinition<RechargeDefinition>(defRef);
        }
        public static ShellDefinition GetShellDefinition(this IContentProvider provider, NamespaceID defRef)
        {
            return provider.GetDefinition<ShellDefinition>(defRef);
        }
        public static AreaDefinition GetAreaDefinition(this IContentProvider provider, NamespaceID defRef)
        {
            return provider.GetDefinition<AreaDefinition>(defRef);
        }
        public static StageDefinition GetStageDefinition(this IContentProvider provider, NamespaceID defRef)
        {
            return provider.GetDefinition<StageDefinition>(defRef);
        }
        public static GridDefinition GetGridDefinition(this IContentProvider provider, NamespaceID defRef)
        {
            return provider.GetDefinition<GridDefinition>(defRef);
        }
        public static BuffDefinition GetBuffDefinition(this IContentProvider provider, NamespaceID defRef)
        {
            return provider.GetDefinition<BuffDefinition>(defRef);
        }
        public static ArmorDefinition GetArmorDefinition(this IContentProvider provider, NamespaceID defRef)
        {
            return provider.GetDefinition<ArmorDefinition>(defRef);
        }
        public static SpawnDefinition GetSpawnDefinition(this IContentProvider provider, NamespaceID defRef)
        {
            return provider.GetDefinition<SpawnDefinition>(defRef);
        }
    }
}
