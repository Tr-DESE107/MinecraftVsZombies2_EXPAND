﻿using System;
using MVZ2.UI;
using UnityEngine;
using UnityEngine.UI;

namespace MVZ2.Note
{
    public class NoteUI : MonoBehaviour
    {
        public void SetNoteSprite(Sprite sprite)
        {
            noteImage.sprite = sprite;
        }
        public void SetBackground(Sprite sprite)
        {
            backgroundImage.sprite = sprite;
        }
        public void SetButtonText(string text)
        {
            button.Text.text = text;
        }
        public void SetButtonInteractable(bool interactable)
        {
            button.Button.interactable = interactable;
        }
        public void SetCanFlip(bool flip)
        {
            flipObj.SetActive(flip);
        }
        public void SetFlipAtLeft(bool left)
        {
            flipButtonLeft.gameObject.SetActive(left);
            flipButtonRight.gameObject.SetActive(!left);
        }
        private void Awake()
        {
            button.Button.onClick.AddListener(OnButtonClickCallback);
            flipButtonLeft.onClick.AddListener(OnNoteFlipClickCallback);
            flipButtonRight.onClick.AddListener(OnNoteFlipClickCallback);
        }
        private void OnNoteFlipClickCallback()
        {
            OnNoteFlipClick?.Invoke();
        }
        private void OnButtonClickCallback()
        {
            OnButtonClick?.Invoke();
        }
        public event Action OnButtonClick;
        public event Action OnNoteFlipClick;
        #region 属性字段
        [SerializeField]
        private Image noteImage;
        [SerializeField]
        private Image backgroundImage;
        [SerializeField]
        private GameObject flipObj;
        [SerializeField]
        private Button flipButtonLeft;
        [SerializeField]
        private Button flipButtonRight;
        [SerializeField]
        private TextButton button;
        #endregion
    }
}
