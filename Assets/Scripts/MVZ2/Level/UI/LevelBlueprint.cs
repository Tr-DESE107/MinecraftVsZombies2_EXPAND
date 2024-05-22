using System;
using MVZ2.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MVZ2.Level.UI
{
    public class LevelBlueprint : MonoBehaviour
    {
        private void Awake()
        {
            blueprint.OnPointerDown += () => OnPointerDown?.Invoke();
        }
        public event Action OnPointerDown;
        public Blueprint Blueprint => blueprint;
        [SerializeField]
        private Blueprint blueprint;
    }
}
