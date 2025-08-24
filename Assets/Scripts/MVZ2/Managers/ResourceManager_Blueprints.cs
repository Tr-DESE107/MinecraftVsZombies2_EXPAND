using System;
using System.Linq;
using System.Text;
using MVZ2.GameContent.Contraptions;
using MVZ2.Metas;
using MVZ2.UI;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Contraptions;
using MVZ2.Vanilla.Level;
using MVZ2.Vanilla.SeedPacks;
using MVZ2Logic;
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
            var metalist = GetBlueprintMetaList(id.SpaceName);
            if (metalist == null)
                return null;
            return metalist.Options.FirstOrDefault(m => m.ID == id.Path);
        }
        public BlueprintEntityMeta[] GetModEntityBlueprintMetas(string spaceName)
        {
            var metalist = GetBlueprintMetaList(spaceName);
            if (metalist == null)
                return null;
            return metalist.Entities.ToArray();
        }
        public BlueprintEntityMeta GetEntityBlueprintMeta(NamespaceID id)
        {
            if (id == null)
                return null;
            var metalist = GetBlueprintMetaList(id.SpaceName);
            if (metalist == null)
                return null;
            return metalist.Entities.FirstOrDefault(m => m.ID == id.Path);
        }
        public BlueprintErrorMeta[] GetModBlueprintErrorMetas(string nsp)
        {
            var modResource = main.ResourceManager.GetModResource(nsp);
            if (modResource == null)
                return Array.Empty<BlueprintErrorMeta>();
            return modResource.BlueprintMetaList.Errors;
        }
        public string GetSeedOptionName(NamespaceID id)
        {
            if (id == null)
                return "null";
            var meta = GetBlueprintOptionMeta(id);
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
            var viewData = GetBlueprintViewData(seed.Definition, seed.Level.IsEndless(), seed.IsCommandBlock());
            viewData.cost = seed.GetCost().ToString();
            return viewData;
        }
        public BlueprintViewData GetBlueprintViewData(SeedDefinition seedDef, bool isEndless, bool isCommandBlock = false)
        {
            if (seedDef == null)
            {
                return new BlueprintViewData()
                {
                    icon = GetDefaultSprite(),
                    cost = "0",
                    triggerActive = false,
                    preset = isCommandBlock ? BlueprintPreset.CommandBlock : BlueprintPreset.Normal,
                    iconGrayscale = isCommandBlock,
                };
            }
            var sprite = GetBlueprintIcon(seedDef);
            string costStr = string.Empty;

            bool commandBlock = seedDef.GetID() == VanillaContraptionID.commandBlock;
            if (!commandBlock)
            {
                var costSB = new StringBuilder();
                costSB.Append(seedDef.GetCost());
                if (seedDef.IsUpgradeBlueprint() && isEndless)
                {
                    costSB.Append("+");
                }
                costStr = costSB.ToString();
            }
            BlueprintPreset preset = BlueprintPreset.Normal;
            if (isCommandBlock || commandBlock)
            {
                preset = BlueprintPreset.CommandBlock;
            }
            else if (seedDef.IsUpgradeBlueprint())
            {
                preset = BlueprintPreset.Upgrade;
            }
            return new BlueprintViewData()
            {
                icon = sprite,
                cost = costStr,
                triggerActive = seedDef.IsTriggerActive(),
                preset = preset,
                iconGrayscale = isCommandBlock
            };
        }
        public BlueprintViewData GetBlueprintViewData(NamespaceID seedID, bool isEndless, bool isCommandBlock = false)
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
            return GetBlueprintViewData(definition, isEndless, isCommandBlock);
        }
        public Sprite GetBlueprintIconMobile(SeedDefinition seedDef)
        {
            if (seedDef != null)
            {
                var seedType = seedDef.GetSeedType();
                if (seedType == SeedTypes.ENTITY)
                {
                    var customEntityMeta = Main.ResourceManager.GetEntityBlueprintMeta(seedDef.GetID());
                    if (customEntityMeta != null && SpriteReference.IsValid(customEntityMeta.GetMobileIcon()))
                    {
                        return GetSprite(customEntityMeta.GetMobileIcon());
                    }
                    else
                    {
                        var entityID = seedDef.GetSeedEntityID();
                        return GetSprite(entityID.SpaceName, $"mobile_blueprint/{entityID.Path}");
                    }
                }
                else if (seedType == SeedTypes.OPTION)
                {
                    var optionID = seedDef.GetSeedOptionID();
                    return GetSprite(optionID.SpaceName, $"mobile_blueprint/{optionID.Path}");
                }
            }
            return GetDefaultSprite();
        }
        public Sprite GetBlueprintIconStandalone(SeedDefinition seedDef)
        {
            if (seedDef != null)
            {
                Sprite sprite = Main.GetFinalSprite(seedDef.GetIcon());
                if (!sprite)
                {
                    sprite = GetModelIcon(seedDef.GetModelID());
                }
                return sprite;
            }
            return GetDefaultSprite();
        }
        public Sprite GetBlueprintIcon(SeedDefinition seedDef)
        {
            return Main.IsMobile() ? GetBlueprintIconMobile(seedDef) : GetBlueprintIconStandalone(seedDef);
        }
    }
}
