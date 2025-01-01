using System;
using System.Collections.Generic;
using System.Linq;
using MukioI18n;
using MVZ2.GameContent.Placements;
using MVZ2.Managers;
using MVZ2.Metas;
using MVZ2.Models;
using MVZ2.Scenes;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Almanacs;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Contraptions;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Stats;
using MVZ2Logic.Artifacts;
using MVZ2Logic.Callbacks;
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
            ui.DisplayPage(AlmanacUI.AlmanacPage.Index);
            UpdateEntries();
            if (!Main.MusicManager.IsPlaying(VanillaMusicID.choosing))
                Main.MusicManager.Play(VanillaMusicID.choosing);
        }
        public void OpenEnemyAlmanac(NamespaceID id)
        {
            ui.DisplayPage(AlmanacUI.AlmanacPage.Enemies);
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
            ui.OnEnemyEntryClick += OnEnemyEntryClickCallback;
            ui.OnArtifactEntryClick += OnArtifactEntryClickCallback;
            ui.OnMiscGroupEntryClick += OnMiscGroupEntryClickCallback;
        }
        private void OnIndexReturnClickCallback()
        {
            Hide();
            OnReturnClick?.Invoke();
        }
        private void OnPageReturnClickCallback()
        {
            ui.DisplayPage(AlmanacUI.AlmanacPage.Index);
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
        private void ViewContraptions()
        {
            var page = Main.IsMobile() ? AlmanacUI.AlmanacPage.ContraptionsMobile : AlmanacUI.AlmanacPage.ContraptionsStandalone;
            ui.DisplayPage(page);
            SetActiveContraptionEntry(contraptionEntries.FirstOrDefault(e => e != null));
        }
        private void ViewEnemies()
        {
            ui.DisplayPage(AlmanacUI.AlmanacPage.Enemies);
            SetActiveEnemyEntry(enemyEntries.FirstOrDefault(e => e != null));
        }
        private void ViewArtifacts()
        {
            ui.DisplayPage(AlmanacUI.AlmanacPage.Artifacts);
            SetActiveArtifactEntry(artifactEntries.FirstOrDefault(e => e != null));
        }
        private void ViewMisc()
        {
            ui.DisplayPage(AlmanacUI.AlmanacPage.Miscs);
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
            ui.SetActiveContraptionEntry(model, name, description, costText, rechargeText);
        }
        private void SetActiveEnemyEntry(NamespaceID enemyID)
        {
            if (!NamespaceID.IsValid(enemyID))
                return;
            GetEntityAlmanacInfos(enemyID, VanillaAlmanacCategories.ENEMIES, out var model, out var name, out var description);
            if (Main.SaveManager.GetSaveStat(VanillaStats.CATEGORY_ENEMY_NEUTRALIZE, enemyID) <= 0)
            {
                name = Main.LanguageManager._p(VanillaStrings.CONTEXT_ENTITY_NAME, VanillaStrings.UNKNOWN_ENTITY_NAME);
                description = Main.LanguageManager._p(VanillaStrings.CONTEXT_ALMANAC, VanillaStrings.NOT_ENCOUNTERED_YET);
            }
            ui.SetActiveEnemyEntry(model, name, description);
        }
        private void SetActiveArtifactEntry(NamespaceID artifactID)
        {
            if (!NamespaceID.IsValid(artifactID))
                return;
            GetArtifactAlmanacInfos(artifactID, VanillaAlmanacCategories.ARTIFACTS, out var sprite, out var name, out var description);
            ui.SetActiveArtifactEntry(sprite, name, description);
        }
        private void SetActiveMiscEntry(AlmanacMetaEntry entry)
        {
            if (entry == null)
                return;
            var name = GetTranslatedString(VanillaStrings.GetAlmanacNameContext(VanillaAlmanacCategories.MISC), entry.name);
            var header = GetTranslatedString(VanillaStrings.GetAlmanacDescriptionContext(VanillaAlmanacCategories.MISC), entry.header);
            header = $"<color=#00007F>{header}</color>";
            var properties = GetTranslatedString(VanillaStrings.GetAlmanacDescriptionContext(VanillaAlmanacCategories.MISC), entry.properties);
            var flavor = GetTranslatedString(VanillaStrings.GetAlmanacDescriptionContext(VanillaAlmanacCategories.MISC), entry.flavor);
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
                    ui.SetActiveMiscEntry(model, name, description);
                    return;
                }
            }
            Sprite sprite = Main.GetFinalSprite(spriteID);
            ui.SetActiveMiscEntry(sprite, name, description);
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
                var flavor = GetTranslatedString(context, almanacMeta.flavor);
                var strings = new string[] { header, properties, flavor }.Where(s => !string.IsNullOrEmpty(s));
                return string.Join("\n\n", strings);
            }
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


            var contraptionViewDatas = contraptionEntries.Select(c => Main.AlmanacManager.GetChoosingBlueprintViewData(c)).ToArray();
            ui.SetContraptionEntries(contraptionViewDatas, false);

            var enemyViewDatas = enemyEntries.Select(c => Main.AlmanacManager.GetEnemyEntryViewData(c)).ToArray();
            ui.SetEnemyEntries(enemyViewDatas);

            var artifactViewDatas = artifactEntries.Select(c => Main.AlmanacManager.GetArtifactEntryViewData(c)).ToArray();
            ui.SetArtifactEntries(artifactViewDatas);
            ui.SetIndexArtifactVisible(artifactEntries.Count > 0);

            var miscViewDatas = miscGroups.Select(c => Main.AlmanacManager.GetMiscGroupViewData(c)).ToArray();
            ui.SetMiscGroups(miscViewDatas);
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


        private MainManager Main => MainManager.Instance;
        private List<NamespaceID> contraptionEntries = new List<NamespaceID>();
        private List<NamespaceID> enemyEntries = new List<NamespaceID>();
        private List<NamespaceID> artifactEntries = new List<NamespaceID>();
        private List<AlmanacEntryGroup> miscGroups = new List<AlmanacEntryGroup>();
        [SerializeField]
        private AlmanacUI ui;
    }
}
