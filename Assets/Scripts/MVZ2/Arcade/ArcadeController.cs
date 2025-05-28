using System;
using System.Collections.Generic;
using System.Linq;
using MukioI18n;
using MVZ2.GameContent.Difficulties;
using MVZ2.Logic.Level;
using MVZ2.Managers;
using MVZ2.Scenes;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Saves;
using MVZ2.Vanilla.Stats;
using MVZ2Logic.Level;
using PVZEngine;
using UnityEngine;

namespace MVZ2.Arcade
{
    public class ArcadeController : MainScenePage
    {
        public override void Display()
        {
            base.Display();
            ui.DisplayPage(ArcadeUI.ArcadePage.Index);
            ui.SetAllInteractable(true);
            UpdateItems();
            if (!Main.MusicManager.IsPlaying(VanillaMusicID.choosing))
                Main.MusicManager.Play(VanillaMusicID.choosing);
        }
        public void DisplayMinigames()
        {
            ui.DisplayPage(ArcadeUI.ArcadePage.Minigame);
        }
        public void DisplayPuzzles()
        {
            ui.DisplayPage(ArcadeUI.ArcadePage.Puzzle);
        }
        private void Awake()
        {
            ui.OnIndexReturnClick += OnIndexReturnClickCallback;
            ui.OnPageReturnClick += OnPageReturnClickCallback;

            ui.OnIndexButtonClick += OnIndexButtonClickCallback;
            ui.OnItemClick += OnItemClickCallback;
        }
        #region �¼��ص�
        private void OnIndexReturnClickCallback()
        {
            Hide();
            OnReturnClick?.Invoke();
        }
        private void OnPageReturnClickCallback(ArcadeUI.ArcadePage page)
        {
            ui.DisplayPage(ArcadeUI.ArcadePage.Index);
        }
        private void OnIndexButtonClickCallback(IndexArcadePage.ButtonType button)
        {
            switch (button)
            {
                case IndexArcadePage.ButtonType.Minigame:
                    DisplayMinigames();
                    break;
                case IndexArcadePage.ButtonType.Puzzle:
                    DisplayPuzzles();
                    break;
            }
            Main.SoundManager.Play2D(VanillaSoundID.tap);
        }
        private async void OnItemClickCallback(ArcadeUI.ArcadePage page, int index)
        {
            var items = page == ArcadeUI.ArcadePage.Puzzle ? puzzleItems : minigameItems;
            Main.SoundManager.Play2D(VanillaSoundID.tap);

            var arcadeID = items[index];
            var arcadeMeta = Main.ResourceManager.GetArcadeMeta(arcadeID);
            if (arcadeMeta == null)
                return;
            var areaID = arcadeMeta.AreaID;
            var stageID = arcadeMeta.StageID;
            if (!NamespaceID.IsValid(areaID) || !NamespaceID.IsValid(stageID))
                return;
            ui.SetAllInteractable(false);
            Main.SaveManager.SaveModDatas();
            await Main.LevelManager.GotoLevelSceneAsync();
            var exitTarget = page == ArcadeUI.ArcadePage.Puzzle ? LevelExitTarget.Puzzle : LevelExitTarget.Minigame;
            Main.LevelManager.InitLevel(areaID, stageID, exitTarget: exitTarget);
            Hide();
        }
        #endregion

