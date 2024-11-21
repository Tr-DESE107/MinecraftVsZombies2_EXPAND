using MVZ2.UI;
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
                    triggerCost = "0",
                };
            }
            var sprite = GetBlueprintIcon(seedDef);
            return new BlueprintViewData()
            {
                icon = sprite,
                cost = seedDef.GetCost().ToString(),
                triggerActive = seedDef.IsTriggerActive(),
                triggerCost = seedDef.GetTriggerCost().ToString(),
            };
        }
        public Sprite GetBlueprintIconMobile(SeedDefinition seedDef)
        {
            if (seedDef == null)
                return GetDefaultSprite();
            if (seedDef.GetSeedType() == SeedTypes.ENTITY)
            {
                var entityID = seedDef.GetSeedEntityID();
                return Main.ResourceManager.GetSprite(entityID.spacename, $"mobile_blueprint/{entityID.path}");
            }
            return GetDefaultSprite();
        }
        public Sprite GetBlueprintIconStandalone(SeedDefinition seedDef)
        {
            if (seedDef == null)
                return GetDefaultSprite();
            if (seedDef.GetSeedType() == SeedTypes.ENTITY)
            {
                var entityID = seedDef.GetSeedEntityID();
                var modelID = entityID.ToModelID(EngineModelID.TYPE_ENTITY);
                return Main.ResourceManager.GetModelIcon(modelID);
            }
            return GetDefaultSprite();
        }
        public Sprite GetBlueprintIcon(SeedDefinition seedDef)
        {
            return Main.IsMobile() ? GetBlueprintIconMobile(seedDef) : GetBlueprintIconStandalone(seedDef);
        }
    }
}
