using MVZ2.Managers;
using MVZ2.Map;
using MVZ2.Metas;
using MVZ2.Scenes;
using MVZ2.Talk;
using MVZ2.Talks;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Callbacks;
using MVZ2Logic;
using MVZ2Logic.Callbacks;
using MVZ2Logic.Games;
using MVZ2Logic.Notes;
using MVZ2Logic.Talk;
using PVZEngine;
using PVZEngine.Callbacks;
using UnityEngine;

namespace MVZ2.Note
{
    public class NoteController : MainScenePage, INote
    {
        public async void SetNote(NamespaceID id)
        {
            meta = main.ResourceManager.GetNoteMeta(id);
            definition = main.Game.GetNoteDefinition(id);
            isFlipped = false;
            ui.SetNoteSprite(main.GetFinalSprite(meta.sprite));
            ui.SetBackground(main.GetFinalSprite(meta.background));
            ui.SetCanFlip(meta.canFlip);
            ui.SetFlipAtLeft(isFlipped);
            var startTalk = meta.startTalk ?? new NamespaceID(id.SpaceName, $"{id.Path}_note");
            await talkController.SimpleStartTalkAsync(startTalk, 0, 3, () => SetInteractable(false));
            SetInteractable(true);
        }
        public void SetButtonText(string text)
        {
            ui.SetButtonText(text);
        }
        public void SetInteractable(bool interactable)
        {
            ui.SetButtonInteractable(interactable);
        }
        #region 生命周期
        private void Awake()
        {
            ui.OnNoteFlipClick += OnNoteFlipClickCallback;
            ui.OnButtonClick += OnButtonClickCallback;
            talkController.OnTalkAction += OnTalkActionCallback;
            talkSystem = new NoteTalkSystem(talkController);
        }
        #endregion

        #region 事件回调
        private void OnNoteFlipClickCallback()
        {
            isFlipped = !isFlipped;
            main.SoundManager.Play2D(VanillaSoundID.paper);
            var sprRef = isFlipped ? meta.flipSprite : meta.sprite;
            ui.SetNoteSprite(main.GetFinalSprite(sprRef));
            ui.SetFlipAtLeft(isFlipped);
        }
        private void OnButtonClickCallback()
        {
            definition?.OnBack(this);
        }
        private void OnTalkActionCallback(string cmd, string[] parameters)
        {
            Global.Game.RunCallbackFiltered(VanillaCallbacks.TALK_ACTION, new VanillaCallbacks.TalkActionParams(talkSystem, cmd, parameters), cmd);
        }
        #endregion

        #region 属性字段
        private MainManager main => MainManager.Instance;
        private NoteMeta meta;
        private NoteDefinition definition;
        private ITalkSystem talkSystem;
        private bool isFlipped;
        [SerializeField]
        private TalkController talkController;
        [SerializeField]
        private NoteUI ui;
        #endregion
    }
}
