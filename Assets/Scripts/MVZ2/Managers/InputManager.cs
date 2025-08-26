using System.Collections.Generic;
using MVZ2.Vanilla.Callbacks;
using MVZ2Logic;
using MVZ2Logic.Games;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MVZ2.Managers
{
    public partial class InputManager : MonoBehaviour, IGlobalInput
    {
        #region 检测指针状态
        public static bool IsPointerDown(int type, int button) => IsPointerDown(GetPointerIdByButtonAndType(button, type));
        public static bool IsPointerHolding(int type, int button) => IsPointerHolding(GetPointerIdByButtonAndType(button, type));
        public static bool IsPointerUp(int type, int button) => IsPointerUp(GetPointerIdByButtonAndType(button, type));
        public int GetActivePointerType()
        {
            return currentPointerType;
        }
        public static bool IsPointerDown(int pointerId)
        {
            return IsPointerOfPhase(pointerId, PointerPhase.Press);
        }
        public static bool IsPointerHolding(int pointerId)
        {
            return IsPointerOfPhase(pointerId, PointerPhase.Hold);
        }
        public static bool IsPointerUp(int pointerId)
        {
            return IsPointerOfPhase(pointerId, PointerPhase.Release);
        }
        public static bool IsPointerOfPhase(int pointerId, PointerPhase phase)
        {
            if (IsPointerMouse(pointerId))
            {
                var mouseButton = GetPointerButton(pointerId);
                switch (phase)
                {
                    case PointerPhase.Press:
                        return Input.GetMouseButtonDown(mouseButton);
                    case PointerPhase.Hold:
                        return Input.GetMouseButton(mouseButton);
                    case PointerPhase.Release:
                        return Input.GetMouseButtonUp(mouseButton);
                }
                return false;
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
        #endregion

        #region 指针ID转换
        public static int GetPointerIdByButtonAndType(int button, int type)
        {
            return type == PointerTypes.MOUSE ? -button - 1 : button;
        }
        public static int GetPointerButton(int pointerId)
        {
            return IsPointerMouse(pointerId) ? -pointerId - 1 : pointerId;
        }
        public static int GetPointerType(int pointerId)
        {
            return IsPointerMouse(pointerId) ? PointerTypes.MOUSE : PointerTypes.TOUCH;
        }
        public static bool IsPointerMouse(int pointerId)
        {
            return pointerId < 0;
        }
        #endregion

        #region 指针数据
        public static PointerData GetPointerDataFromEventData(PointerEventData eventData)
        {
            return new PointerData()
            {
                button = (int)eventData.button,
                type = GetPointerType(eventData.pointerId)
            };
        }
        public static PointerData GetPointerDataFromPointerId(int pointerId)
        {
            return new PointerData()
            {
                button = GetPointerButton(pointerId),
                type = GetPointerType(pointerId)
            };
        }
        public static PointerInteractionData GetPointerInteractionParamsFromEventData(PointerEventData eventData, PointerInteraction interaction)
        {
            return new PointerInteractionData()
            {
                pointer = GetPointerDataFromEventData(eventData),
                interaction = interaction
            };
        }
        public static PointerInteractionData GetPointerInteractionParamsFromPointerId(int pointerId, PointerInteraction interaction)
        {
            return new PointerInteractionData()
            {
                pointer = GetPointerDataFromPointerId(pointerId),
                interaction = PointerInteraction.Hold
            };
        }
        #endregion

        #region 获取指针位置
        public Vector2 GetPointerPosition(int pointerId)
        {
            if (IsPointerMouse(pointerId))
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
        #endregion

        #region 获取所有指针
        public IEnumerable<PointerPositionParams> GetTouchUps()
        {
            var touches = Input.touches;
            if (touches.Length > 0)
            {
                for (int i = 0; i < touches.Length; i++)
                {
                    var touch = touches[i];
                    if (touch.phase == TouchPhase.Canceled || touch.phase == TouchPhase.Ended)
                    {
                        yield return new PointerPositionParams()
                        {
                            button = i,
                            type = PointerTypes.TOUCH,
                            position = touch.position
                        };
                    }
                }
            }
        }
        public IEnumerable<PointerPositionParams> GetMouseUps(int button)
        {
            if (Input.GetMouseButtonUp(button))
            {
                yield return new PointerPositionParams()
                {
                    button = button,
                    type = PointerTypes.MOUSE,
                    position = Input.mousePosition
                };
            }
        }
        #endregion

        private void Update()
        {
            UpdatePointer();
            UpdateKeys();
        }
        private void UpdatePointer()
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
                        var position = (Vector2)Input.mousePosition;
                        var delta = position - lastMousePosition;
                        lastMousePosition = position;
                        for (int mouse = 0; mouse <= 2; mouse++)
                        {
                            bool hasState = false;
                            if (Input.GetMouseButtonDown(mouse))
                            {
                                hasState = true;
                                PollPointerEvent(currentPointerType, mouse, position, delta, PointerPhase.Press);
                            }
                            if (Input.GetMouseButton(mouse))
                            {
                                hasState = true;
                                PollPointerEvent(currentPointerType, mouse, position, delta, PointerPhase.Hold);
                            }
                            if (Input.GetMouseButtonUp(mouse))
                            {
                                hasState = true;
                                PollPointerEvent(currentPointerType, mouse, position, delta, PointerPhase.Release);
                            }
                            if (!hasState)
                            {
                                PollPointerEvent(currentPointerType, mouse, position, delta, PointerPhase.None);
                            }
                        }
                    }
                    break;
                case PointerTypes.TOUCH:
                    int touchCount = Input.touchCount;
                    for (int i = 0; i < touchCount; i++)
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
                        var delta = Vector2.zero;
                        if (lastTouchPositions.Count > i)
                        {
                            delta = touch.position - lastTouchPositions[i];
                            lastTouchPositions[i] = touch.position;
                        }
                        else
                        {
                            lastTouchPositions.Add(touch.position);
                        }
                        PollPointerEvent(currentPointerType, touch.fingerId, touch.position, delta, phase);
                    }
                    lastTouchPositions.RemoveRange(touchCount, lastTouchPositions.Count - touchCount);
                    break;
            }
            FlushPointerCaches();

        }
        private void FlushPointerCaches()
        {
            foreach (var poll in pointerEventCacheList)
            {
                var param = new VanillaCallbacks.PostPointerActionParams()
                {
                    type = poll.type,
                    button = poll.button,
                    screenPos = poll.position,
                    phase = poll.phase,
                    delta = poll.delta,
                };
                Main.Game.RunCallbackFiltered(VanillaCallbacks.POST_POINTER_ACTION, param, param.phase);
            }
            pointerEventCacheList.Clear();
        }
        private void PollPointerEvent(int type, int button, Vector2 screenPosition, Vector2 delta, PointerPhase phase)
        {
            var cache = new PointerEventCacheData()
            {
                type = type,
                button = button,
                position = screenPosition,
                phase = phase,
                delta = delta,
            };
            pointerEventCacheList.Add(cache);
        }
        Vector2 IGlobalInput.GetPointerScreenPosition()
        {
            return GetPointerPosition();
        }
        bool IGlobalInput.IsPointerDown(int type, int button)
        {
            return IsPointerDown(type, button);
        }
        bool IGlobalInput.IsPointerHolding(int type, int button)
        {
            return IsPointerHolding(type, button);
        }
        bool IGlobalInput.IsPointerUp(int type, int button)
        {
            return IsPointerUp(type, button);
        }
        public MainManager Main => MainManager.Instance;
        private int currentPointerType = PointerTypes.MOUSE;
        private Vector2 lastMousePosition;
        private List<Vector2> lastTouchPositions = new List<Vector2>();
        private List<PointerEventCacheData> pointerEventCacheList = new List<PointerEventCacheData>();
    }
    public struct PointerPositionParams
    {
        public int type;
        public int button;
        public Vector2 position;
    }
    public struct PointerEventCacheData
    {
        public int type;
        public int button;
        public Vector2 position;
        public Vector2 delta;
        public PointerPhase phase;

        public override string ToString()
        {
            return $"type: {type}, button: {button}, position: {position}, delta: {delta}, phase: {phase}";
        }
    }
}
