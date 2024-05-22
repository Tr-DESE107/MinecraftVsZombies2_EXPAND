using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MVZ2.Level.UI
{
    public class HeldItem : MonoBehaviour
    {
        public void SetIcon(Sprite sprite)
        {
            spriteRenderer.sprite = sprite;
        }
        [SerializeField]
        private SpriteRenderer spriteRenderer;
    }
}
