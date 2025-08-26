using System.Collections.Generic;
using System.Linq;
using MukioI18n;
using MVZ2.GameContent.Areas;
using MVZ2.GameContent.Contraptions;
using MVZ2.GameContent.Placements;
using MVZ2.Vanilla.Contraptions;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Level;
using MVZ2.Vanilla.SeedPacks;
using MVZ2Logic;
using MVZ2Logic.Callbacks;
using MVZ2Logic.Modding;
using MVZ2Logic.SeedPacks;
using PVZEngine;
using PVZEngine.Callbacks;
using PVZEngine.Definitions;
using PVZEngine.Entities;

namespace MVZ2.GameContent.GlobalCallbacks
{
    [ModGlobalCallbacks]
    public class BlueprintRecommendGlobalCallbacks : VanillaGlobalCallbacks
    {
        public override void Apply(Mod mod)
        {
            mod.AddTrigger(LogicLevelCallbacks.GET_BLUEPRINT_NOT_RECOMMONDED, GetBlueprintNotRecommondedCallback);
            mod.AddTrigger(LogicLevelCallbacks.GET_BLUEPRINT_WARNINGS, GetBlueprintWarningsCallback);
        }

        private void GetBlueprintNotRecommondedCallback(LogicLevelCallbacks.GetBlueprintNotRecommondedParams param, CallbackResult callbackResult)
        {
            var level = param.level;
            var blueprint = param.blueprintID;
            var content = level.Content;
            var blueprintDef = content.GetSeedDefinition(blueprint);
            if (blueprintDef == null)
                return;

            if (blueprintDef.GetSeedType() == SeedTypes.ENTITY)
            {
                var entityID = blueprintDef.GetSeedEntityID();
                var entityDef = content.GetEntityDefinition(entityID);
                if (entityDef == null)
                    return;
                if (level.IsDay())
                {
                    // 白天的夜间器械
                    if (entityDef.IsNocturnal())
                    {
                        callbackResult.SetFinalValue(true);
                        return;
                    }
                }
                // 白天的荧石
                if (entityID == VanillaContraptionID.glowstone)
                {
                    if (level.IsDay() && level.AreaID != VanillaAreaID.dream)
                    {
                        callbackResult.SetFinalValue(true);
                        return;
                    }
                }
                var areaTags = level.AreaDefinition.GetAreaTags();
                if (areaTags != null)
                {
                    if (areaTags.Contains(VanillaAreaTags.noWater))
                    {
                        // 无水地形的水生器械
                        if (entityDef.GetPlacementID() == VanillaPlacementID.aquatic)
                        {
                            callbackResult.SetFinalValue(true);
                            return;
                        }
                    }
                }
            }
        }
        private void GetBlueprintWarningsCallback(LogicLevelCallbacks.GetBlueprintWarningsParams param, CallbackResult callbackResult)
        {
            var level = param.level;
            var chosenBlueprints = param.chosenBlueprints;
            var blueprintsForChoose = param.blueprintsForChoose;
            var results = param.warnings;

            var content = level.Content;
            var chosenBlueprintDefs = chosenBlueprints.Select(item => level.Content.GetSeedDefinition(item.id));
            var chosenBlueprintEntityDefs = chosenBlueprintDefs.Where(def => def.GetSeedType() == SeedTypes.ENTITY)
                .Select(def => level.Content.GetEntityDefinition(def.GetSeedEntityID()))
                .Where(def => def != null);

            var entityDefsForChoose = blueprintsForChoose.Select(id =>
            {
                if (!NamespaceID.IsValid(id))
                    return null;
                var blueprintDef = level.Content.GetSeedDefinition(id);
                if (blueprintDef == null)
                    return null;
                if (blueprintDef.GetSeedType() != SeedTypes.ENTITY)
                    return null;
                return level.Content.GetEntityDefinition(blueprintDef.GetSeedEntityID());
            }).Where(def => def != null);

            // 升级
            var upgradeBlueprints = chosenBlueprintEntityDefs.Where(d => d.IsUpgradeBlueprint());
            foreach (var upgradeDef in upgradeBlueprints)
            {
                var neededBase = upgradeDef.GetUpgradeFromEntity();
                if (chosenBlueprints.Any(b => b.id == neededBase))
                    continue;
                if (!blueprintsForChoose.Contains(neededBase))
                    continue;
                var baseName = Global.Game.GetEntityName(neededBase);
                var upgradeName = Global.Game.GetEntityName(upgradeDef.GetID());
                results.Add(Global.Game.GetText(WARNING_MISSING_UPGRADE_BASE, baseName, upgradeName));
            }
            // 攻击者
            var enemyPool = level.GetEnemyPool();
            var contraptionAttackers = chosenBlueprintEntityDefs.Select(def => def.GetCounterTagsFor()).Where(t => t != null).SelectMany(t => t).Distinct();
            var missingTagsList = new List<NamespaceID>();
            foreach (var spawnID in enemyPool)
            {
                var spawnDef = level.Content.GetSpawnDefinition(spawnID);
                if (spawnDef == null)
                    continue;
                var tags = spawnDef.GetCounterTags(level);
                if (tags == null)
                    continue;
                foreach (var tag in tags)
                {
                    if (contraptionAttackers.Contains(tag))
                        continue;
                    if (missingTagsList.Contains(tag))
                        continue;
                    missingTagsList.Add(tag);
                    var counterName = Global.Game.GetEntityCounterName(tag);
                    results.Add(Global.Game.GetText(WARNING_MISSING_ATTACKER, counterName));
                }
            }
            // 生产者
            if (!chosenBlueprintEntityDefs.Any(e => e.IsProducer()) && entityDefsForChoose.Any(e => e.IsProducer()))
            {
                results.Add(Global.Game.GetText(WARNING_MISSING_PRODUCER));
            }
            // 水生器械
            var areaTags = level.AreaDefinition.GetAreaTags();
            if (areaTags != null && areaTags.Contains(VanillaAreaTags.water))
            {
                if (!chosenBlueprintEntityDefs.Any(e => e.GetPlacementID() == VanillaPlacementID.aquatic) && entityDefsForChoose.Any(e => e.GetPlacementID() == VanillaPlacementID.aquatic))
                {
                    results.Add(Global.Game.GetText(WARNING_MISSING_AQUATIC));
                }
            }

        }
        [TranslateMsg("选卡警告，{0}为原器械，{1}为升级器械")]
        public const string WARNING_MISSING_UPGRADE_BASE = "你确定想要在没有{0}的情况下使用{1}？";
        [TranslateMsg("选卡警告，{0}为目标敌人")]
        public const string WARNING_MISSING_ATTACKER = "你没有选能干掉{0}的器械，确定要继续吗？";
        [TranslateMsg("选卡警告")]
        public const string WARNING_MISSING_PRODUCER = "你没有选能生产能量的器械，确定要继续吗？";
        [TranslateMsg("选卡警告")]
        public const string WARNING_MISSING_AQUATIC = "你确定这关不使用任何水生器械吗？";
    }
}
