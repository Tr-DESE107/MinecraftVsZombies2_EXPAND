using UnityEngine;

namespace MVZ2.Level.UI
{
    public class ProgressBarBanner : MonoBehaviour
    {
        public void SetRiseProgress(float progress)
        {
            var sizeDelta = pole.sizeDelta;
            sizeDelta.y = Mathf.Lerp(startY, endY, progress);
            pole.sizeDelta = sizeDelta;
        }
        [SerializeField]
        private RectTransform pole;
        [SerializeField]
        private float startY;
        [SerializeField]
        private float endY;
    }
}
