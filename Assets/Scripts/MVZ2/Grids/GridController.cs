using System;
using MVZ2.HeldItems;
using MVZ2.Level;
using MVZ2Logic.HeldItems;
using PVZEngine.Level;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MVZ2.Grids
{
    public class GridController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, ILevelRaycastReceiver
    {
        public void UpdateGrid(GridViewData viewData)
        {
            transform.localPosition = viewData.position;
            spriteRenderer.sprite = viewData.sprite;
        }
        public void SetColor(Color color)
        {
            spriteRenderer.color = color;
        }
        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
            OnPointerEnter?.Invoke(this, eventData);
        }
        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            OnPointerExit?.Invoke(this, eventData);
        }
        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            OnPointerDown?.Invoke(this, eventData);
        }
        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {
            OnPointerUp?.Invoke(this, eventData);
        }
        bool ILevelRaycastReceiver.IsValidReceiver(LevelEngine level, HeldItemDefinition definition, IHeldItemData data)
        {
            return definition != null && definition.IsForGrid();
        }
        int ILevelRaycastReceiver.GetSortingLayer()
        {
            return spriteRenderer.sortingLayerID;
        }
        int ILevelRaycastReceiver.GetSortingOrder()
        {
            return spriteRenderer.sortingOrder;
        }
        public event Action<GridController, PointerEventData> OnPointerEnter;
        public event Action<GridController, PointerEventData> OnPointerExit;
        public event Action<GridController, PointerEventData> OnPointerDown;
        public event Action<GridController, PointerEventData> OnPointerUp;
        public int Lane { get; set; }
        public int Column { get; set; }
        [SerializeField]
        private SpriteRenderer spriteRenderer;
    }
    public struct GridViewData
    {
        public Vector2 position;
        public Sprite sprite;
    }
}
