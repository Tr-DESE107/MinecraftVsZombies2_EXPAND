using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MVZ2.MusicRoom
{
    public class MusicBar : MonoBehaviour, IPointerUpHandler
    {
        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {
            OnPointerUp?.Invoke();
        }
        public event Action OnPointerUp;

    }
}
