using System;
using System.Linq;
using PVZEngine;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MVZ2.Map
{
    public class MapKey : Selectable, IPointerClickHandler
    {
        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
            if (!IsInteractable())
                return;
            // Not left mouse nor first touch.
            if (eventData.pointerId != -1 && eventData.pointerId != 0)
                return;
            OnClick?.Invoke();
        }
        public event Action OnClick;
    }
}
