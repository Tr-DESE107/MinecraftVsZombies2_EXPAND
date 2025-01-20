using System;
using System.Linq;
using PVZEngine;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MVZ2.Map
{
    public class MapElementButton : Selectable, IPointerClickHandler
    {
        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
            if (!IsInteractable())
                return;
            // Not left mouse nor first touch.
            if (eventData.pointerId != -1 && eventData.pointerId != 0)
                return;
            OnClick?.Invoke(id.Get());
        }
        public event Action<NamespaceID> OnClick;
        [SerializeField]
        private NamespaceIDReference id;
    }
}
