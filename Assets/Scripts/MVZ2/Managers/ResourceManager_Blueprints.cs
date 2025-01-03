using MVZ2.UI;
using MVZ2.Vanilla.Contraptions;
using MVZ2.Vanilla.SeedPacks;
using MVZ2Logic.SeedPacks;
using PVZEngine;
using PVZEngine.Definitions;
using PVZEngine.Level;
using PVZEngine.SeedPacks;
using UnityEngine;

namespace MVZ2.Managers
{
    public partial class ResourceManager : MonoBehaviour
    {
        public BlueprintViewData GetBlueprintViewData(SeedPack seed)
        {
            if (seed == null)
                return BlueprintViewData.Empty;
            var viewData = GetBlueprintViewData(seed.Definition);
            viewData.cost = seed.GetCost().ToString();
            return viewData;
        }
        public BlueprintViewData GetBlueprintViewData(SeedDefinition seedDef)
        {
            if (seedDef == null)
            {
                return new BlueprintViewData()
                {
                    icon = GetDefaultSprite(),
                    cost = "0",
                    triggerActive = false,
                };
            }
            var sprite = GetBlueprintIcon(seedDef);
            return new BlueprintViewData()
            {
                icon = sprite,
                cost = seedDef.GetCost().ToString(),
                triggerActive = seedDef.IsTriggerActive(),
                preset = seedDef.IsUpgradeBlueprint() ? BlueprintPreset.Upgrade : BlueprintPreset.Normal
            };
        }
        public BlueprintViewData GetBlueprintViewData(NamespaceID seedID)
        {
            if (!NamespaceID.IsValid(seedID))
            {
                return new BlueprintViewData()
                {
                    triggerActive = false,
                    cost = "0",
                    icon = GetDefaultSprite()
                };
            }
            var definition = main.Game.GetSeedDefinition(seedID);
            return GetBlueprintViewData(definition);
        }
        public Sprite GetBlueprintIconMobile(SeedDefinition seedDef)
        {
            if (seedDef == null)
                return GetDefaultSprite();
            if (seedDef.GetSeedType() != SeedTypes.ENTITY)
                return GetDefaultSprite();
            var entityID = seedDef.GetSeedEntityID();
            return Main.ResourceManager.GetSprite(entityID.spacename, $"mobile_blueprint/{entityID.path}");
        }
        public Sprite GetBlueprintIconStandalone(SeedDefinition seedDef)
        {
            if (seedDef == null)
                return GetDefaultSprite();
            if (seedDef.GetSeedType() != SeedTypes.ENTITY)
                return GetDefaultSprite();
            var entityID = seedDef.GetSeedEntityID();
            if (!NamespaceID.IsValid(entityID))
                return GetDefaultSprite();
            var entityDef = Main.Game.GetEntityDefinition(entityID);
            if (entityDef == null)
                return GetDefaultSprite();
            var modelID = entityDef.GetModelID();
            return Main.ResourceManager.GetModelIcon(modelID);
        }
        public Sprite GetBlueprintIcon(SeedDefinition seedDef)
        {
            return Main.IsMobile() ? GetBlueprintIconMobile(seedDef) : GetBlueprintIconStandalone(seedDef);
        }
    }
}
