using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using MukioI18n;
using MVZ2.Managers;
using MVZ2.Scenes;
using MVZ2.Talk;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Callbacks;
using MVZ2Logic;
using MVZ2Logic.Archives;
using MVZ2Logic.Talk;
using PVZEngine;
using PVZEngine.Callbacks;
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
