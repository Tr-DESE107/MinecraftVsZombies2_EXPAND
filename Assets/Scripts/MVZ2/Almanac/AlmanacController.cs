using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using MukioI18n;
using MVZ2.GameContent.Contraptions;
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
using UnityEngine.EventSystems;

namespace MVZ2.Almanacs
{
    public class AlmanacController : MainScenePage
    {
        public override void Display()
        {
            base.Display();
            ui.DisplayPage(AlmanacPageType.Index);
            UpdateEntries();
            if (!Main.MusicManager.IsPlaying(VanillaMusicID.choosing))
                Main.MusicManager.Play(VanillaMusicID.choosing);
        }
        public void OpenEnemyAlmanac(NamespaceID id)
        {
            ui.DisplayPage(AlmanacPageType.Enemies);
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
            ui.OnReturnClick += OnReturnClickCallback;

            ui.OnIndexButtonClick += OnIndexButtonClickCallback;

            ui.OnCommandBlockClick += OnCommandBlockClickCallback;

            ui.OnContraptionEntryClick += OnContraptionEntryClickCallback;
            ui.OnMiscEntryClick += OnMiscEntryClickCallback;
            ui.OnGroupEntryClick += OnGroupEntryClickCallback;
            ui.OnZoomClick += OnZoomClickCallback;

            ui.OnDescriptionIconEnter += OnDescriptionIconEnterCallback;
            ui.OnDescriptionIconExit += OnDescriptionIconExitCallback;
            ui.OnDescriptionIconDown += OnDescriptionIconDownCallback;
            ui.OnTagIconEnter += OnTagIconEnterCallback;
            ui.OnTagIconExit += OnTagIconExitCallback;
            ui.OnTagIconDown += OnTagIconDownCallback;

            ui.OnZoomReturnClick += OnZoomReturnClickCallback;
            ui.OnZoomScaleValueChanged += OnZoomScaleValueChangedCallback;
        }
        private void OnReturnClickCallback(bool page)
        {
            UnlockAndHideTooltip();
            if (page)
            {
                ui.DisplayPage(AlmanacPageType.Index);
            }
            else
            {
                Hide();
                OnReturnClick?.Invoke();
            }
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
        private void OnCommandBlockClickCallback(PointerEventData eventData)
        {
            if (eventData.IsMouseButNotLeft())
                return;
            SetActiveContraptionEntry(VanillaContraptionID.commandBlock);
            Main.SoundManager.Play2D(VanillaSoundID.tap);
        }
        private void OnContraptionEntryClickCallback(int index, PointerEventData data)
        {
            if (data.IsMouseButNotLeft())
                return;
            SetActiveContraptionEntry(contraptionEntries[index]);
            Main.SoundManager.Play2D(VanillaSoundID.tap);
        }
        private void OnMiscEntryClickCallback(AlmanacPageType page, int index)
        {
            switch (page)
            {
                case AlmanacPageType.Enemies:
                    SetActiveEnemyEntry(enemyEntries[index]);
                    break;
                case AlmanacPageType.Artifacts:
                    SetActiveArtifactEntry(artifactEntries[index]);
                    break;
            }
            Main.SoundManager.Play2D(VanillaSoundID.tap);
        }
        private void OnGroupEntryClickCallback(AlmanacPageType page, int groupIndex, int entryIndex)
        {
            switch (page)
            {
                case AlmanacPageType.Miscs:
                    SetActiveMiscEntry(miscGroups[groupIndex].entries[entryIndex]);
                    break;
            }
            Main.SoundManager.Play2D(VanillaSoundID.tap);
        }
        #region 缩放
        private void OnZoomClickCallback(AlmanacPageType page)
        {
            if (page != AlmanacPageType.Miscs)
                return;
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
        private void OnTagIconEnterCallback(AlmanacPageType page, int index)
        {
            if (tagTooltipLockedTarget == index)
                return;
            var icon = ui.GetTagIcon(page, index);
            var tagInfo = GetEntryTagIconInfo(index);
            var viewData = GetTagTooltipViewData(tagInfo.tagID, tagInfo.enumValue);
            ui.ShowTooltip(icon, viewData);
            UnlockTooltip();
        }
        private void OnTagIconExitCallback(AlmanacPageType page, int index)
        {
            if (tagTooltipLockedTarget >= 0)
                return;
            ui.HideTooltip();
        }
        private void OnTagIconDownCallback(AlmanacPageType page, int index)
        {
            if (Main.InputManager.GetActivePointerType() != PointerTypes.TOUCH)
                return;
            LockTooltipEntryTag(tagTooltipLockedTarget == index ? -1 : index);
            Main.SoundManager.Play2D(VanillaSoundID.tap);
        }
        private void OnDescriptionIconEnterCallback(AlmanacPageType page, string linkID)
        {
            if (descriptionTagTooltipLockedTarget == linkID)
                return;
            var icon = ui.GetDescriptionIcon(page, linkID);
            if (!TryParseLinkID(linkID, out var index, out var tagID, out var enumValueID))
                return;
            var viewData = GetTagTooltipViewData(tagID, enumValueID);
            ui.ShowTooltip(icon, viewData);
            UnlockTooltip();
        }
        private void OnDescriptionIconExitCallback(AlmanacPageType page, string linkID)
        {
            if (!string.IsNullOrEmpty(descriptionTagTooltipLockedTarget))
                return;
            ui.HideTooltip();
        }
        private void OnDescriptionIconDownCallback(AlmanacPageType page, string linkID)
        {
            if (Main.InputManager.GetActivePointerType() != PointerTypes.TOUCH)
                return;
            LockTooltipDescription(descriptionTagTooltipLockedTarget == linkID ? null : linkID);
            Main.SoundManager.Play2D(VanillaSoundID.tap);
        }
        #endregion

        private void ViewContraptions()
        {
            var page = Main.IsMobile() ? AlmanacPageType.ContraptionsMobile : AlmanacPageType.ContraptionsStandalone;
            ui.DisplayPage(page);
            SetActiveContraptionEntry(contraptionEntries.FirstOrDefault(e => e != null));
        }
        private void ViewEnemies()
        {
            ui.DisplayPage(AlmanacPageType.Enemies);
            SetActiveEnemyEntry(enemyEntries.FirstOrDefault(e => e != null));
        }
        private void ViewArtifacts()
        {
            ui.DisplayPage(AlmanacPageType.Artifacts);
            SetActiveArtifactEntry(artifactEntries.FirstOrDefault(e => e != null));
        }
        private void ViewMisc()
        {
            ui.DisplayPage(AlmanacPageType.Miscs);
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

            var page = Global.IsMobile() ? AlmanacPageType.ContraptionsMobile : AlmanacPageType.ContraptionsStandalone;
            UpdateEntryTags(page, VanillaAlmanacCategories.CONTRAPTIONS, contraptionID);

            var iconInfos = GetDescriptionTagIconInfos(description);
            var replacements = iconInfos.Select(i => i.replacement).ToArray();
            var iconStacks = iconInfos.Select(i => i.viewData).ToArray();
            var finalDesc = ReplaceText(description, replacements);

            ui.SetActiveContraptionEntry(model, almanacCamera, name, finalDesc, costText, rechargeText);
            ui.UpdateContraptionDescriptionIcons(iconStacks);
            UnlockAndHideTooltip();
        }
        private void SetActiveEnemyEntry(NamespaceID enemyID)
        {
            if (!NamespaceID.IsValid(enemyID))
                return;
            activeEnemyEntryID = enemyID;
            GetEntityAlmanacInfos(enemyID, VanillaAlmanacCategories.ENEMIES, out var model, out var name, out var description);

            bool notEncountered = Main.SaveManager.GetSaveStat(VanillaStats.CATEGORY_ENEMY_NEUTRALIZE, enemyID) <= 0;
            if (notEncountered)
            {
                name = Main.LanguageManager._p(VanillaStrings.CONTEXT_ENTITY_NAME, VanillaStrings.UNKNOWN_ENTITY_NAME);
                description = Main.LanguageManager._p(VanillaStrings.CONTEXT_ALMANAC, VanillaStrings.NOT_ENCOUNTERED_YET);

                ClearEntryTags(AlmanacPageType.Enemies);
            }
            else
            {
                UpdateEntryTags(AlmanacPageType.Enemies, VanillaAlmanacCategories.ENEMIES, enemyID);
            }

            var iconInfos = GetDescriptionTagIconInfos(description);
            var replacements = iconInfos.Select(i => i.replacement).ToArray();
            var iconStacks = iconInfos.Select(i => i.viewData).ToArray();
            var finalDesc = ReplaceText(description, replacements);

            ui.SetActiveEnemyEntry(model, almanacCamera, name, finalDesc);
            ui.UpdateEnemyDescriptionIcons(iconStacks);
            UnlockAndHideTooltip();
        }
        private void SetActiveArtifactEntry(NamespaceID artifactID)
        {
            if (!NamespaceID.IsValid(artifactID))
                return;
            activeArtifactEntryID = artifactID;
            GetArtifactAlmanacInfos(artifactID, VanillaAlmanacCategories.ARTIFACTS, out var sprite, out var name, out var description);


            UpdateEntryTags(AlmanacPageType.Artifacts, VanillaAlmanacCategories.ARTIFACTS, artifactID);

            var iconInfos = GetDescriptionTagIconInfos(description);
            var replacements = iconInfos.Select(i => i.replacement).ToArray();
            var iconStacks = iconInfos.Select(i => i.viewData).ToArray();
            var finalDesc = ReplaceText(description, replacements);

            ui.SetActiveArtifactEntry(sprite, name, finalDesc);
            ui.UpdateArtifactDescriptionIcons(iconStacks);
            UnlockAndHideTooltip();
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

            UpdateEntryTags(AlmanacPageType.Miscs, VanillaAlmanacCategories.MISC, miscID);

            var iconInfos = GetDescriptionTagIconInfos(description);
            var replacements = iconInfos.Select(i => i.replacement).ToArray();
            var iconStacks = iconInfos.Select(i => i.viewData).ToArray();
            var finalDesc = ReplaceText(description, replacements);

            if (NamespaceID.IsValid(modelID) && Main.ResourceManager.GetModelMeta(modelID) is ModelMeta modelMeta)
            {
                var model = Main.ResourceManager.GetModel(modelMeta.Path);
                ui.SetActiveMiscEntry(model, almanacCamera, name, finalDesc);
            }
            else
            {
                var spriteSized = entry.iconFixedSize;
                var zoom = entry.iconZoom;
                Sprite sprite = Main.GetFinalSprite(spriteID);

                ui.SetActiveMiscEntry(sprite, name, finalDesc, spriteSized, zoom);
            }
            ui.UpdateMiscDescriptionIcons(iconStacks);
            UnlockAndHideTooltip();
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

            name = Main.ResourceManager.GetEntityName(entityID);
            description = GetAlmanacDescription(entityID, almanacCategory);

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
        private string GetAlmanacDescription(NamespaceID almanacID, string almanacCategory)
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
                var flavorKeys = almanacMeta.GetValidFlavors(Main.SaveManager);
                var flavors = flavorKeys.Select(f => GetTranslatedString(context, f));
                var flavor = string.Join("\n\n", flavors);
                var strings = new string[] { header, properties, flavor }.Where(s => !string.IsNullOrEmpty(s));
                return string.Join("\n\n", strings);
            }
        }

        #region Description Tag
        private bool TryParseLinkID(string linkID, out int index, out NamespaceID tagID, out string enumValue)
        {
            index = -1;
            tagID = null;
            enumValue = null;
            var indexStart = linkID.IndexOf('[');
            var indexEnd = linkID.IndexOf(']');
            if (indexStart < 0 || indexEnd < 0)
            {
                return false;
            }
            var indexStr = linkID.Substring(indexStart + 1, indexEnd - indexStart - 1);
            if (!ParseHelper.TryParseInt(indexStr, out index))
            {
                return false;
            }

            var afterIndex = linkID.Substring(indexEnd + 1);
            var ampIndex = afterIndex.IndexOf('&');
            string tagIDStr;
            if (ampIndex < 0)
            {
                tagIDStr = afterIndex;
            }
            else
            {
                tagIDStr = afterIndex.Substring(0, ampIndex);
                enumValue = afterIndex.Substring(ampIndex + 1);
            }
            var defaultNsp = Main.BuiltinNamespace;
            return NamespaceID.TryParse(tagIDStr, defaultNsp, out tagID);
        }
        private string GetLinkIDByTag(int index, string tagID)
        {
            return $"[{index}]{tagID}";
        }
        private string GetLinkIDByEnumTag(int index, string tagID, string enumID)
        {
            return $"[{index}]{tagID}&{enumID}";
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
                string enumValue = null;
                if (match.Groups.Count > 3 && match.Groups[3].Success)
                {
                    enumValue = match.Groups[3].Value;
                }
                GetTagMetaViewProperties(tagMeta, enumValue, out var iconSpriteRef, out var backgroundSpriteRef, out var backgroundColor, out var markSpriteRef);
                if (!string.IsNullOrEmpty(enumValue))
                {
                    linkID = GetLinkIDByEnumTag(i, tagIDStr, enumValue);
                }
                else
                {
                    linkID = GetLinkIDByTag(i, tagIDStr);
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
                rep += $"<space={descriptionTagSpacing}>";
                rep += $"<sprite=\"tag_icon_placeholder\" index=0>";
                rep += $"<space={descriptionTagSpacing}>";
                rep += $"</link>";
                var replacement = new ReplacementInfo()
                {
                    startIndex = match.Index,
                    length = match.Length,
                    text = rep
                };
                var iconViewdata = new AlmanacTagIconViewData()
                {
                    background = new AlmanacTagIconLayerViewData() { sprite = backgroundSprite, tint = backgroundColor },
                    main = new AlmanacTagIconLayerViewData() { sprite = iconSprite, tint = Color.white },
                    mark = new AlmanacTagIconLayerViewData() { sprite = markSprite, tint = Color.white }
                };
                var viewData = new AlmanacDescriptionTagViewData()
                {
                    linkID = linkID,
                    icon = iconViewdata,
                    size = size,
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
            list.Sort(CompareEntryTagInfo);
            return list.Distinct().ToArray();
        }
        private int CompareEntryTagInfo(AlmanacEntryTagInfo a, AlmanacEntryTagInfo b)
        {
            var metaA = Main.ResourceManager.GetAlmanacTagMeta(a.tagID);
            var metaB = Main.ResourceManager.GetAlmanacTagMeta(b.tagID);
            var priorityA = metaA?.priority ?? 0;
            var priorityB = metaB?.priority ?? 0;
            return priorityA.CompareTo(priorityB);
        }
        private void UpdateEntryTags(AlmanacPageType page, string category, NamespaceID id)
        {
            var tags = GetEntryTags(category, id);

            currentTags.Clear();
            currentTags.AddRange(tags);

            var viewDatas = currentTags.Select(t => GetTagIconViewData(t)).ToArray();
            ui.UpdateTagIcons(page, viewDatas);
        }
        private void ClearEntryTags(AlmanacPageType page)
        {
            currentTags.Clear();
            ui.UpdateTagIcons(page, Array.Empty<AlmanacTagIconViewData>());
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
            GetTagMetaViewProperties(tagMeta, info.enumValue, out var iconSpriteRef, out var backgroundSpriteRef, out var backgroundColor, out var markSpriteRef);

            var iconSprite = Main.GetFinalSprite(iconSpriteRef);
            var backgroundSprite = Main.GetFinalSprite(backgroundSpriteRef);
            var markSprite = Main.GetFinalSprite(markSpriteRef);

            return new AlmanacTagIconViewData()
            {
                background = new AlmanacTagIconLayerViewData() { sprite = backgroundSprite, tint = backgroundColor },
                main = new AlmanacTagIconLayerViewData() { sprite = iconSprite, tint = Color.white },
                mark = new AlmanacTagIconLayerViewData() { sprite = markSprite, tint = Color.white },
            };
        }
        private void GetTagMetaViewProperties(AlmanacTagMeta tagMeta, string enumValue, out SpriteReference iconSpriteRef, out SpriteReference backgroundSpriteRef, out Color backgroundColor, out SpriteReference markSpriteRef)
        {
            var defaultNsp = Main.BuiltinNamespace;
            iconSpriteRef = tagMeta.iconSprite;
            backgroundSpriteRef = tagMeta.backgroundSprite;
            backgroundColor = tagMeta.backgroundColor;
            markSpriteRef = tagMeta.markSprite;
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
        #endregion

        private void LockTooltipEntryTag(int entryTagIndex)
        {
            tagTooltipLockedTarget = entryTagIndex;
            descriptionTagTooltipLockedTarget = null;
        }
        private void LockTooltipDescription(string descriptionTagLinkID)
        {
            tagTooltipLockedTarget = -1;
            descriptionTagTooltipLockedTarget = descriptionTagLinkID;
        }
        private void UnlockTooltip()
        {
            tagTooltipLockedTarget = -1;
            descriptionTagTooltipLockedTarget = null;
        }
        private void UnlockAndHideTooltip()
        {
            UnlockTooltip();
            ui.HideTooltip();
        }
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
        private int tagTooltipLockedTarget;
        private string descriptionTagTooltipLockedTarget;

        [SerializeField]
        private Camera almanacCamera;
        [SerializeField]
        private AlmanacUI ui;
        [SerializeField]
        private float descriptionTagSpacing = 2;
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
