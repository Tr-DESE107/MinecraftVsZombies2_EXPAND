using System;
using log4net.Core;
using MVZ2.GameContent;
using MVZ2.Games;
using MVZ2.Managers;
using MVZ2.Resources;
using MVZ2.Talk;
using PVZEngine;
using UnityEngine;

namespace MVZ2.Note
{
    public class NoteController : MainScenePage
    {
        public void SetNote(NamespaceID id)
        {
            meta = main.ResourceManager.GetNoteMeta(id);
            ui.SetNoteSprite(main.LanguageManager.GetSprite(meta.sprite));
            ui.SetBackground(main.LanguageManager.GetSprite(meta.background));
            ui.SetCanFlip(meta.canFlip);
            ui.SetFlipAtLeft(isFlipped);
            var startTalk = meta.startTalk ?? new NamespaceID(id.spacename, $"{id.path}_note");
            if (NamespaceID.IsValid(startTalk) && main.ResourceManager.GetTalkGroup(startTalk) != null)
            {
                StartTalk(startTalk, 0, 3);
                ui.SetButtonInteractable(false);
            }
        }
        public void SetButtonText(string text)
        {
            ui.SetButtonText(text);
        }
        public override void Hide()
        {
            base.Hide();
            OnClose = null;
        }
        #region 生命周期
        private void Awake()
        {
            ui.OnNoteFlipClick += OnNoteFlipClickCallback;
            ui.OnButtonClick += OnButtonClickCallback;
            talkController.OnTalkAction += OnTalkActionCallback;
            talkController.OnTalkEnd += OnTalkEndCallback;
        }
        #endregion

        #region 事件回调
        private void OnNoteFlipClickCallback()
        {
            isFlipped = !isFlipped;
            main.SoundManager.Play2D(SoundID.paper);
            var sprRef = isFlipped ? meta.flipSprite : meta.sprite;
            ui.SetNoteSprite(main.LanguageManager.GetSprite(sprRef));
            ui.SetFlipAtLeft(isFlipped);
        }
        private void OnButtonClickCallback()
        {
            OnClose?.Invoke();
        }
        private void OnTalkActionCallback(string action, string[] param)
        {

        }
        private void OnTalkEndCallback()
        {
            ui.SetButtonInteractable(true);
        }
        #endregion

        private void StartTalk(NamespaceID groupId, int section, float delay = 0)
        {
            talkController.StartTalk(groupId, section, delay);
        }

        public event Action OnClose;

        #region 属性字段
        private MainManager main => MainManager.Instance;
        private NoteMeta meta;
        private bool isFlipped;
        [SerializeField]
        private TalkController talkController;
        [SerializeField]
        private NoteUI ui;
        #endregion
    }
}
