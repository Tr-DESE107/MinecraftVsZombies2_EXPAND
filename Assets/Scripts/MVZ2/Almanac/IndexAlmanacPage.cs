using System;
using UnityEngine;
using UnityEngine.UI;

namespace MVZ2.Almanacs
{
    public class IndexAlmanacPage : AlmanacPage
    {
        protected override void Awake()
        {
            base.Awake();
            viewContraptionButton.onClick.AddListener(() => OnButtonClick?.Invoke(ButtonType.ViewContraption));
            viewEnemyButton.onClick.AddListener(() => OnButtonClick?.Invoke(ButtonType.ViewEnemy));
            viewCharacterButton.onClick.AddListener(() => OnButtonClick?.Invoke(ButtonType.ViewCharacter));
            viewMiscButton.onClick.AddListener(() => OnButtonClick?.Invoke(ButtonType.ViewMisc));
        }
        public event Action<ButtonType> OnButtonClick;
        [SerializeField]
        private Button viewContraptionButton;
        [SerializeField]
        private Button viewEnemyButton;
        [SerializeField]
        private Button viewCharacterButton;
        [SerializeField]
        private Button viewMiscButton;
        public enum ButtonType
        {
            ViewContraption,
            ViewEnemy,
            ViewCharacter,
            ViewMisc
        }
    }
}
