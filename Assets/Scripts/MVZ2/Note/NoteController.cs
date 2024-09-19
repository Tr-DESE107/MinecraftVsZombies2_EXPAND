using System;
using MVZ2.GameContent;
using PVZEngine;
using UnityEngine;

namespace MVZ2.Note
{
    public class NoteController : MainScenePage
    {
        public void SetNote(NamespaceID id)
        {
            SetNote(main.ResourceManager.GetNoteMeta(id));
        }
        public void SetNote(NoteMeta meta)
        {
            this.meta = meta;
            ui.SetNoteSprite(main.LanguageManager.GetSprite(meta.sprite));
            ui.SetBackground(main.LanguageManager.GetSprite(meta.background));
            ui.SetCanFlip(meta.canFlip);
            ui.SetFlipAtLeft(isFlipped);
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
        #endregion

        public event Action OnClose;

        #region 属性字段
        private MainManager main => MainManager.Instance;
        private NoteMeta meta;
        private bool isFlipped;
        [SerializeField]
        private NoteUI ui;
        #endregion
    }
}
