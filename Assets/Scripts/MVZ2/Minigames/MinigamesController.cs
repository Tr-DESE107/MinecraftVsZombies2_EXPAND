using System;
using System.Collections.Generic;
using System.Linq;
using MVZ2.Almanacs;
using MVZ2.GameContent.Contraptions;
using MVZ2.Level;
using MVZ2.Logic.Level;
using MVZ2.Managers;
using MVZ2.Metas;
using MVZ2.Scenes;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Almanacs;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Saves;
using PVZEngine;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace MVZ2.Minigames
{
    public class MinigamesController : MainScenePage
    {
        public override void Display()
        {
            base.Display();
            ui.DisplayPage(MinigamesUI.MinigamePage.Index);
            ui.SetAllInteractable(true);
            UpdateItems();
            if (!Main.MusicManager.IsPlaying(VanillaMusicID.choosing))
                Main.MusicManager.Play(VanillaMusicID.choosing);
        }
        public void DisplayMinigames()
        {
            ui.DisplayPage(MinigamesUI.MinigamePage.Minigame);
        }
        public void DisplayPuzzles()
        {
            ui.DisplayPage(MinigamesUI.MinigamePage.Puzzle);
        }
        private void Awake()
        {
            ui.OnIndexReturnClick += OnIndexReturnClickCallback;
            ui.OnPageReturnClick += OnPageReturnClickCallback;

            ui.OnIndexButtonClick += OnIndexButtonClickCallback;
            ui.OnItemClick += OnItemClickCallback;
        }
        #region 事件回调
        private void OnIndexReturnClickCallback()
        {
            Hide();
            OnReturnClick?.Invoke();
        }
        private void OnPageReturnClickCallback(MinigamesUI.MinigamePage page)
        {
            ui.DisplayPage(MinigamesUI.MinigamePage.Index);
        }
        private void OnIndexButtonClickCallback(IndexMinigamePage.ButtonType button)
        {
            switch (button)
            {
                case IndexMinigamePage.ButtonType.Minigame:
                    DisplayMinigames();
                    break;
                case IndexMinigamePage.ButtonType.Puzzle:
                    DisplayPuzzles();
                    break;
            }
            Main.SoundManager.Play2D(VanillaSoundID.tap);
        }
        private async void OnItemClickCallback(MinigamesUI.MinigamePage page, int index)
        {
            var items = page == MinigamesUI.MinigamePage.Puzzle ? puzzleItems : minigameItems;
            Main.SoundManager.Play2D(VanillaSoundID.tap);

            var minigameID = items[index];
            var minigameMeta = Main.ResourceManager.GetMinigameMeta(minigameID);
            if (minigameMeta == null)
                return;
            var areaID = minigameMeta.AreaID;
            var stageID = minigameMeta.StageID;
            if (!NamespaceID.IsValid(areaID) || !NamespaceID.IsValid(stageID))
                return;
            ui.SetAllInteractable(false);
            Main.SaveManager.SaveModDatas();
            await Main.LevelManager.GotoLevelSceneAsync();
            var exitTarget = page == MinigamesUI.MinigamePage.Puzzle ? LevelExitTarget.Puzzle : LevelExitTarget.Minigame;
            Main.LevelManager.InitLevel(areaID, stageID, exitTarget: exitTarget);
            Hide();
        }
        #endregion

        private void GetOrderedMinigames(IEnumerable<NamespaceID> minigames, List<NamespaceID> appendList)
        {
            var idList = GetIDListByMinigameOrder(minigames, MinigameTypes.MINIGAME);
            appendList.AddRange(idList);
        }
        private void GetOrderedPuzzles(IEnumerable<NamespaceID> minigames, List<NamespaceID> appendList)
        {
            var idList = GetIDListByMinigameOrder(minigames, MinigameTypes.PUZZLE);
            appendList.AddRange(idList);
        }
        private NamespaceID[] GetIDListByMinigameOrder(IEnumerable<NamespaceID> idList, string type)
        {
            if (idList == null || idList.Count() == 0)
                return Array.Empty<NamespaceID>();
            var minigames = idList
                .Select(id => (id, meta: Main.ResourceManager.GetMinigameMeta(id)))
                .Where(tuple => tuple.meta.Type == type);
            if (minigames.Count() <= 0)
                return Array.Empty<NamespaceID>();
            var minigameIndexes = minigames.Select(tuple => (tuple.id, index: tuple.meta.Index));
            var maxAlmanacIndex = minigameIndexes.Max(tuple => tuple.index);
            var ordered = new NamespaceID[maxAlmanacIndex + 1];
            for (int i = 0; i < ordered.Length; i++)
            {
                var tuple = minigameIndexes.FirstOrDefault(tuple => tuple.index == i);
                ordered[i] = tuple.id;
            }
            return ordered;
        }
        private MinigameItemViewData GetMinigameItemViewData(NamespaceID id)
        {
            var meta = Main.ResourceManager.GetMinigameMeta(id);
            var stageID = meta.StageID;
            var stageMeta = Main.ResourceManager.GetStageMeta(stageID);
            if (Main.SaveManager.IsValidAndLocked(stageMeta.Unlock) || stageMeta == null)
            {
                return MinigameItemViewData.Empty;
            }
            var name = GetTranslatedString(VanillaStrings.CONTEXT_LEVEL_NAME, stageMeta.Name);
            var icon = Main.GetFinalSprite(meta.Icon);

            var difficulty = Main.SaveManager.GetLevelDifficulty(stageID);
            var difficultyMeta = Main.ResourceManager.GetDifficultyMeta(difficulty);
            Sprite clearSprite = null;
            if (difficultyMeta != null)
            {
                var clearSpriteID = difficultyMeta.MinigameIcon;
                clearSprite = Main.GetFinalSprite(clearSpriteID);
            }
            return new MinigameItemViewData()
            {
                name = name,
                sprite = icon,
                clearSprite = clearSprite
            };
        }
        private void UpdateItems()
        {
            minigameItems.Clear();
            puzzleItems.Clear();
            var minigames = Main.ResourceManager.GetAllMinigames().Where(id =>
            {
                var meta = Main.ResourceManager.GetMinigameMeta(id);
                if (meta == null)
                    return false;
                var stage = Main.ResourceManager.GetStageMeta(meta.StageID);
                if (stage == null)
                    return false;
                return Main.SaveManager.IsInvalidOrUnlocked(stage.Unlock);
            });
            GetOrderedMinigames(minigames, minigameItems);
            GetOrderedPuzzles(minigames, puzzleItems);

            var minigameViewDatas = minigameItems.Select(c => GetMinigameItemViewData(c)).ToArray();
            ui.SetMinigameItems(minigameViewDatas);

            var puzzleViewDatas = puzzleItems.Select(c => GetMinigameItemViewData(c)).ToArray();
            ui.SetPuzzleItems(puzzleViewDatas);
        }
        private string GetTranslatedString(string context, string text, params object[] args)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;
            return Main.LanguageManager._p(context, text, args);
        }
        public event Action OnReturnClick;

        private MainManager Main => MainManager.Instance;
        private List<NamespaceID> minigameItems = new List<NamespaceID>();
        private List<NamespaceID> puzzleItems = new List<NamespaceID>();

        [SerializeField]
        private Camera almanacCamera;
        [SerializeField]
        private MinigamesUI ui;
    }
}
