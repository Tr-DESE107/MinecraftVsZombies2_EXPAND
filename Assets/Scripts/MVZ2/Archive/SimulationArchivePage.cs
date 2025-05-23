using UnityEngine;
using UnityEngine.UI;

namespace MVZ2.Archives
{
    public class SimulationArchivePage : ArchivePage
    {
        public void SetBackground(Sprite background)
        {
            backgroundImage.sprite = background;
            if (background)
            {
                backgroundRatioFitter.aspectRatio = background.rect.width / background.rect.height;
            }
        }
        [SerializeField]
        private Image backgroundImage;
        [SerializeField]
        private AspectRatioFitter backgroundRatioFitter;
    }
}
