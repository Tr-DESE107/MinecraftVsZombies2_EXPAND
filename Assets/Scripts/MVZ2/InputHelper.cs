﻿using MVZ2.Managers;
using MVZ2Logic;
using UnityEngine.EventSystems;

namespace MVZ2
{
    public static class InputHelper
    {
        public static bool IsMouseButNotLeft(this PointerEventData data)
        {
            var pointerType = InputManager.GetPointerType(data.pointerId);
            var pointerButton = InputManager.GetPointerButton(data.pointerId);
            return pointerType == PointerTypes.MOUSE && pointerButton != MouseButtons.LEFT;
        }
    }
}
