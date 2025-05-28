﻿using System.Linq;
using PVZEngine.Armors;
using PVZEngine.Base;
using PVZEngine.Buffs;
using PVZEngine.Damages;
using PVZEngine.Definitions;
using PVZEngine.Entities;
using PVZEngine.Grids;
using PVZEngine.Placements;

namespace PVZEngine
{
    public static class ContentProviderHelper
    {
        public static T GetDefinitionByType<T>(this IGameContent provider, string type) where T : Definition
        {
            return provider.GetDefinitions<T>(type).FirstOrDefault();
        }
        public static T GetBuffDefinition<T>(this IGameContent provider) where T : BuffDefinition
        {
            return provider.GetDefinitionByType<T>(EngineDefinitionTypes.BUFF);
        }
        public static T GetArmorDefinition<T>(this IGameContent provider) where T : ArmorDefinition
        {
            return provider.GetDefinitionByType<T>(EngineDefinitionTypes.ARMOR);
        }
        public static EntityDefinition GetEntityDefinition(this IGameContent provider, NamespaceID defRef)
        {
            return provider.GetDefinition<EntityDefinition>(EngineDefinitionTypes.ENTITY, defRef);
        }
        public static EntityBehaviourDefinition GetEntityBehaviourDefinition(this IGameContent provider, NamespaceID defRef)
        {
            return provider.GetDefinition<EntityBehaviourDefinition>(EngineDefinitionTypes.ENTITY_BEHAVIOUR, defRef);
        }
        public static SeedDefinition GetSeedDefinition(this IGameContent provider, NamespaceID defRef)
        {
            return provider.GetDefinition<SeedDefinition>(EngineDefinitionTypes.SEED, defRef);
        }
        public static RechargeDefinition GetRechargeDefinition(this IGameContent provider, NamespaceID defRef)
        {
            return provider.GetDefinition<RechargeDefinition>(EngineDefinitionTypes.RECHARGE, defRef);
        }
        public static ShellDefinition GetShellDefinition(this IGameContent provider, NamespaceID defRef)
        {
            return provider.GetDefinition<ShellDefinition>(EngineDefinitionTypes.SHELL, defRef);
        }
        public static PlacementDefinition GetPlacementDefinition(this IGameContent provider, NamespaceID defRef)
        {
            return provider.GetDefinition<PlacementDefinition>(EngineDefinitionTypes.PLACEMENT, defRef);
        }
        public static AreaDefinition GetAreaDefinition(this IGameContent provider, NamespaceID defRef)
        {
            return provider.GetDefinition<AreaDefinition>(EngineDefinitionTypes.AREA, defRef);
        }
        public static StageDefinition GetStageDefinition(this IGameContent provider, NamespaceID defRef)
        {
            return provider.GetDefinition<StageDefinition>(EngineDefinitionTypes.STAGE, defRef);
        }
        public static GridDefinition GetGridDefinition(this IGameContent provider, NamespaceID defRef)
        {
            return provider.GetDefinition<GridDefinition>(EngineDefinitionTypes.GRID, defRef);
        }
        public static BuffDefinition GetBuffDefinition(this IGameContent provider, NamespaceID defRef)
        {
            return provider.GetDefinition<BuffDefinition>(EngineDefinitionTypes.BUFF, defRef);
        }
        public static ArmorDefinition GetArmorDefinition(this IGameContent provider, NamespaceID defRef)
        {
            return provider.GetDefinition<ArmorDefinition>(EngineDefinitionTypes.ARMOR, defRef);
        }
        public static ArmorBehaviourDefinition GetArmorBehaviourDefinition(this IGameContent provider, NamespaceID defRef)
        {
            return provider.GetDefinition<ArmorBehaviourDefinition>(EngineDefinitionTypes.ARMOR_BEHAVIOUR, defRef);
        }
        public static SpawnDefinition GetSpawnDefinition(this IGameContent provider, NamespaceID defRef)
        {
            return provider.GetDefinition<SpawnDefinition>(EngineDefinitionTypes.SPAWN, defRef);
        }
    }
}
