using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MVZ2.Almanacs;
using MVZ2.ChapterTransition;
using MVZ2.GameContent.Stages;
using MVZ2.Mainmenu;
using MVZ2.Mainmenu.UI;
using MVZ2.Managers;
using MVZ2.Map;
using MVZ2.Note;
using MVZ2.Save;
using MVZ2.Titlescreen;
using MVZ2.UI;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Saves;
using MVZ2Logic;
using MVZ2Logic.Scenes;
using PVZEngine;
using UnityEngine;

namespace MVZ2.Scenes
{
    public class MainSceneController : MonoBehaviour, ISceneController
    {
        public void GotoMapOrMainmenu()
        {
            if (main.SaveManager.IsLevelCleared(VanillaStageID.prologue))
            {
                var lastMapID = main.SaveManager.GetLastMapID() ?? main.ResourceManager.GetFirstMapID();
                DisplayMap(lastMapID);
            }
            else
            {
                DisplayPage(MainScenePageType.Mainmenu);
            }
        }
        public void Init()
        {
            achievementHint.gameObject.SetActive(true);
        }
        #region 对话框
        public void ShowDialogMessage(string title, string desc, Action onSelect = null)
        {
            ShowDialog(title, desc, new string[]
            {
                main.LanguageManager._(VanillaStrings.CONFIRM),
            }, (index) => onSelect?.Invoke());
        }
        public Task ShowDialogMessageAsync(string title, string desc)
        {
            var tcs = new TaskCompletionSource<object>();
            ShowDialogMessage(title, desc, () => tcs.SetResult(null));
            return tcs.Task;
        }
        public void ShowDialogSelect(string title, string desc, Action<bool> onSelect = null)
        {
            ShowDialog(title, desc, new string[]
            {
                main.LanguageManager._(VanillaStrings.YES),
                main.LanguageManager._(VanillaStrings.NO),
            }, (index) => onSelect?.Invoke(index == 0));
        }
        public Task<bool> ShowDialogSelectAsync(string title, string desc)
        {
            var tcs = new TaskCompletionSource<bool>();
            ShowDialogSelect(title, desc, (result) => tcs.SetResult(result));
            return tcs.Task;
        }
        public void ShowDialog(string title, string desc, string[] options, Action<int> onSelect = null)
        {
            ui.ShowDialog(title, desc, options, onSelect);
        }
        public Task<string> ShowInputNameDialogAsync(InputNameType type)
        {
            return inputNameDialog.Show(type);
        }
        public Task<string> ShowInputNameDialogRenameAsync(int index)
        {
            return inputNameDialog.ShowRename(index);
        }
        public Task<int> ShowDeleteUserDialogAsync(IEnumerable<UserDataItem> users)
        {
            return deleteUserDialog.Show(users);
        }
        #endregion
        public void ShowAchievementEarnTips(IEnumerable<NamespaceID> achievements)
        {
            achievementHint.Show(achievements);
            if (achievements.Count() > 0)
            {
                main.SoundManager.Play2D(VanillaSoundID.achievement);
            }
        }
        public void ShowPortal()
        {
            portal.Fadeout();
        }
        public void SetPortalFadeIn(Action OnFadeIn)
        {
            portal.SetDisplay(true);
            portal.OnFadeIn += OnFadeIn;
        }
        public void SetPortalFadeOut()
        {
            portal.SetDisplay(false);
            portal.ResetFadeout();
        }
        public void DisplayPage(MainScenePageType type)
        {
            foreach (var pair in pages)
            {
                if (pair.Key == type)
                    pair.Value.Display();
                else
                    pair.Value.Hide();
            }
        }
        public void HidePages()
        {
            foreach (var pair in pages)
            {
                pair.Value.Hide();
            }
        }
        public void DisplayMap(NamespaceID mapId)
        {
            DisplayPage(MainScenePageType.Map);
            map.SetMap(mapId);
        }
        public void DisplayNote(NamespaceID id, string buttonText)
        {
            DisplayPage(MainScenePageType.Note);
            note.SetNote(id);
            note.SetButtonText(buttonText);
        }
        public void DisplayAlmanac(Action onReturn)
        {
            DisplayPage(MainScenePageType.Almanac);
            almanac.OnReturnClick += OnReturn;
            void OnReturn()
            {
                onReturn?.Invoke();
                almanac.OnReturnClick -= OnReturn;
            }
        }
        public void DisplayEnemyAlmanac(NamespaceID enemyID)
        {
            almanac.OpenEnemyAlmanac(enemyID);
        }
        public Task DisplayChapterTransitionAsync(NamespaceID id)
        {
            return chapterTransition.DisplayAsync(id);
        }
        public void HideChapterTransition()
        {
            chapterTransition.Hide();
        }
        public void SetBlackScreen(float value)
        {
            ui.SetBlackScreen(value);
        }
        public void FadeBlackScreen(float target, float duration)
        {
            ui.FadeBlackScreen(target, duration);
        }
        private void Awake()
        {
            pages.Add(MainScenePageType.Landing, landing);
            pages.Add(MainScenePageType.Titlescreen, titlescreen);
            pages.Add(MainScenePageType.Mainmenu, mainmenu);
            pages.Add(MainScenePageType.Note, note);
            pages.Add(MainScenePageType.Map, map);
            pages.Add(MainScenePageType.Almanac, almanac);
        }
        private MainManager main => MainManager.Instance;
        private Dictionary<MainScenePageType, MainScenePage> pages = new Dictionary<MainScenePageType, MainScenePage>();
        [SerializeField]
        private MainSceneUI ui;
        [SerializeField]
        private LandingController landing;
        [SerializeField]
        private TitlescreenController titlescreen;
        [SerializeField]
        private MainmenuController mainmenu;
        [SerializeField]
        private NoteController note;
        [SerializeField]
        private MapController map;
        [SerializeField]
        private PortalController portal;
        [SerializeField]
        private ChapterTransitionController chapterTransition;
        [SerializeField]
        private AlmanacController almanac;
        [SerializeField]
        private InputNameDialogController inputNameDialog;
        [SerializeField]
        private DeleteUserDialogController deleteUserDialog;
        [SerializeField]
        private AchievementHintController achievementHint;
    }
}
