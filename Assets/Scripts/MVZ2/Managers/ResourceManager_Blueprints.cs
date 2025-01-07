using System.Linq;
using MVZ2.Level.UI;
using MVZ2.Metas;
using MVZ2.UI;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Contraptions;
using MVZ2.Vanilla.SeedPacks;
using MVZ2Logic.Games;
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
        #region 选项
        public BlueprintMetaList GetBlueprintMetaList(string spaceName)
        {
            var modResource = GetModResource(spaceName);
            if (modResource == null)
                return null;
            return modResource.BlueprintMetaList;
        }
        public BlueprintOptionMeta[] GetModBlueprintOptionMetas(string spaceName)
        {
            var metalist = GetBlueprintMetaList(spaceName);
            if (metalist == null)
                return null;
            return metalist.Options.ToArray();
        }
        public BlueprintOptionMeta GetBlueprintOptionMeta(NamespaceID id)
        {
            if (id == null)
                return null;
            var metalist = GetBlueprintMetaList(id.spacename);
            if (metalist == null)
                return null;
            return metalist.Options.FirstOrDefault(m => m.ID == id.path);
        }
        public string GetSeedOptionName(NamespaceID id)
        {
            if (id == null)
                return "null";
            var meta = GetEntityMeta(id);
            if (meta == null)
                return id.ToString();
            var name = meta.Name ?? VanillaStrings.UNKNOWN_OPTION_NAME;
            return Main.LanguageManager._p(VanillaStrings.CONTEXT_OPTION_NAME, name);
        }
        #endregion
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
            if (seedDef != null)
            {
                var seedType = seedDef.GetSeedType();
                if (seedType == SeedTypes.ENTITY)
                {
                    var entityID = seedDef.GetSeedEntityID();
                    return GetSprite(entityID.spacename, $"mobile_blueprint/{entityID.path}");
                }
                else if (seedType == SeedTypes.OPTION)
                {
                    var optionID = seedDef.GetSeedOptionID();
                    return GetSprite(optionID.spacename, $"mobile_blueprint/{optionID.path}");
                }
            }
            return GetDefaultSprite();
        }
        public Sprite GetBlueprintIconStandalone(SeedDefinition seedDef)
        {
            if (seedDef != null)
            {
                var seedType = seedDef.GetSeedType();
                if (seedType == SeedTypes.ENTITY)
                {
                    var entityID = seedDef.GetSeedEntityID();
                    if (!NamespaceID.IsValid(entityID))
                        return GetDefaultSprite();
                    var entityDef = Main.Game.GetEntityDefinition(entityID);
                    if (entityDef == null)
                        return GetDefaultSprite();
                    var modelID = entityDef.GetModelID();
                    return GetModelIcon(modelID);
                }
                else if (seedType == SeedTypes.OPTION)
                {
                    var optionID = seedDef.GetSeedOptionID();
                    if (!NamespaceID.IsValid(optionID))
                        return GetDefaultSprite();
                    var optionDef = Main.Game.GetSeedOptionDefinition(optionID);
                    if (optionDef == null)
                        return GetDefaultSprite();
                    var iconID = optionDef.GetIcon();
                    return GetSprite(iconID);
                }
            }
            return GetDefaultSprite();
        }
        public Sprite GetBlueprintIcon(SeedDefinition seedDef)
        {
            return Main.IsMobile() ? GetBlueprintIconMobile(seedDef) : GetBlueprintIconStandalone(seedDef);
        }
    }
}
