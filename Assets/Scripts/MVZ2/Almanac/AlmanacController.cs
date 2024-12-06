using System;
using System.Collections.Generic;
using System.Linq;
using MukioI18n;
using MVZ2.Managers;
using MVZ2.Models;
using MVZ2.Scenes;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Almanacs;
using MVZ2.Vanilla.Audios;
using PVZEngine;
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
            ui.OnMiscEntryClick += OnMiscEntryClickCallback;
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
        private void OnMiscEntryClickCallback(int index)
        {
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
        }
        private void SetActiveContraptionEntry(NamespaceID contraptionID)
        {
            if (!NamespaceID.IsValid(contraptionID))
                return;
            GetEntityAlmanacInfos(contraptionID, VanillaAlmanacCategories.CONTRAPTIONS, out var model, out var name, out var description);
            ui.SetActiveContraptionEntry(model, name, description);
        }
        private void SetActiveEnemyEntry(NamespaceID enemyID)
        {
            if (!NamespaceID.IsValid(enemyID))
                return;
            GetEntityAlmanacInfos(enemyID, VanillaAlmanacCategories.ENEMIES, out var model, out var name, out var description);
            ui.SetActiveEnemyEntry(model, name, description);
        }
        private void GetEntityAlmanacInfos(NamespaceID entityID, string almanacCategory, out Model model, out string name, out string description)
        {
            model = null;
            name = null;
            description = null;
            if (!NamespaceID.IsValid(entityID))
                return;
            var definition = Main.Game.GetEntityDefinition(entityID);
            var modelID = definition.GetModelID();
            var modelMeta = Main.ResourceManager.GetModelMeta(modelID);
            model = Main.ResourceManager.GetModel(modelMeta.Path);
            name = Main.ResourceManager.GetEntityName(entityID);
            var almanacMeta = Main.ResourceManager.GetAlmanacMetaEntry(almanacCategory, entityID);
            if (almanacMeta == null)
            {
                description = string.Empty;
                return;
            }

            var header = Main.LanguageManager._p(VanillaStrings.CONTEXT_ENTITY_DESCRIPTION, almanacMeta.header);
            var properties = Main.LanguageManager._p(VanillaStrings.CONTEXT_ENTITY_DESCRIPTION, almanacMeta.properties);
            var flavor = Main.LanguageManager._p(VanillaStrings.CONTEXT_ENTITY_DESCRIPTION, almanacMeta.flavor);
            description = Main.LanguageManager._p(VanillaStrings.CONTEXT_ENTITY_DESCRIPTION, DESCRIPTION_TEMPLATE, header, properties, flavor);
        }
        private void UpdateEntries()
        {
            contraptionEntries.Clear();
            var blueprints = Main.SaveManager.GetUnlockedContraptions();
            Main.AlmanacManager.GetOrderedBlueprints(blueprints, contraptionEntries);

            enemyEntries.Clear();
            var enemies = Main.SaveManager.GetUnlockedEnemies();
            Main.AlmanacManager.GetOrderedEnemies(enemies, enemyEntries);


            var contraptionViewDatas = contraptionEntries.Select(c => Main.AlmanacManager.GetChoosingBlueprintViewData(c)).ToArray();
            ui.SetContraptionEntries(contraptionViewDatas, false);

            var enemyViewDatas = enemyEntries.Select(c => Main.AlmanacManager.GetEnemyViewData(c)).ToArray();
            ui.SetEnemyEntries(enemyViewDatas);
        }
        public event Action OnReturnClick;
        [TranslateMsg("图鉴描述模板，{0}为开头段，{1}为属性段，{2}为趣文段", VanillaStrings.CONTEXT_ENTITY_DESCRIPTION)]
        public const string DESCRIPTION_TEMPLATE = "{0}\n{1}\n{2}";
        private MainManager Main => MainManager.Instance;
        private List<NamespaceID> contraptionEntries = new List<NamespaceID>();
        private List<NamespaceID> enemyEntries = new List<NamespaceID>();
        private List<NamespaceID> characterEntries = new List<NamespaceID>();
        private List<NamespaceID> miscEntries = new List<NamespaceID>();
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
