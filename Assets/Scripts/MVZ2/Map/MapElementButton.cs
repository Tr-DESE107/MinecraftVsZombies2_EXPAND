using System;
using System.Linq;
using MVZ2.UI;
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
        public void SetArrowVisible(bool visible)
        {
            arrow.SetActive(visible);
        }
        public event Action<NamespaceID> OnClick;
        [SerializeField]
        private NamespaceIDReference id;
        [SerializeField]
        private GameObject arrow;
    }
}
