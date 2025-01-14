using MVZ2.Managers;
using MVZ2.Vanilla.Callbacks;
using MVZ2Logic;
using PVZEngine.Triggers;
using UnityEngine;
using UnityEngine.UIElements;

namespace MVZ2.Assets.Scripts.MVZ2.Managers
{
    public class InputManager : MonoBehaviour
    {
        private void Update()
        {
            for (int mouse = 0; mouse <= 2; mouse++)
            {
                if (Input.GetMouseButtonDown(mouse))
                {
                    RunPointerCallback(PointerTypes.MOUSE, mouse, Input.mousePosition, PointerPhase.Press);
                }
                if (Input.GetMouseButton(mouse))
                {
                    RunPointerCallback(PointerTypes.MOUSE, mouse, Input.mousePosition, PointerPhase.Hold);
                }
                if (Input.GetMouseButtonUp(mouse))
                {
                    RunPointerCallback(PointerTypes.MOUSE, mouse, Input.mousePosition, PointerPhase.Release);
                }
            }
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
        }
        private void RunPointerCallback(int type, int index, Vector2 screenPosition, PointerPhase phase)
        {
            Main.Game.RunCallbackFiltered(VanillaCallbacks.POST_POINTER_ACTION, phase, c => c(type, index, screenPosition, phase));
        }
        public MainManager Main => MainManager.Instance;
    }
    public static class MouseButton
    {
        public const int LEFT = 0;
        public const int RIGHT = 1;
        public const int MIDDLE = 2;
    }
}
