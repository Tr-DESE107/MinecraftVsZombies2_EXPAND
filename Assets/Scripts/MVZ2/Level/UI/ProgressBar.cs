using MVZ2.UI;
using UnityEngine;

namespace MVZ2.Level.UI
{
    public class ProgressBar : MonoBehaviour
    {
        public void SetProgress(float progress)
        {
            var barContentRectTrans = barTransform.parent as RectTransform;
            var x = Mathf.Lerp(0, barContentRectTrans.rect.width, progress);
            var sizeDelta = barTransform.sizeDelta;
            sizeDelta.x = x;
            barTransform.sizeDelta = sizeDelta;

            var iconPos = iconTransform.anchoredPosition;
            iconPos.x = -x;
            iconTransform.anchoredPosition = iconPos;
        }
        public void SetBannerProgresses(float[] progresses)
        {
            flags.updateList(progresses.Length, (i, rect) =>
            {
                var component = rect.GetComponent<ProgressBarBanner>();
                component.SetRiseProgress(progresses[i]);
            });
        }

        [SerializeField]
        private ElementList flags;
        [SerializeField]
        private RectTransform barTransform;
        [SerializeField]
        private RectTransform iconTransform;
    }
}
