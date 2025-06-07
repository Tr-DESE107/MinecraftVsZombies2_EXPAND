﻿using UnityEngine;
using UnityEngine.UI;

namespace MVZ2.UI
{
    public class AlmanacTagIconLayer : MonoBehaviour
    {
        public void UpdateView(AlmanacTagIconLayerViewData viewData)
        {
            image.sprite = viewData.sprite;
            image.enabled = image.sprite;
            image.color = viewData.tint;
        }
        [SerializeField]
        private Image image;
    }
    public struct AlmanacTagIconLayerViewData
    {
        public Sprite sprite;
        public Color tint;
    }
}
