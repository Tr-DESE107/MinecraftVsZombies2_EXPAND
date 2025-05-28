using System.Collections.Generic;
using MVZ2.Vanilla.Callbacks;
using MVZ2Logic;
using UnityEngine;

namespace MVZ2.Managers
{
    public partial class InputManager : MonoBehaviour
    {
        public bool IsPointerDown(int pointerId)
        {
            return IsPointerOfPhase(pointerId, PointerPhase.Press);
        }
        public bool IsPointerHolding(int pointerId)
        {
            return IsPointerOfPhase(pointerId, PointerPhase.Hold);
        }
        public bool IsPointerUp(int pointerId)
        {
            return IsPointerOfPhase(pointerId, PointerPhase.Release);
        }
        public bool IsPointerOfPhase(int pointerId, PointerPhase phase)
        {
            if (pointerId < 0)
            {
                var mouseButton = -pointerId - 1;
                switch (phase)
                {
                    case PointerPhase.Press:
                        return Input.GetMouseButtonDown(mouseButton);
                    case PointerPhase.Hold:
                        return Input.GetMouseButton(mouseButton);
                    case PointerPhase.Release:
                        return Input.GetMouseButtonUp(mouseButton);
                }
            }
            var touches = Input.touches;
            for (int i = 0; i < touches.Length; i++)
            {
                var touch = touches[i];
                if (touch.fingerId == pointerId)
                {
                    switch (phase)
                    {
                        case PointerPhase.Press:
                            return touch.phase == TouchPhase.Began;
                        case PointerPhase.Hold:
                            return touch.phase == TouchPhase.Stationary || touch.phase == TouchPhase.Moved;
                        case PointerPhase.Release:
                            return touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled;
                    }
                }
            }
            return false;
        }
        public Vector2 GetPointerPosition(int pointerId)
        {
            if (pointerId < 0)
            {
                return Input.mousePosition;
            }
            var touches = Input.touches;
            for (int i = 0; i < touches.Length; i++)
            {
                var touch = touches[i];
                if (touch.fingerId == pointerId)
                {
                    return touch.position;
                }
            }
            return Vector2.zero;
        }
        public Vector2 GetPointerPosition()
        {
            if (Input.touchCount > 0)
            {
                return Input.GetTouch(0).position;
            }
            return Input.mousePosition;
        }
        public IEnumerable<Vector2> GetLeftPointerUps()
        {
            var touches = Input.touches;
            if (touches.Length > 0)
            {
                for (int i = 0; i < touches.Length; i++)
                {
                    var touch = touches[i];
                    if (touch.phase == TouchPhase.Canceled || touch.phase == TouchPhase.Ended)
                    {
                        yield return touch.position;
                    }
                }
            }
            else
            {
                if (Input.GetMouseButtonUp(MouseButton.LEFT))
                {
                    yield return Input.mousePosition;
                }
            }
        }
        private void Update()
        {
            if (currentPointerType == PointerTypes.MOUSE)
            {
                if (Input.touchCount > 0)
                {
                    currentPointerType = PointerTypes.TOUCH;
                }
            }
            else if (currentPointerType == PointerTypes.TOUCH)
            {
                if (Input.touchCount <= 0 && Input.mousePresent)
                {
                    var mousePos = Input.mousePosition;
                    if (mousePos.x >= 0 && mousePos.y >= 0 && mousePos.x <= Screen.width && mousePos.y <= Screen.height)
                    {
                        for (int mouse = 0; mouse <= 2; mouse++)
                        {
                            if (Input.GetMouseButtonDown(mouse))
                            {
                                currentPointerType = PointerTypes.MOUSE;
                                break;
                            }
                        }
                    }
                }
            }

            switch (currentPointerType)
            {
                case PointerTypes.MOUSE:
                    {
                        for (int mouse = 0; mouse <= 2; mouse++)
                        {
                            bool hasState = false;
                            if (Input.GetMouseButtonDown(mouse))
                            {
                                hasState = true;
                                RunPointerCallback(PointerTypes.MOUSE, mouse, Input.mousePosition, PointerPhase.Press);
                            }
                            if (Input.GetMouseButton(mouse))
                            {
                                hasState = true;
                                RunPointerCallback(PointerTypes.MOUSE, mouse, Input.mousePosition, PointerPhase.Hold);
                            }
                            if (Input.GetMouseButtonUp(mouse))
                            {
                                hasState = true;
                                RunPointerCallback(PointerTypes.MOUSE, mouse, Input.mousePosition, PointerPhase.Release);
                            }
                            if (!hasState)
                            {
                                RunPointerCallback(PointerTypes.MOUSE, mouse, Input.mousePosition, PointerPhase.None);
                            }
                        }
                    }
                    break;
                case PointerTypes.TOUCH:
                    for (int i = 0; i < Input.touchCount; i++)
                    {
                        var touch = Input.GetTouch(i);
                        PointerPhase phase = PointerPhase.Hold;
                        switch (touch.phase)
                        {
                            case TouchPhase.Began:
                                phase = PointerPhase.Press;
                                break;
                            case TouchPhase.Ended:
                            case TouchPhase.Canceled:
                                phase = PointerPhase.Release;
                                break;
                        }
                        RunPointerCallback(PointerTypes.TOUCH, touch.fingerId, touch.position, phase);
                    }
                    break;
            }
        }
        private void RunPointerCallback(int type, int index, Vector2 screenPosition, PointerPhase phase)
        {
            Main.Game.RunCallbackFiltered(VanillaCallbacks.POST_POINTER_ACTION, new VanillaCallbacks.PostPointerActionParams(type, index, screenPosition, phase), phase);
        }
        public MainManager Main => MainManager.Instance;
        private int currentPointerType = PointerTypes.MOUSE;
    }
    public static class MouseButton
    {
        public const int LEFT = 0;
        public const int RIGHT = 1;
        public const int MIDDLE = 2;
    }
}