        private void GetOrderedArcade(IEnumerable<NamespaceID> arcades, List<NamespaceID> appendList)
        {
            var idList = GetIDListByArcadeOrder(arcades, ArcadeTypes.MINIGAME);
            appendList.AddRange(idList);
        }
        private void GetOrderedPuzzles(IEnumerable<NamespaceID> arcades, List<NamespaceID> appendList)
        {
            var idList = GetIDListByArcadeOrder(arcades, ArcadeTypes.PUZZLE);
            appendList.AddRange(idList);
        }
        private NamespaceID[] GetIDListByArcadeOrder(IEnumerable<NamespaceID> idList, string type)
        {
            if (idList == null || idList.Count() == 0)
                return Array.Empty<NamespaceID>();
            var arcades = idList
                .Select(id => (id, meta: Main.ResourceManager.GetArcadeMeta(id)))
                .Where(tuple => tuple.meta.Type == type);
            if (arcades.Count() <= 0)
                return Array.Empty<NamespaceID>();
            var arcadeIndexes = arcades.Select(tuple => (tuple.id, index: tuple.meta.Index));
            var maxAlmanacIndex = arcadeIndexes.Max(tuple => tuple.index);
            var ordered = new NamespaceID[maxAlmanacIndex + 1];
            for (int i = 0; i < ordered.Length; i++)
            {
                var tuple = arcadeIndexes.FirstOrDefault(tuple => tuple.index == i);
                ordered[i] = tuple.id;
            }
            return ordered;
        }
        private ArcadeItemViewData GetArcadeItemViewData(NamespaceID id)
        {
            var meta = Main.ResourceManager.GetArcadeMeta(id);
            var stageID = meta?.StageID;
            var stageMeta = Main.ResourceManager.GetStageMeta(stageID);
            if (stageMeta == null)
            {
                return ArcadeItemViewData.Empty;
            }
            bool unlocked = Main.SaveManager.IsAllInvalidOrUnlocked(stageMeta.Unlocks);
            string name;
            if (unlocked)
            {
                name = GetTranslatedStringParticular(VanillaStrings.CONTEXT_LEVEL_NAME, stageMeta.Name);
            }
            else
            {
                name = GetTranslatedString(LEVEL_NAME_NOT_UNLOCKED);
            }
            var hint = string.Empty;
            if (unlocked && stageMeta.Type == StageTypes.TYPE_PUZZLE_ENDLESS)
            {
                var flags = Main.SaveManager.GetSaveStat(VanillaStats.CATEGORY_MAX_ENDLESS_FLAGS, stageID);
                hint = GetTranslatedString(ENDLESS_MAX_STREAKS, flags);
            }
            var icon = Main.GetFinalSprite(meta.Icon);

            Sprite clearSprite = null;
            if (Main.SaveManager.IsLevelCleared(stageID))
            {
                var difficulty = Main.SaveManager.GetLevelDifficulty(stageID);
                var difficultyMeta = Main.ResourceManager.GetDifficultyMeta(difficulty);
                if (difficultyMeta == null)
                {
                    difficultyMeta = Main.ResourceManager.GetDifficultyMeta(VanillaDifficulties.normal);
                }
                if (difficultyMeta != null)
                {
                    var clearSpriteID = difficultyMeta.ArcadeIcon;
                    clearSprite = Main.GetFinalSprite(clearSpriteID);
                }
            }
            return new ArcadeItemViewData()
            {
                name = name,
                hint = hint,
                sprite = icon,
                clearSprite = clearSprite,
                unlocked = unlocked,
            };
        }
        private void UpdateItems()
        {
            minigameItems.Clear();
            puzzleItems.Clear();
            var arcades = Main.ResourceManager.GetAllArcadeItems().Where(id =>
            {
                var meta = Main.ResourceManager.GetArcadeMeta(id);
                if (meta == null)
                    return false;
                var stage = Main.ResourceManager.GetStageMeta(meta.StageID);
                if (stage == null)
                    return false;
                return Main.SaveManager.IsAllInvalidOrUnlocked(meta.HiddenUntil);
            });
            GetOrderedArcade(arcades, minigameItems);
            GetOrderedPuzzles(arcades, puzzleItems);

            var minigameViewDatas = minigameItems.Select(c => GetArcadeItemViewData(c)).ToArray();
            ui.SetMinigameItems(minigameViewDatas);

            var puzzleViewDatas = puzzleItems.Select(c => GetArcadeItemViewData(c)).ToArray();
            ui.SetPuzzleItems(puzzleViewDatas);
        }
        private string GetTranslatedString(string text, params object[] args)
        {
            return Main.LanguageManager._(text, args);
        }
        private string GetTranslatedStringParticular(string context, string text, params object[] args)
        {
            return Main.LanguageManager._p(context, text, args);
        }
        public event Action OnReturnClick;

        [TranslateMsg("未解锁的小游戏关卡名")]
        public const string LEVEL_NAME_NOT_UNLOCKED = "未解锁";
        [TranslateMsg("无尽模式的小游戏的连胜显示，{0}为最高连胜")]
        public const string ENDLESS_MAX_STREAKS = "最高连胜：\n{0}";
        private MainManager Main => MainManager.Instance;
        private List<NamespaceID> minigameItems = new List<NamespaceID>();
        private List<NamespaceID> puzzleItems = new List<NamespaceID>();

        [SerializeField]
        private Camera almanacCamera;
        [SerializeField]
        private ArcadeUI ui;
    }
}
