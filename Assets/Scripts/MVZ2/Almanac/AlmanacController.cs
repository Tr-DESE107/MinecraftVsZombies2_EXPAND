using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using MukioI18n;
using MVZ2.GameContent.Contraptions;
using MVZ2.GameContent.Placements;
using MVZ2.Level.UI;
using MVZ2.Managers;
using MVZ2.Metas;
using MVZ2.Models;
using MVZ2.Scenes;
using MVZ2.UI;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Almanacs;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Contraptions;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Saves;
using MVZ2.Vanilla.Stats;
using MVZ2Logic;
using MVZ2Logic.Almanacs;
using MVZ2Logic.Artifacts;
using MVZ2Logic.Callbacks;
using MVZ2Logic.Games;
using PVZEngine;
using PVZEngine.Entities;
using PVZEngine.Level;
using UnityEngine;

namespace MVZ2.Almanacs
{
    public class AlmanacController : MainScenePage
    {
        public override void Display()
        {
            base.Display();
            ui.DisplayPage(AlmanacUIPage.Index);
            UpdateEntries();
            if (!Main.MusicManager.IsPlaying(VanillaMusicID.choosing))
                Main.MusicManager.Play(VanillaMusicID.choosing);
        }
        public void OpenEnemyAlmanac(NamespaceID id)
        {
            ui.DisplayPage(AlmanacUIPage.Enemies);
            if (enemyEntries.Contains(id))
            {
                SetActiveEnemyEntry(id);
            }
            else
            {
                SetActiveEnemyEntry(enemyEntries.FirstOrDefault());
            }
        }
        private void Awake()
        {
            ui.OnIndexReturnClick += OnIndexReturnClickCallback;
            ui.OnPageReturnClick += OnPageReturnClickCallback;

            ui.OnIndexButtonClick += OnIndexButtonClickCallback;
            ui.OnContraptionEntryClick += OnContraptionEntryClickCallback;
            ui.OnCommandBlockClick += OnCommandBlockClickCallback;
            ui.OnEnemyEntryClick += OnEnemyEntryClickCallback;
            ui.OnArtifactEntryClick += OnArtifactEntryClickCallback;
            ui.OnMiscGroupEntryClick += OnMiscGroupEntryClickCallback;
            ui.OnEnemyZoomClick += OnEnemyZoomClickCallback;
            ui.OnArtifactZoomClick += OnArtifactZoomClickCallback;
            ui.OnMiscZoomClick += OnMiscZoomClickCallback;

            ui.OnDescriptionIconEnter += OnDescriptionIconEnterCallback;
            ui.OnDescriptionIconExit += OnDescriptionIconExitCallback;
            ui.OnTagIconEnter += OnTagIconEnterCallback;
            ui.OnTagIconExit += OnTagIconExitCallback;

            ui.OnZoomReturnClick += OnZoomReturnClickCallback;
            ui.OnZoomScaleValueChanged += OnZoomScaleValueChangedCallback;
        }
        private void OnIndexReturnClickCallback()
        {
            Hide();
            OnReturnClick?.Invoke();
        }
        private void OnPageReturnClickCallback()
        {
            ui.DisplayPage(AlmanacUIPage.Index);
        }
        private void OnIndexButtonClickCallback(IndexAlmanacPage.ButtonType button)
        {
            switch (button)
            {
                case IndexAlmanacPage.ButtonType.ViewContraption:
                    ViewContraptions();
                    break;
                case IndexAlmanacPage.ButtonType.ViewEnemy:
                    ViewEnemies();
                    break;
                case IndexAlmanacPage.ButtonType.ViewArtifact:
                    ViewArtifacts();
                    break;
                case IndexAlmanacPage.ButtonType.ViewMisc:
                    ViewMisc();
                    break;
            }
        }
        private void OnContraptionEntryClickCallback(int index)
        {
            SetActiveContraptionEntry(contraptionEntries[index]);
            Main.SoundManager.Play2D(VanillaSoundID.tap);
        }
        private void OnCommandBlockClickCallback()
        {
            SetActiveContraptionEntry(VanillaContraptionID.commandBlock);
            Main.SoundManager.Play2D(VanillaSoundID.tap);
        }
        private void OnEnemyEntryClickCallback(int index)
        {
            SetActiveEnemyEntry(enemyEntries[index]);
            Main.SoundManager.Play2D(VanillaSoundID.tap);
        }
        private void OnArtifactEntryClickCallback(int index)
        {
            SetActiveArtifactEntry(artifactEntries[index]);
            Main.SoundManager.Play2D(VanillaSoundID.tap);
        }
        private void OnMiscGroupEntryClickCallback(int groupIndex, int entryIndex)
        {
            SetActiveMiscEntry(miscGroups[groupIndex].entries[entryIndex]);
            Main.SoundManager.Play2D(VanillaSoundID.tap);
        }
        #region 缩放
        private void OnEnemyZoomClickCallback()
        {
        }
        private void OnArtifactZoomClickCallback()
        {
        }
        private void OnMiscZoomClickCallback()
        {
            var sprite = GetMiscEntryIconSprite(activeMiscEntryID);
            if (sprite)
            {
                ui.StartZoom(sprite);
                SetZoomScale(1);
            }
            Main.SoundManager.Play2D(VanillaSoundID.tap);
        }
        private void OnZoomReturnClickCallback()
        {
            ui.StopZoom();
        }
        private void OnZoomScaleValueChangedCallback(float value)
        {
            SetZoomScale(value);
        }
        #endregion

