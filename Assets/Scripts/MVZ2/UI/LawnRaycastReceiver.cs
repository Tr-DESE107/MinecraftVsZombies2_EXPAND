﻿using System;
using MVZ2.HeldItems;
using MVZ2.Level;
using MVZ2.Managers;
using MVZ2Logic;
using MVZ2Logic.HeldItems;
using PVZEngine.Level;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MVZ2.UI
{
    public class LawnRaycastReceiver : MonoBehaviour, ILevelRaycastReceiver
    {
        public LawnArea GetArea()
        {
            return area;
        }
        private void Awake()
        {
            holdStreakHandler.OnPointerInteraction += (_, d, i) => OnPointerInteraction?.Invoke(this, d, i);
        }
        bool ILevelRaycastReceiver.IsValidReceiver(LevelEngine level, HeldItemDefinition definition, IHeldItemData data, PointerEventData eventData)
        {
            if (definition == null)
                return false;
            var target = new HeldItemTargetLawn(level, area);
            var pointer = InputManager.GetPointerDataFromEventData(eventData);
            return definition.IsValidFor(target, data, pointer);
        }
        int ILevelRaycastReceiver.GetSortingLayer()
        {
            return canvas.sortingLayerID;
        }
        int ILevelRaycastReceiver.GetSortingOrder()
        {
            return canvas.sortingOrder;
        }
        public event Action<LawnRaycastReceiver, PointerEventData, PointerInteraction> OnPointerInteraction;
        [SerializeField]
        private Canvas canvas;
        [SerializeField]
        private LawnArea area;
        [SerializeField]
        private LevelPointerInteractionHandler holdStreakHandler;
    }
}
