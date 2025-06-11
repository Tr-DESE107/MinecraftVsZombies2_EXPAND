﻿using UnityEngine;

namespace MVZ2.Models
{
    public class LightController : MonoBehaviour
    {
        public void SetRange(Vector2 scale)
        {
            this.scale = scale;
            transform.localScale = new Vector3(scale.x, scale.y);
        }
        public void SetColor(Color color)
        {
            this.color = color;
            lightRenderer.color = color;
        }
        private void Update()
        {
            UpdateLight();
        }
        private void UpdateLight()
        {
            var randomLightScale = 1 + Random.Range(-shakeRange, shakeRange);
            lightRenderer.transform.localScale = new Vector3(randomLightScale, randomLightScale);
        }
        public SerializableLightController ToSerializable()
        {
            return new SerializableLightController()
            {
                scale = scale,
                color = color,
            };
        }
        public void LoadFromSerializable(SerializableLightController serializable)
        {
            if (serializable == null)
                return;
            SetRange(serializable.scale);
            SetColor(serializable.color);
        }
        [SerializeField]
        private SpriteRenderer lightRenderer;
        [SerializeField]
        private float shakeRange = 0.05f;
        private Vector2 scale;
        private Color color;
    }
}