        #region 描述图标
        private void OnTagIconEnterCallback(AlmanacUIPage page, int index)
        {
            var icon = ui.GetTagIcon(page, index);
            var tagInfo = GetEntryTagIconInfo(index);
            var viewData = GetTagTooltipViewData(tagInfo.tagID, tagInfo.enumValue);
            ui.ShowTooltip(icon, viewData);
        }
        private void OnTagIconExitCallback(AlmanacUIPage page, int index)
        {
            ui.HideTooltip();
        }
        private void OnDescriptionIconEnterCallback(AlmanacUIPage page, string linkID)
        {
            var icon = ui.GetDescriptionIcon(page, linkID);
            if (!TryParseLinkID(linkID, out var tagID, out var enumValueID))
                return;
            var viewData = GetTagTooltipViewData(tagID, enumValueID);
            ui.ShowTooltip(icon, viewData);
        }
        private void OnDescriptionIconExitCallback(AlmanacUIPage page, string linkID)
        {
            ui.HideTooltip();
        }
        #endregion

        private void ViewContraptions()
        {
            var page = Main.IsMobile() ? AlmanacUIPage.ContraptionsMobile : AlmanacUIPage.ContraptionsStandalone;
            ui.DisplayPage(page);
            SetActiveContraptionEntry(contraptionEntries.FirstOrDefault(e => e != null));
        }
        private void ViewEnemies()
        {
            ui.DisplayPage(AlmanacUIPage.Enemies);
            SetActiveEnemyEntry(enemyEntries.FirstOrDefault(e => e != null));
        }
        private void ViewArtifacts()
        {
            ui.DisplayPage(AlmanacUIPage.Artifacts);
            SetActiveArtifactEntry(artifactEntries.FirstOrDefault(e => e != null));
        }
        private void ViewMisc()
        {
            ui.DisplayPage(AlmanacUIPage.Miscs);
            SetActiveMiscEntry(miscGroups.FirstOrDefault()?.entries?.FirstOrDefault(e => e != null));
        }
        private void SetActiveContraptionEntry(NamespaceID contraptionID)
        {
            if (!NamespaceID.IsValid(contraptionID))
                return;
            GetEntityAlmanacInfos(contraptionID, VanillaAlmanacCategories.CONTRAPTIONS, out var model, out var name, out var description);

            int cost = 0;
            string recharge = string.Empty;
            var definition = Main.Game.GetEntityDefinition(contraptionID);
            if (definition != null)
            {
                cost = definition.GetCost();
                var rechargeID = definition.GetRechargeID();
                var rechargeDefinition = Main.Game.GetRechargeDefinition(rechargeID);
                if (rechargeDefinition != null)
                {
                    recharge = GetTranslatedString(VanillaStrings.CONTEXT_RECHARGE_TIME, rechargeDefinition.GetName());
                }
            }
            var costText = GetTranslatedString(VanillaStrings.CONTEXT_ALMANAC, COST_LABEL, cost);
            var rechargeText = GetTranslatedString(VanillaStrings.CONTEXT_ALMANAC, RECHARGE_LABEL, recharge);

            var page = Global.IsMobile() ? AlmanacUIPage.ContraptionsMobile : AlmanacUIPage.ContraptionsStandalone;
            UpdateEntryTags(page, VanillaAlmanacCategories.CONTRAPTIONS, contraptionID);

            var iconInfos = GetDescriptionTagIconInfos(description);
            var replacements = iconInfos.Select(i => i.replacement).ToArray();
            var iconStacks = iconInfos.Select(i => i.viewData).ToArray();
            var finalDesc = ReplaceText(description, replacements);

            ui.SetActiveContraptionEntry(model, almanacCamera, name, finalDesc, costText, rechargeText);
            ui.UpdateContraptionDescriptionIcons(iconStacks);
        }
        private void SetActiveEnemyEntry(NamespaceID enemyID)
        {
            if (!NamespaceID.IsValid(enemyID))
                return;
            activeEnemyEntryID = enemyID;
            GetEntityAlmanacInfos(enemyID, VanillaAlmanacCategories.ENEMIES, out var model, out var name, out var description);
            if (Main.SaveManager.GetSaveStat(VanillaStats.CATEGORY_ENEMY_NEUTRALIZE, enemyID) <= 0)
            {
                name = Main.LanguageManager._p(VanillaStrings.CONTEXT_ENTITY_NAME, VanillaStrings.UNKNOWN_ENTITY_NAME);
                description = Main.LanguageManager._p(VanillaStrings.CONTEXT_ALMANAC, VanillaStrings.NOT_ENCOUNTERED_YET);
            }

            UpdateEntryTags(AlmanacUIPage.Enemies, VanillaAlmanacCategories.ENEMIES, enemyID);

            var iconInfos = GetDescriptionTagIconInfos(description);
            var replacements = iconInfos.Select(i => i.replacement).ToArray();
            var iconStacks = iconInfos.Select(i => i.viewData).ToArray();
            var finalDesc = ReplaceText(description, replacements);

            ui.SetActiveEnemyEntry(model, almanacCamera, name, description);
            ui.UpdateEnemyDescriptionIcons(iconStacks);
        }
        private void SetActiveArtifactEntry(NamespaceID artifactID)
        {
            if (!NamespaceID.IsValid(artifactID))
                return;
            activeArtifactEntryID = artifactID;
            GetArtifactAlmanacInfos(artifactID, VanillaAlmanacCategories.ARTIFACTS, out var sprite, out var name, out var description);


            UpdateEntryTags(AlmanacUIPage.Artifacts, VanillaAlmanacCategories.ARTIFACTS, artifactID);

            var iconInfos = GetDescriptionTagIconInfos(description);
            var replacements = iconInfos.Select(i => i.replacement).ToArray();
            var iconStacks = iconInfos.Select(i => i.viewData).ToArray();
            var finalDesc = ReplaceText(description, replacements);

            ui.SetActiveArtifactEntry(sprite, name, description);
            ui.UpdateEnemyDescriptionIcons(iconStacks);
        }
        private void SetActiveMiscEntry(NamespaceID miscID)
        {
            if (!NamespaceID.IsValid(miscID))
                return;
            var entry = Main.ResourceManager.GetAlmanacMetaEntry(VanillaAlmanacCategories.MISC, miscID);
            if (entry == null)
                return;
            activeMiscEntryID = miscID;
            var name = GetTranslatedString(VanillaStrings.GetAlmanacNameContext(VanillaAlmanacCategories.MISC), entry.name);

            var descContext = VanillaStrings.GetAlmanacDescriptionContext(VanillaAlmanacCategories.MISC);
            var header = GetTranslatedString(descContext, entry.header);
            header = $"<color=#00007F>{header}</color>";
            var properties = GetTranslatedString(descContext, entry.properties);

            var flavorKeys = entry.GetValidFlavors(Main.SaveManager);
            var flavors = flavorKeys.Select(f => GetTranslatedString(descContext, f));
            var flavor = string.Join("\n\n", flavors);

            var strings = new string[] { header, properties, flavor }.Where(s => !string.IsNullOrEmpty(s));
            var description = string.Join("\n\n", strings);

            var spriteID = entry.sprite;
            var modelID = entry.model;
            if (NamespaceID.IsValid(modelID))
            {
                var modelMeta = Main.ResourceManager.GetModelMeta(modelID);
                if (modelMeta != null)
                {
                    var model = Main.ResourceManager.GetModel(modelMeta.Path);
                    ui.SetActiveMiscEntry(model, almanacCamera, name, description);
                    return;
                }
            }
            var spriteSized = entry.iconFixedSize;
            var zoom = entry.iconZoom;
            Sprite sprite = Main.GetFinalSprite(spriteID);

            UpdateEntryTags(AlmanacUIPage.Miscs, VanillaAlmanacCategories.MISC, miscID);

            var iconInfos = GetDescriptionTagIconInfos(description);
            var replacements = iconInfos.Select(i => i.replacement).ToArray();
            var iconStacks = iconInfos.Select(i => i.viewData).ToArray();
            var finalDesc = ReplaceText(description, replacements);

            ui.SetActiveMiscEntry(sprite, name, finalDesc, spriteSized, zoom);
            ui.UpdateMiscDescriptionIcons(iconStacks);
        }
        private Sprite GetMiscEntryIconSprite(NamespaceID miscID)
        {
            if (!NamespaceID.IsValid(miscID))
                return null;
            var entry = Main.ResourceManager.GetAlmanacMetaEntry(VanillaAlmanacCategories.MISC, miscID);
            return GetMiscEntryIconSprite(entry);
        }
        private Sprite GetMiscEntryIconSprite(AlmanacMetaEntry entry)
        {
            if (entry == null)
                return null;
            var spriteID = entry.sprite;
            return Main.GetFinalSprite(spriteID);
        }
        private void GetEntityAlmanacInfos(NamespaceID entityID, string almanacCategory, out Model model, out string name, out string description)
        {
            model = null;
            name = null;
            description = null;
            if (!NamespaceID.IsValid(entityID))
                return;
            var definition = Main.Game.GetEntityDefinition(entityID);
            bool nocturnal = definition.IsNocturnal();
            var placement = definition.GetPlacementID();

            string extraPropertyText = string.Empty;
            if (definition.IsNocturnal())
            {
                extraPropertyText += "\n" + Main.LanguageManager._p(VanillaStrings.CONTEXT_ALMANAC, EXTRA_PROPERTY_NOCTURNAL);
            }
            if (placement == VanillaPlacementID.aquatic)
            {
                extraPropertyText += "\n" + Main.LanguageManager._p(VanillaStrings.CONTEXT_ALMANAC, EXTRA_PROPERTY_AQUATIC);
            }
            name = Main.ResourceManager.GetEntityName(entityID);
            description = GetAlmanacDescription(entityID, almanacCategory, extraPropertyText);

            if (definition == null)
                return;
            var modelID = definition.GetModelID();
            var modelMeta = Main.ResourceManager.GetModelMeta(modelID);
            if (modelMeta != null)
            {
                model = Main.ResourceManager.GetModel(modelMeta.Path);
            }
        }
        private void GetArtifactAlmanacInfos(NamespaceID entityID, string almanacCategory, out Sprite sprite, out string name, out string description)
        {
            sprite = null;
            name = null;
            description = null;
            if (!NamespaceID.IsValid(entityID))
                return;
            name = Main.ResourceManager.GetArtifactName(entityID);
            description = GetAlmanacDescription(entityID, almanacCategory);


            var definition = Main.Game.GetArtifactDefinition(entityID);
            if (definition == null)
                return;
            var spriteReference = definition.GetSpriteReference();
            sprite = Main.GetFinalSprite(spriteReference);
        }
        private string GetAlmanacDescription(NamespaceID almanacID, string almanacCategory, string extraPropertyText = null)
        {
            if (!NamespaceID.IsValid(almanacID))
                return string.Empty;
            var almanacMeta = Main.ResourceManager.GetAlmanacMetaEntry(almanacCategory, almanacID);
            if (almanacMeta == null)
            {
                return string.Empty;
            }
            else
            {
                var context = VanillaStrings.GetAlmanacDescriptionContext(almanacCategory);
                var header = GetTranslatedString(context, almanacMeta.header);
                header = $"<color=#00007F>{header}</color>";
                var properties = GetTranslatedString(context, almanacMeta.properties);
                if (!string.IsNullOrEmpty(extraPropertyText))
                {
                    properties += extraPropertyText;
                }
                var flavorKeys = almanacMeta.GetValidFlavors(Main.SaveManager);
                var flavors = flavorKeys.Select(f => GetTranslatedString(context, f));
                var flavor = string.Join("\n\n", flavors);
                var strings = new string[] { header, properties, flavor }.Where(s => !string.IsNullOrEmpty(s));
                return string.Join("\n\n", strings);
            }
        }

