using MVZ2.UI;
using UnityEngine;
using UnityEngine.UI;

namespace MVZ2.Level.UI
{
    public class ProgressBar : MonoBehaviour
    {
        public void SetProgress(float progress)
        {
            slider.SetValueWithoutNotify(progress);
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
        private ElementListUI flags;
        [SerializeField]
        private Slider slider;
    }
}
