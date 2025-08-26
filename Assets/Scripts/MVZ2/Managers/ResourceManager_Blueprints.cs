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
        public BlueprintEntityMeta[] GetModEntityBlueprintMetas(string spaceName)
        {
            var metalist = GetBlueprintMetaList(spaceName);
            if (metalist == null)
                return null;
            return metalist.Entities.ToArray();
        }
        public BlueprintErrorMeta[] GetModBlueprintErrorMetas(string nsp)
        {
            var modResource = main.ResourceManager.GetModResource(nsp);
            if (modResource == null)
                return Array.Empty<BlueprintErrorMeta>();
            return modResource.BlueprintMetaList.Errors;
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
        public string GetBlueprintName(NamespaceID blueprintID, bool commandBlock)
        {
            string name = main.Game.GetBlueprintName(blueprintID);
            if (commandBlock)
            {
                name = Global.Game.GetTextParticular(name, VanillaStrings.COMMAND_BLOCK_BLUEPRINT_NAME_TEMPLATE);
            }
            return name;
        }
        public string GetBlueprintTooltip(NamespaceID blueprintID)
        {
            return main.Game.GetBlueprintTooltip(blueprintID);
        }
        public Sprite GetBlueprintIconMobile(SeedDefinition seedDef)
        {
            if (seedDef != null)
            {
                Sprite sprite = Main.GetFinalSprite(seedDef.GetMobileIcon());
                return sprite;
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