        #region Description Tag
        private bool TryParseLinkID(string linkID, out NamespaceID tagID, out string enumValue)
        {
            var ampIndex = linkID.IndexOf('&');
            string tagIDStr;
            enumValue = null;
            if (ampIndex < 0)
            {
                tagIDStr = linkID;
            }
            else
            {
                tagIDStr = linkID.Substring(0, ampIndex);
                enumValue = linkID.Substring(ampIndex + 1);
            }
            var defaultNsp = Main.BuiltinNamespace;
            return NamespaceID.TryParse(tagIDStr, defaultNsp, out tagID);
        }
        private string GetLinkIDByTag(string tagID)
        {
            return tagID;
        }
        private string GetLinkIDByEnumTag(string tagID, string enumID)
        {
            return $"{tagID}&{enumID}";
        }
        private TooltipViewData GetTagTooltipViewData(NamespaceID tagID, string enumValue)
        {
            AlmanacTagMeta tagMeta = Main.ResourceManager.GetAlmanacTagMeta(tagID);
            if (tagMeta == null)
                return default;
            string name = string.Empty;
            string desc = string.Empty;
            if (NamespaceID.IsValid(tagMeta.enumType))
            {
                var defaultNsp = Main.BuiltinNamespace;
                var enumMeta = Main.ResourceManager.GetAlmanacTagEnumMeta(tagMeta.enumType);
                var enumValueMeta = enumMeta?.FindValueByString(enumValue, defaultNsp);
                if (enumValueMeta != null)
                {
                    var tagName = Main.LanguageManager._p(VanillaStrings.CONTEXT_ALMANAC_TAG_NAME, tagMeta.name);
                    var enumValueName = Main.LanguageManager._p(VanillaStrings.CONTEXT_ALMANAC_TAG_ENUM_NAME, enumValueMeta.name);
                    name = Main.LanguageManager._p(VanillaStrings.CONTEXT_ALMANAC, TAG_ENUM_TEMPLATE, tagName, enumValueName);
                    desc = Main.LanguageManager._p(VanillaStrings.CONTEXT_ALMANAC_TAG_ENUM_DESCRIPTION, enumValueMeta.description);
                    return new TooltipViewData()
                    {
                        name = name,
                        description = desc,
                    };
                }
            }
            name = Main.LanguageManager._p(VanillaStrings.CONTEXT_ALMANAC_TAG_NAME, tagMeta.name);
            desc = Main.LanguageManager._p(VanillaStrings.CONTEXT_ALMANAC_TAG_DESCRIPTION, tagMeta.description);
            return new TooltipViewData()
            {
                name = name,
                description = desc,
            };
        }
        private DescriptionTagIconInfo[] GetDescriptionTagIconInfos(string text)
        {
            List<DescriptionTagIconInfo> infos = new List<DescriptionTagIconInfo>();

            string pattern = @"<tag +id ?= ?""([\w:]*)""( +enum ?= ?""([\w:]*)"")? */>";
            var matches = Regex.Matches(text, pattern);
            var defaultNsp = Main.BuiltinNamespace;
            for (int i = 0; i < matches.Count; i++)
            {
                var match = matches[i];
                string linkID;
                if (match.Groups.Count < 2)
                    continue;
                var tagIDStr = match.Groups[1].Value;
                if (!NamespaceID.TryParse(tagIDStr, defaultNsp, out var tagID))
                    continue;
                AlmanacTagMeta tagMeta = Main.ResourceManager.GetAlmanacTagMeta(tagID);
                if (tagMeta == null)
                    continue;
                SpriteReference iconSpriteRef = tagMeta.iconSprite;
                SpriteReference backgroundSpriteRef = tagMeta.backgroundSprite;
                SpriteReference markSpriteRef = tagMeta.markSprite;
                Color backgroundColor = tagMeta.backgroundColor;
                if (match.Groups.Count > 3 && match.Groups[3].Success)
                {
                    var enumValue = match.Groups[3].Value;
                    linkID = GetLinkIDByEnumTag(tagIDStr, enumValue);
                    if (NamespaceID.IsValid(tagMeta.enumType))
                    {
                        AlmanacTagEnumMeta tagEnumMeta = Main.ResourceManager.GetAlmanacTagEnumMeta(tagMeta.enumType);
                        AlmanacTagEnumValueMeta valueMeta = tagEnumMeta?.FindValueByString(enumValue, defaultNsp);
                        if (valueMeta != null)
                        {
                            iconSpriteRef = valueMeta.iconSprite;
                            backgroundColor = valueMeta.backgroundColor;
                        }
                    }
                }
                else
                {
                    linkID = GetLinkIDByTag(tagIDStr);
                }

                var iconSprite = Main.GetFinalSprite(iconSpriteRef);
                var backgroundSprite = Main.GetFinalSprite(backgroundSpriteRef);
                var markSprite = Main.GetFinalSprite(markSpriteRef);
                var size = new Vector2(32, 32);
                if (backgroundSprite)
                {
                    size = backgroundSprite.rect.size;
                }


                // 存储替换信息
                string rep = string.Empty;
                rep += $"<link=\"{linkID}\">";
                rep += $"<size={size.x}>";
                rep += $"<sprite=\"tag_icon_placeholder\" index=0>";
                rep += $"</size>";
                rep += $"</link>";
                var replacement = new ReplacementInfo()
                {
                    startIndex = match.Index,
                    length = match.Length,
                    text = rep
                };
                var iconViewdata = new AlmanacTagIconViewData()
                {
                    background = new AlmanacTagIconLayerViewData() { sprite = backgroundSprite, tint = backgroundColor, scale = Vector3.one },
                    main = new AlmanacTagIconLayerViewData() { sprite = iconSprite, tint = Color.white, scale = Vector3.one },
                    mark = new AlmanacTagIconLayerViewData() { sprite = markSprite, tint = Color.white, scale = Vector3.one },
                };
                var viewData = new AlmanacDescriptionTagViewData()
                {
                    linkID = linkID,
                    icon = iconViewdata,
                };
                infos.Add(new DescriptionTagIconInfo()
                {
                    replacement = replacement,
                    viewData = viewData
                });
            }
            return infos.ToArray();
        }
        private static string ReplaceText(string text, ReplacementInfo[] replacements)
        {
            StringBuilder sb = new StringBuilder(text);
            for (int i = replacements.Length - 1; i >= 0; i--)
            {
                var replacement = replacements[i];
                sb.Remove(replacement.startIndex, replacement.length);
                sb.Insert(replacement.startIndex, replacement.text);
            }
            return sb.ToString();
        }
        #endregion

