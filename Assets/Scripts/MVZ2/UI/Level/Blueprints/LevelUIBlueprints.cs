﻿using System;
using MVZ2.Level;
using MVZ2.Level.UI;
using MVZ2Logic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MVZ2.UI
{
    public class LevelUIBlueprints : MonoBehaviour, ILevelBlueprintRuntimeUI
    {
        public void SetConveyorMode(bool value)
        {
            isConveyor = value;
            foreach (var classic in blueprintClassicModeObjects)
            {
                classic.SetActive(!value);
            }
            foreach (var conveyor in blueprintConveyorModeObjects)
            {
                conveyor.SetActive(value);
            }
        }
        public bool IsConveyorMode()
        {
            return isConveyor;
        }
        public Blueprint GetCurrentModeBlueprint(int index)
        {
            return isConveyor ? GetConveyorBlueprintAt(index) : GetClassicBlueprintAt(index);
        }
        public void SetSortingToChoosing(bool choosing)
        {
            var layer = choosing ? choosingSortingLayer : battleSortingLayer;
            foreach (var canvas in blueprintSortingCanvases)
            {
                canvas.sortingLayerID = layer.id;
            }
        }
        private void Awake()
        {
            blueprints.OnBlueprintPointerInteraction += OnBlueprintPointerInteractionCallback;
            conveyor.OnBlueprintPointerInteraction += OnConveyorPointerInteractionCallback;
        }
        private void OnBlueprintPointerInteractionCallback(int index, PointerEventData eventData, PointerInteraction interaction)
        {
            OnBlueprintPointerInteraction?.Invoke(index, eventData, interaction, false);
        }
        private void OnConveyorPointerInteractionCallback(int index, PointerEventData eventData, PointerInteraction interaction)
        {
            OnBlueprintPointerInteraction?.Invoke(index, eventData, interaction, true);
        }

        #region 经典模式蓝图
        public void SetBlueprintsActive(bool visible)
        {
            blueprintClassicEnabledObj.SetActive(visible);
            blueprintConveyorEnabledObj.SetActive(visible);
        }
        public void SetClassicBlueprintSlotCount(int count)
        {
            blueprints.SetSlotCount(count);
        }
        public Blueprint CreateClassicBlueprint()
        {
            return blueprints.CreateBlueprint();
        }
        public void InsertClassicBlueprint(int index, Blueprint blueprint)
        {
            blueprints.InsertBlueprint(index, blueprint);
        }
        public bool RemoveClassicBlueprint(Blueprint blueprint)
        {
            return blueprints.RemoveBlueprint(blueprint);
        }
        public void RemoveClassicBlueprintAt(int index)
        {
            blueprints.RemoveBlueprintAt(index);
        }
        public bool DestroyClassicBlueprint(Blueprint blueprint)
        {
            return blueprints.DestroyBlueprint(blueprint);
        }
        public void DestroyClassicBlueprintAt(int index)
        {
            blueprints.DestroyBlueprintAt(index);
        }
        public Blueprint GetClassicBlueprintAt(int index)
        {
            return blueprints.GetBlueprintAt(index);
        }
        public int GetClassicBlueprintIndex(Blueprint blueprint)
        {
            return blueprints.GetBlueprintIndex(blueprint);
        }
        public void ForceAlignBlueprint(int index)
        {
            blueprints.ForceAlign(index);
        }
        #endregion

        #region 传送带模式蓝图
        public Vector3 GetClassicBlueprintPosition(int index)
        {
            return blueprints.GetBlueprintPosition(index);
        }
        public Blueprint ConveyBlueprint()
        {
            return conveyor.CreateBlueprint();
        }
        public void InsertConveyorBlueprint(int index, Blueprint blueprint)
        {
            conveyor.InsertBlueprint(index, blueprint);
        }
        public bool DestroyConveyorBlueprint(Blueprint blueprint)
        {
            return conveyor.DestroyBlueprint(blueprint);
        }
        public void DestroyConveyorBlueprintAt(int index)
        {
            conveyor.DestroyBlueprintAt(index);
        }
        public Blueprint GetConveyorBlueprintAt(int index)
        {
            return conveyor.GetBlueprintAt(index);
        }
        public int GetConveyorBlueprintIndex(Blueprint blueprint)
        {
            return conveyor.GetBlueprintIndex(blueprint);
        }
        public void SetConveyorBlueprintSlotCount(int count)
        {
            conveyor.SetSlotCount(count);
        }
        public void SetConveyorBlueprintNormalizedPosition(int index, float position)
        {
            conveyor.SetBlueprintNormalizedPosition(index, position);
        }
        #endregion

        public event Action<int, PointerEventData, PointerInteraction, bool> OnBlueprintPointerInteraction;

        private bool isConveyor;
        [SerializeField]
        GameObject blueprintClassicEnabledObj;
        [SerializeField]
        GameObject blueprintConveyorEnabledObj;
        [SerializeField]
        GameObject[] blueprintClassicModeObjects;
        [SerializeField]
        GameObject[] blueprintConveyorModeObjects;

        [Header("Sorting")]
        [SerializeField]
        Canvas[] blueprintSortingCanvases;
        [SerializeField]
        SortingLayerPicker battleSortingLayer;
        [SerializeField]
        SortingLayerPicker choosingSortingLayer;

        [Header("Blueprints")]
        [SerializeField]
        BlueprintArray blueprints;
        [SerializeField]
        Conveyor conveyor;
    }
}
