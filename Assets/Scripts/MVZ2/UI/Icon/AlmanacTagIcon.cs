using System;
using MVZ2.Level.UI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MVZ2.UI
{
    public class AlmanacTagIcon : MonoBehaviour, ITooltipTarget, IPointerEnterHandler, IPointerExitHandler
    {
        public void UpdateContainer(AlmanacTagIconViewData viewData)
        {
            UpdateBackground(viewData.background);
            UpdateMain(viewData.main);
            UpdateMark(viewData.mark);
        }
        public void UpdateBackground(AlmanacTagIconLayerViewData viewData)
        {
            backgroundLayer.UpdateView(viewData);
        }
        public void UpdateMain(AlmanacTagIconLayerViewData viewData)
        {
            mainLayer.UpdateView(viewData);
        }
        public void UpdateMark(AlmanacTagIconLayerViewData viewData)
        {
            markLayer.UpdateView(viewData);
        }
        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
            OnPointerEnter?.Invoke(this);
        }
        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            OnPointerExit?.Invoke(this);
        }
        TooltipAnchor ITooltipTarget.Anchor => tooltipAnchor;
        public event Action<AlmanacTagIcon> OnPointerEnter;
        public event Action<AlmanacTagIcon> OnPointerExit;
        [SerializeField]
        private TooltipAnchor tooltipAnchor;
        [SerializeField]
        private AlmanacTagIconLayer backgroundLayer;
        [SerializeField]
        private AlmanacTagIconLayer mainLayer;
        [SerializeField]
        private AlmanacTagIconLayer markLayer;
    }
    public struct AlmanacTagIconViewData
    {
        public AlmanacTagIconLayerViewData background;
        public AlmanacTagIconLayerViewData main;
        public AlmanacTagIconLayerViewData mark;
    }
}