        #region Entry Tag
        private AlmanacEntryTagInfo[] GetEntryTags(string category, NamespaceID id)
        {
            List<AlmanacEntryTagInfo> list = new List<AlmanacEntryTagInfo>();
            var param = new LogicCallbacks.GetAlmanacEntryTagsParams(category, id, list);
            Global.Game.RunCallbackFiltered(LogicCallbacks.GET_ALMANAC_ENTRY_TAGS, param, category);

            var almanacEntry = Main.ResourceManager.GetAlmanacMetaEntry(category, id);
            if (almanacEntry != null)
            {
                list.AddRange(almanacEntry.tags);
            }
            return list.Distinct().ToArray();
        }
        private void UpdateEntryTags(AlmanacUIPage page, string category, NamespaceID id)
        {
            var tags = GetEntryTags(category, id);

            currentTags.Clear();
            currentTags.AddRange(tags);

            var viewDatas = currentTags.Select(t => GetTagIconViewData(t)).ToArray();
            ui.UpdateTagIcons(page, viewDatas);
        }
        private AlmanacEntryTagInfo GetEntryTagIconInfo(int index)
        {
            return currentTags[index];
        }
        private AlmanacTagIconViewData GetTagIconViewData(AlmanacEntryTagInfo info)
        {
            var tagID = info.tagID;
            AlmanacTagMeta tagMeta = Main.ResourceManager.GetAlmanacTagMeta(tagID);
            if (tagMeta == null)
                return default;
            var enumValue = info.enumValue;
            var defaultNsp = Main.BuiltinNamespace;
            SpriteReference iconSpriteRef = tagMeta.iconSprite;
            SpriteReference backgroundSpriteRef = tagMeta.backgroundSprite;
            SpriteReference markSpriteRef = tagMeta.markSprite;
            Color backgroundColor = tagMeta.backgroundColor;
            if (NamespaceID.IsValid(tagMeta.enumType))
            {
                AlmanacTagEnumMeta tagEnumMeta = Main.ResourceManager.GetAlmanacTagEnumMeta(tagMeta.enumType);
                AlmanacTagEnumValueMeta valueMeta = tagEnumMeta?.FindValueByString(enumValue, defaultNsp);
                if (valueMeta != null)
                {
                    iconSpriteRef = valueMeta.iconSprite;
                    backgroundColor = valueMeta.backgroundColor;
                }
            }

            var iconSprite = Main.GetFinalSprite(iconSpriteRef);
            var backgroundSprite = Main.GetFinalSprite(backgroundSpriteRef);
            var markSprite = Main.GetFinalSprite(markSpriteRef);

            return new AlmanacTagIconViewData()
            {
                background = new AlmanacTagIconLayerViewData() { sprite = backgroundSprite, tint = backgroundColor, scale = Vector3.one },
                main = new AlmanacTagIconLayerViewData() { sprite = iconSprite, tint = Color.white, scale = Vector3.one },
                mark = new AlmanacTagIconLayerViewData() { sprite = markSprite, tint = Color.white, scale = Vector3.one },
            };
        }
        #endregion

