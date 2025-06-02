using System;
using System.Collections.Generic;
using MVZ2.Managers;
using MVZ2Logic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MVZ2
{
    public class LevelPointerInteractionHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IPointerReleaseHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IPointerClickHandler, IDropHandler
    {
        public void UpdateHoldAndStreak()
        {
            foreach (var eventData in hoveredPointerDatas)
            {
                OnPointerInteraction?.Invoke(this, eventData, PointerInteraction.Stay);
                if (pressedPointerDatas.Exists(e => e.pointerId == eventData.pointerId))
                {
                    OnPointerInteraction?.Invoke(this, eventData, PointerInteraction.Hold);
                }
                if (InputManager.IsPointerHolding(eventData.pointerId))
                {
                    OnPointerInteraction?.Invoke(this, eventData, PointerInteraction.Streak);
                }
            }
        }
        public bool IsHovered()
        {
            return hoveredPointerDatas.Count > 0;
        }
        public bool IsPressed()
        {
            return hoveredPointerDatas.Count > 0;
        }
        public int GetHoveredPointerCount()
        {
            return hoveredPointerDatas.Count;
        }
        public PointerEventData GetHoveredPointerEventData(int index)
        {
            return hoveredPointerDatas[index];
        }

        #region 私有方法


        #region 接口实现
        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
            hoveredPointerDatas.Add(eventData);
            OnPointerInteraction?.Invoke(this, eventData, PointerInteraction.Enter);
        }
        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            hoveredPointerDatas.RemoveAll(e => e.pointerId == eventData.pointerId);
            OnPointerInteraction?.Invoke(this, eventData, PointerInteraction.Exit);
        }
        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            pressedPointerDatas.Add(eventData);
            OnPointerInteraction?.Invoke(this, eventData, PointerInteraction.Down);
        }
        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {
            pressedPointerDatas.RemoveAll(e => e.pointerId == eventData.pointerId);
            OnPointerInteraction?.Invoke(this, eventData, PointerInteraction.Up);
        }
        void IPointerReleaseHandler.OnPointerRelease(PointerEventData eventData)
        {
            OnPointerInteraction?.Invoke(this, eventData, PointerInteraction.Release);
        }
        void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
        {
            OnPointerInteraction?.Invoke(this, eventData, PointerInteraction.BeginDrag);
        }
        void IDragHandler.OnDrag(PointerEventData eventData)
        {
            OnPointerInteraction?.Invoke(this, eventData, PointerInteraction.Drag);
        }
        void IEndDragHandler.OnEndDrag(PointerEventData eventData)
        {
            OnPointerInteraction?.Invoke(this, eventData, PointerInteraction.EndDrag);
        }
        void IDropHandler.OnDrop(PointerEventData eventData)
        {
            OnPointerInteraction?.Invoke(this, eventData, PointerInteraction.Drop);
        }
        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
            OnPointerInteraction?.Invoke(this, eventData, PointerInteraction.Click);
        }
        #endregion

        #endregion

        #region 事件
        public event Action<LevelPointerInteractionHandler, PointerEventData, PointerInteraction> OnPointerInteraction;
        #endregion

        #region 属性字段
        private List<PointerEventData> hoveredPointerDatas = new List<PointerEventData>();
        private List<PointerEventData> pressedPointerDatas = new List<PointerEventData>();
        #endregion
    }
    public interface IPointerReleaseHandler : IEventSystemHandler
    {
        /// <summary>
        /// Use this callback to detect pointer up events.
        /// </summary>
        void OnPointerRelease(PointerEventData eventData);
    }
}
