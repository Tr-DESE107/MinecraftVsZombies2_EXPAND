﻿using System;
using MVZ2.UI;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MVZ2.Store
{
    public class StoreProductItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public void UpdateItem(ProductItemViewData viewData)
        {
            root.SetActive(!viewData.empty);
            iconImage.sprite = viewData.icon;
            iconImage.enabled = !viewData.isBlueprint && iconImage.sprite;

            blueprintRoot.SetActive(viewData.isBlueprint);
            blueprintStandalone.gameObject.SetActive(!viewData.isBlueprintMobile);
            blueprintMobile.gameObject.SetActive(viewData.isBlueprintMobile);

            blueprintStandalone.UpdateView(viewData.blueprint);
            blueprintMobile.UpdateView(viewData.blueprint);

            iconText.text = viewData.text;
            priceText.text = viewData.price;
            button.interactable = viewData.interactable;
        }
        private void Awake()
        {
            button.onClick.AddListener(() => OnClick?.Invoke(this));
        }
        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
            OnPointerEnter?.Invoke(this);
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            OnPointerExit?.Invoke(this);
        }
        public event Action<StoreProductItem> OnClick;
        public event Action<StoreProductItem> OnPointerEnter;
        public event Action<StoreProductItem> OnPointerExit;
        [SerializeField]
        private Button button;
        [SerializeField]
        private GameObject root;
        [SerializeField]
        private Image iconImage;
        [SerializeField]
        private GameObject blueprintRoot;
        [SerializeField]
        private Blueprint blueprintStandalone;
        [SerializeField]
        private Blueprint blueprintMobile;
        [SerializeField]
        private TextMeshProUGUI iconText;
        [SerializeField]
        private TextMeshProUGUI priceText;
    }
    public struct ProductItemViewData
    {
        public bool empty;
        public Sprite icon;
        public BlueprintViewData blueprint;
        public bool isBlueprint;
        public bool isBlueprintMobile;
        public string text;
        public string price;
        public bool interactable;
        public static readonly ProductItemViewData Empty = new ProductItemViewData() { empty = true };
    }
}
