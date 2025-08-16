using System;
using UnityEngine;
using UnityEngine.UI;

namespace MVZ2.UI
{
    public class MainSceneUI : MonoBehaviour
    {
        public void SetScreenCoverColor(Color value)
        {
            screenCoverFader.Value = value;
        }
        public void FadeScreenCoverColor(Color target, float duration)
        {
            screenCoverFader.StartFade(target, duration);
        }
        public void ShowDialog(string title, string desc, string[] options, Action<int> onSelect = null)
        {
            dialog.gameObject.SetActive(true);
            dialog.SetDialog(title, desc, options, (i) =>
            {
                dialog.gameObject.SetActive(false);
                onSelect?.Invoke(i);
            });
            dialog.ResetPosition();
        }
        public bool HasDialog()
        {
            return dialog.gameObject.activeSelf;
        }

        #region 工具提示
        public void ShowTooltip()
        {
            tooltip.Show();
            tooltip.ForceRebuildLayout();
        }
        public void HideTooltip()
        {
            tooltip.Hide();
        }
        public void UpdateTooltip(TooltipViewData viewData)
        {
            tooltip.SetView(viewData);
        }
        #endregion

        private void Awake()
        {
            screenCoverFader.OnValueChanged += OnBlackscreenFaderValueChangedCallback;
        }
        private void OnBlackscreenFaderValueChangedCallback(Color value)
        {
            blackscreenImage.color = value;
            blackscreenImage.raycastTarget = value.a > 0;
        }
        [SerializeField]
        private Tooltip tooltip;
        [SerializeField]
        private CustomDialog dialog;
        [SerializeField]
        private Image blackscreenImage;
        [SerializeField]
        private ColorFader screenCoverFader;
    }
}
