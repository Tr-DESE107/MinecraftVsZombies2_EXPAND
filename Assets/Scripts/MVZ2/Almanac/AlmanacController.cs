using System;
using System.Collections.Generic;
using System.Linq;
using MukioI18n;
using MVZ2.Managers;
using MVZ2.Metas;
using MVZ2.Models;
using MVZ2.Scenes;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Almanacs;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Entities;
using MVZ2Logic;
using PVZEngine;
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
            Main.MusicManager.Play(VanillaMusicID.choosing);
        }
        private void Awake()
        {
            ui.OnIndexReturnClick += OnIndexReturnClickCallback;
            ui.OnPageReturnClick += OnPageReturnClickCallback;

            ui.OnIndexButtonClick += OnIndexButtonClickCallback;
            ui.OnContraptionEntryClick += OnContraptionEntryClickCallback;
            ui.OnEnemyEntryClick += OnEnemyEntryClickCallback;
            ui.OnCharacterEntryClick += OnCharacterEntryClickCallback;
            ui.OnMiscGroupEntryClick += OnMiscGroupEntryClickCallback;
        }
        private void OnIndexReturnClickCallback()
        {
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
                case IndexAlmanacPage.ButtonType.ViewCharacter:
                    ViewCharacter();
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
        private void OnCharacterEntryClickCallback(int index)
        {
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
            SetActiveContraptionEntry(contraptionEntries.FirstOrDefault());
        }
        private void ViewEnemies()
        {
            ui.DisplayPage(AlmanacUI.AlmanacPage.Enemies);
            SetActiveEnemyEntry(enemyEntries.FirstOrDefault());
        }
        private void ViewCharacter()
        {
        }
        private void ViewMisc()
        {
            ui.DisplayPage(AlmanacUI.AlmanacPage.Miscs);
            SetActiveMiscEntry(miscGroups.FirstOrDefault()?.entries?.FirstOrDefault());
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
            ui.SetActiveEnemyEntry(model, name, description);
        }
        private void SetActiveMiscEntry(AlmanacMetaEntry entry)
        {
            if (entry == null)
                return;
            var name = GetTranslatedString(VanillaStrings.GetAlmanacNameContext(VanillaAlmanacCategories.MISC), entry.name);
            var header = GetTranslatedString(VanillaStrings.GetAlmanacDescriptionContext(VanillaAlmanacCategories.MISC), entry.header);
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
            Sprite sprite = Main.ResourceManager.GetSprite(spriteID);
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
            if (definition == null)
                return;
            name = Main.ResourceManager.GetEntityName(entityID);
            var almanacMeta = Main.ResourceManager.GetAlmanacMetaEntry(almanacCategory, entityID);
            if (almanacMeta == null)
            {
                description = string.Empty;
            }
            else
            {
                var context = VanillaStrings.GetAlmanacDescriptionContext(almanacCategory);
                var header = GetTranslatedString(context, almanacMeta.header);
                var properties = GetTranslatedString(context, almanacMeta.properties);
                var flavor = GetTranslatedString(context, almanacMeta.flavor);
                var strings = new string[] { header, properties, flavor }.Where(s => !string.IsNullOrEmpty(s));
                description = string.Join("\n\n", strings);
            }


            var modelID = definition.GetModelID();
            var modelMeta = Main.ResourceManager.GetModelMeta(modelID);
            if (modelMeta != null)
            {
                model = Main.ResourceManager.GetModel(modelMeta.Path);
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

            miscGroups.Clear();
            Main.AlmanacManager.GetUnlockedMiscGroups(miscGroups);


            var contraptionViewDatas = contraptionEntries.Select(c => Main.AlmanacManager.GetChoosingBlueprintViewData(c)).ToArray();
            ui.SetContraptionEntries(contraptionViewDatas, false);

            var enemyViewDatas = enemyEntries.Select(c => Main.AlmanacManager.GetEnemyViewData(c)).ToArray();
            ui.SetEnemyEntries(enemyViewDatas);

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
        private MainManager Main => MainManager.Instance;
        private List<NamespaceID> contraptionEntries = new List<NamespaceID>();
        private List<NamespaceID> enemyEntries = new List<NamespaceID>();
        private List<NamespaceID> characterEntries = new List<NamespaceID>();
        private List<AlmanacEntryGroup> miscGroups = new List<AlmanacEntryGroup>();
        private MiscType miscType;
        [SerializeField]
        private AlmanacUI ui;

        public enum MiscType
        {
            Enemy,
            Character,
            Misc
        }
    }
}
