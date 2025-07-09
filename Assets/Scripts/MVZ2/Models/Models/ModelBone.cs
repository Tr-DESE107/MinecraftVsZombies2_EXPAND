﻿using UnityEngine;

namespace MVZ2.Models
{
    [DisallowMultipleComponent]
    public sealed class ModelBone : MonoBehaviour
    {
        public void SetLightVisible(bool visible)
        {
            if (lightController)
            {
                lightController.gameObject.SetActive(visible);
            }
        }
        public void SetLightColor(Color color)
        {
            if (lightController)
            {
                lightController.SetColor(color);
            }
        }
        public void SetLightRange(Vector2 range)
        {
            if (lightController)
            {
                lightController.SetRange(range);
            }
        }
        [Header("Light")]
        [SerializeField]
        private LightController lightController;
    }
}