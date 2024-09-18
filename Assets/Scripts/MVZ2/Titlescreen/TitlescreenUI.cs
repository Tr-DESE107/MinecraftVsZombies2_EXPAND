using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MVZ2.Titlescreen
{
    public class TitlescreenUI : MonoBehaviour
    {
        public void SetVersionText(string text)
        {
            versionText.text = text;
        }
        private void Awake()
        {
            button.onClick.AddListener(OnButtonClickCallback);
        }
        private void OnButtonClickCallback()
        {
            OnButtonClick?.Invoke();
        }
        public event Action OnButtonClick;
        #region 属性字段
        [SerializeField]
        private Button button;
        [SerializeField]
        private TextMeshProUGUI versionText;
        #endregion
    }
}
