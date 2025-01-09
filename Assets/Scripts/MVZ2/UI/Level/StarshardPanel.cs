using System;
using MVZ2.UI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MVZ2.Level.UI
{
    public class StarshardPanel : LevelUIUnit
    {
        public void SetIconSprite(Sprite sprite)
        {
            icon.SetSprite(sprite);
        }
        public void SetSelected(bool selected)
        {
            animator.SetBool("Selected", selected);
        }
        public void SetDisabled(bool selected)
        {
            animator.SetBool("Disabled", selected);
        }
        public void SetPoints(int count, int maxCount)
        {
            pointsList.updateList(maxCount, (i, rect) =>
            {
                var point = rect.GetComponent<StarshardPanelPoint>();
                point.SetHighlight(i < count);
            });
        }
        private void Awake()
        {
            icon.OnPointerDown += (data) => OnPointerDown?.Invoke(data);
        }
        public event Action<PointerEventData> OnPointerDown;
        public StarshardPanelIcon Icon => icon;
        [SerializeField]
        private Animator animator;
        [SerializeField]
        private StarshardPanelIcon icon;
        [SerializeField]
        private ElementListUI pointsList;
    }
}
