using System;
using MVZ2.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MVZ2.Level.UI
{
    public class StarshardPanel : MonoBehaviour
    {
        public void SetIconSprite(Sprite sprite)
        {
            icon.SetSprite(sprite);
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
        [SerializeField]
        private StarshardPanelIcon icon;
        [SerializeField]
        private ElementList pointsList;
    }
}
