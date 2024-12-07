using System;
using MVZ2.Level;
using MVZ2Logic.HeldItems;
using PVZEngine.Entities;
using PVZEngine.Level;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MVZ2.Grids
{
    public class GridController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, ILevelRaycastReceiver
    {
        public void SetColor(Color color)
        {
            spriteRenderer.color = color;
        }
        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
            OnPointerEnter?.Invoke(eventData);
        }
        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            OnPointerExit?.Invoke(eventData);
        }
        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            OnPointerDown?.Invoke(eventData);
        }
        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {
            OnPointerUp?.Invoke(eventData);
        }
        bool ILevelRaycastReceiver.IsValidReceiver(LevelEngine level, HeldItemDefinition definition, long id)
        {
            if (definition == null || !definition.IsForGrid())
                return false;
            var flags = definition.GetHeldFlagsOnGrid(level.GetGrid(Column, Lane), id);
            return flags.HasFlag(HeldFlags.Valid);
        }
        public event Action<PointerEventData> OnPointerEnter;
        public event Action<PointerEventData> OnPointerExit;
        public event Action<PointerEventData> OnPointerDown;
        public event Action<PointerEventData> OnPointerUp;
        public int Lane { get; set; }
        public int Column { get; set; }
        [SerializeField]
        private SpriteRenderer spriteRenderer;
    }
}