        private void UpdateEntries()
        {
            contraptionEntries.Clear();
            var blueprints = Main.SaveManager.GetUnlockedContraptions();
            Main.AlmanacManager.GetOrderedBlueprints(blueprints, contraptionEntries);

            enemyEntries.Clear();
            var enemies = Main.SaveManager.GetUnlockedEnemies();
            Main.AlmanacManager.GetOrderedEnemies(enemies, enemyEntries);

            artifactEntries.Clear();
            var artifacts = Main.SaveManager.GetUnlockedArtifacts();
            Main.AlmanacManager.GetOrderedArtifacts(artifacts, artifactEntries);

            miscGroups.Clear();
            Main.AlmanacManager.GetUnlockedMiscGroups(miscGroups);


            var contraptionViewDatas = contraptionEntries.Select(c => Main.AlmanacManager.GetChoosingBlueprintViewData(c, false)).ToArray();
            var commandBlockViewData = Main.AlmanacManager.GetChoosingBlueprintViewData(VanillaContraptionID.commandBlock, false);
            ui.SetContraptionEntries(contraptionViewDatas, Main.SaveManager.IsCommandBlockUnlocked(), commandBlockViewData);

            var enemyViewDatas = enemyEntries.Select(c => Main.AlmanacManager.GetEnemyEntryViewData(c)).ToArray();
            ui.SetEnemyEntries(enemyViewDatas);

            var artifactViewDatas = artifactEntries.Select(c => Main.AlmanacManager.GetArtifactEntryViewData(c)).ToArray();
            ui.SetArtifactEntries(artifactViewDatas);
            ui.SetIndexArtifactVisible(artifactEntries.Count > 0);

            var miscViewDatas = miscGroups.Select(c => Main.AlmanacManager.GetMiscGroupViewData(c)).ToArray();
            ui.SetMiscGroups(miscViewDatas);
        }
        private void SetZoomScale(float value)
        {
            ui.SetZoomScale(value);
            var percentage = Main.GetFloatPercentageText(value);
            var text = Main.LanguageManager._p(VanillaStrings.CONTEXT_ALMANAC, OPTION_ZOOM_SCALE, percentage);
            ui.SetZoomScaleSliderText(text);
            ui.SetZoomScaleSliderValue(value);
        }
        private string GetTranslatedString(string context, string text, params object[] args)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;
            return Main.LanguageManager._p(context, text, args);
        }
        public event Action OnReturnClick;
        [TranslateMsg("图鉴描述模板，{0}为能量", VanillaStrings.CONTEXT_ALMANAC)]
        public const string COST_LABEL = "花费：<color=red>{0}</color>";
        [TranslateMsg("图鉴描述模板，{0}为冷却时间", VanillaStrings.CONTEXT_ALMANAC)]
        public const string RECHARGE_LABEL = "冷却时间：<color=red>{0}</color>";
        [TranslateMsg("图鉴描述", VanillaStrings.CONTEXT_ALMANAC)]
        public const string EXTRA_PROPERTY_NOCTURNAL = "<color=blue>白天失效</color>";
        [TranslateMsg("图鉴描述", VanillaStrings.CONTEXT_ALMANAC)]
        public const string EXTRA_PROPERTY_AQUATIC = "<color=blue>只能放在水上</color>";
        [TranslateMsg("图鉴放大选项，{0}为缩放等级", VanillaStrings.CONTEXT_ALMANAC)]
        public const string OPTION_ZOOM_SCALE = "缩放：{0}";
        [TranslateMsg("图鉴标签枚举值的名称模板，{0}为标签名，{1}为值名", VanillaStrings.CONTEXT_ALMANAC)]
        public const string TAG_ENUM_TEMPLATE = "{0}：{1}";


        private MainManager Main => MainManager.Instance;
        private List<NamespaceID> contraptionEntries = new List<NamespaceID>();
        private List<NamespaceID> enemyEntries = new List<NamespaceID>();
        private List<NamespaceID> artifactEntries = new List<NamespaceID>();
        private List<AlmanacEntryGroup> miscGroups = new List<AlmanacEntryGroup>();
        private List<AlmanacEntryTagInfo> currentTags = new List<AlmanacEntryTagInfo>();
        private NamespaceID activeEnemyEntryID;
        private NamespaceID activeArtifactEntryID;
        private NamespaceID activeMiscEntryID;

        [SerializeField]
        private Camera almanacCamera;
        [SerializeField]
        private AlmanacUI ui;
    }
    public struct ReplacementInfo
    {
        public int startIndex;
        public int length;
        public string text;
    }
    public struct DescriptionTagIconInfo
    {
        public ReplacementInfo replacement;
        public AlmanacDescriptionTagViewData viewData;
    }
}
