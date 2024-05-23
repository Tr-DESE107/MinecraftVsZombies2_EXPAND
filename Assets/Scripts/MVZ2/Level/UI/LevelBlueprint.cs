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
            blueprint.OnPointerDown += (b) => OnPointerDown?.Invoke(this);
        }
        public event Action<LevelBlueprint> OnPointerDown;
        public Blueprint Blueprint => blueprint;
        [SerializeField]
        private Blueprint blueprint;
    }
}
