using MVZ2.Managers;
using MVZ2.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MVZ2.Debugs
{
    public class DebugConsoleIcon : Selectable, IPointerClickHandler
    {
        public void OnPointerClick(PointerEventData eventData)
        {
            if (!eventData.dragging)
            {
                Main.Scene.DisplayConsole();
            }
        }
        private MainManager Main => MainManager.Instance;
    }
}
