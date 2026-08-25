#nullable enable

using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MVZ2.UI.Level
{
    /// <summary>  
    /// 可“按住”的按钮：按下时 IsHeld=true，抬起或手指滑出时 IsHeld=false。  
    /// 用于重装兵器关卡在移动端控制矿车上下移动。  
    /// </summary>  
    public class MinecartMoveButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
    {
        public bool IsHeld { get; private set; }

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            SetHeld(true);
        }
        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {
            SetHeld(false);
        }
        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            // 手指/指针滑出按钮范围也视为松手，避免卡住持续移动  
            SetHeld(false);
        }
        private void OnDisable()
        {
            // 关卡切换或按钮隐藏时复位，避免残留按住状态  
            SetHeld(false);
        }
        private void SetHeld(bool held)
        {
            if (IsHeld == held)
                return;
            IsHeld = held;
            OnHeldChanged?.Invoke(held);
        }

        public event Action<bool>? OnHeldChanged;
    }
}
