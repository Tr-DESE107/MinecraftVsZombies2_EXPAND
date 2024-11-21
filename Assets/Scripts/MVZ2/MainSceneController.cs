using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using MukioI18n;
using MVZ2.ChapterTransition;
using MVZ2.GameContent;
using MVZ2.Landing;
using MVZ2.Localization;
using MVZ2.Logic.Scenes;
using MVZ2.Mainmenu;
using MVZ2.Managers;
using MVZ2.Map;
using MVZ2.Note;
using MVZ2.Titlescreen;
using MVZ2.UI;
using PVZEngine;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace MVZ2
{
    public class MainSceneController : MonoBehaviour, ISceneController
    {
        public void ShowDialogConfirm(string title, string desc, Action<bool> onSelect = null)
        {
            ShowDialog(title, desc, new string[]
            {
                main.LanguageManager._(StringTable.YES),
                main.LanguageManager._(StringTable.NO),
            }, (index) => onSelect?.Invoke(index == 0));
        }
        public Task<bool> ShowDialogConfirmAsync(string title, string desc)
        {
            var tcs = new TaskCompletionSource<bool>();
            ShowDialogConfirm(title, desc, (result) => tcs.SetResult(result));
            return tcs.Task;
        }
        public void ShowDialog(string title, string desc, string[] options, Action<int> onSelect = null)
        {
            ui.ShowDialog(title, desc, options, onSelect);
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
    }
}
