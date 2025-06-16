﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using MVZ2.UI;
using MVZ2Logic.Level;
using PVZEngine.Level;
using UnityEngine;

namespace MVZ2.Level
{
    public interface ILevelBlueprintController : ILevelControllerPart
    {
        BlueprintController GetCurrentBlueprintControllerByIndex(int index);
        void DestroyClassicBlueprintAt(int index);
        void DestroyConveyorBlueprintAt(int index);
        float GetConveyorSpeed();
        void SetUIConveyorMode(bool conveyor);
        ConveyorBlueprintController GetConveyorBlueprintController(int index);
        void SetConveyorBlueprintUIPosition(int index, float position);
    }
    public class LevelBlueprintController : LevelControllerPart, ILevelBlueprintController
    {
        public void ForceUpdateBlueprintHotkeyTexts()
        {
            foreach (var blueprint in classicBlueprints)
            {
                if (blueprint == null)
                    continue;
                blueprint.ForceUpdateBlueprintHotkeyText();
            }
            foreach (var blueprint in conveyorBlueprints)
            {
                if (blueprint == null)
                    continue;
                blueprint.ForceUpdateBlueprintHotkeyText();
            }
        }
        #region 引擎层

        public override void AddEngineCallbacks(LevelEngine level)
        {
            base.AddEngineCallbacks(level);

            level.OnSeedAdded += Engine_OnSeedAddedCallback;
            level.OnSeedRemoved += Engine_OnSeedRemovedCallback;
            level.OnSeedSlotCountChanged += Engine_OnSeedPackCountChangedCallback;

            level.OnConveyorSeedAdded += Engine_OnConveyorSeedPackAddedCallback;
            level.OnConveyorSeedRemoved += Engine_OnConveyorSeedPackRemovedCallback;
            level.OnConveyorSeedSlotCountChanged += Engine_OnConveyorSeedPackCountChangedCallback;
        }
        public override void RemoveEngineCallbacks(LevelEngine level)
        {
            base.RemoveEngineCallbacks(level);

            level.OnSeedAdded -= Engine_OnSeedAddedCallback;
            level.OnSeedRemoved -= Engine_OnSeedRemovedCallback;
            level.OnSeedSlotCountChanged -= Engine_OnSeedPackCountChangedCallback;

            level.OnConveyorSeedAdded -= Engine_OnConveyorSeedPackAddedCallback;
            level.OnConveyorSeedRemoved -= Engine_OnConveyorSeedPackRemovedCallback;
            level.OnConveyorSeedSlotCountChanged -= Engine_OnConveyorSeedPackCountChangedCallback;
        }

        #region 事件回调
        private void Engine_OnSeedAddedCallback(int index)
        {
            CreateClassicSeedController(index);
        }
        private void Engine_OnSeedRemovedCallback(int index)
        {
            DestroyClassicSeedController(index);
        }
        private void Engine_OnSeedPackCountChangedCallback(int count)
        {
            UpdateUIClassicBlueprintCount();
        }
        private void Engine_OnConveyorSeedPackAddedCallback(int index)
        {
            CreateConveyorSeedController(index);
        }
        private void Engine_OnConveyorSeedPackRemovedCallback(int index)
        {
            DestroyConveyorSeedController(index);
        }
        private void Engine_OnConveyorSeedPackCountChangedCallback(int count)
        {
            UpdateUIConveyorBlueprintCount();
        }
        #endregion

        #endregion

        #region 经典模式控制器
        public ClassicBlueprintController GetClassicBlueprintController(int index)
        {
            if (index < 0 || index >= classicBlueprints.Length)
                return null;
            return classicBlueprints[index];
        }
        private ClassicBlueprintController CreateClassicSeedController(int index)
        {
            var seed = Level.GetSeedPackAt(index);
            if (seed == null)
                return null;

            Blueprint classicBlueprint = UI.Blueprints.CreateClassicBlueprint();
            UI.Blueprints.InsertClassicBlueprint(index, classicBlueprint);
            UI.Blueprints.ForceAlignBlueprint(index);

            var controller = new ClassicBlueprintController(Controller, classicBlueprint, index, seed);
            controller.Init();
            classicBlueprints[index] = controller;
            return controller;
        }
        private void DestroyClassicSeedController(int index)
        {
            var controller = GetClassicBlueprintController(index);
            if (controller == null)
                return;
            controller.Destroy();
            classicBlueprints[index] = null;
        }
        #endregion

        #region 传送带模式控制器
        public ConveyorBlueprintController GetConveyorBlueprintController(int index)
        {
            if (index < 0 || index >= conveyorBlueprints.Count)
                return null;
            return conveyorBlueprints[index];
        }
        private ConveyorBlueprintController CreateConveyorSeedController(int index)
        {
            var seed = Level.GetConveyorSeedPackAt(index);
            if (seed == null)
                return null;

            Blueprint conveyorBlueprint = UI.Blueprints.ConveyBlueprint();
            UI.Blueprints.InsertConveyorBlueprint(index, conveyorBlueprint);

            for (int i = index; i < conveyorBlueprints.Count; i++)
            {
                conveyorBlueprints[i].Index++;
            }
            var controller = new ConveyorBlueprintController(Controller, conveyorBlueprint, index, seed);
            controller.Init();
            conveyorBlueprints.Insert(index, controller);
            return controller;
        }
        private void DestroyConveyorSeedController(int index)
        {
            var controller = GetConveyorBlueprintController(index);
            if (controller == null)
                return;
            controller.Destroy();
            conveyorBlueprints.RemoveAt(index);
            for (int i = index; i < conveyorBlueprints.Count; i++)
            {
                conveyorBlueprints[i].Index--;
            }
        }
        #endregion

        public BlueprintController GetCurrentBlueprintControllerByIndex(int index)
        {
            if (Level.IsConveyorMode())
            {
                return GetConveyorBlueprintController(index);
            }
            else
            {
                return GetClassicBlueprintController(index);
            }
        }

        #region 更新

        public override void UpdateLogic()
        {
            base.UpdateLogic();
            foreach (var blueprint in classicBlueprints)
            {
                if (blueprint == null)
                    continue;
                blueprint.UpdateFixed();
            }
            foreach (var blueprint in conveyorBlueprints)
            {
                if (blueprint == null)
                    continue;
                blueprint.UpdateFixed();
            }
        }
        public override void UpdateFrame(float deltaTime, float simulationSpeed)
        {
            base.UpdateFrame(deltaTime, simulationSpeed);
            foreach (var blueprint in classicBlueprints)
            {
                if (blueprint == null)
                    continue;
                blueprint.UpdateFrame(deltaTime * simulationSpeed);
            }
            foreach (var blueprint in conveyorBlueprints)
            {
                if (blueprint == null)
                    continue;
                blueprint.UpdateFrame(deltaTime * simulationSpeed);
            }
        }

        #endregion

        protected override SerializableLevelControllerPart GetSerializable()
        {
            return new SerializableLevelBlueprintController()
            {
                classicBlueprints = classicBlueprints.Select(b => b == null ? null : b.ToSerializable()).ToArray(),
                conveyorBlueprints = conveyorBlueprints.Select(b => b == null ? null : b.ToSerializable()).ToArray(),
            };
        }
        public override void LoadFromSerializable(SerializableLevelControllerPart seri)
        {
            if (seri is not SerializableLevelBlueprintController serializable)
                return;
            UpdateUIClassicBlueprintCount();
            var seedPacks = Level.GetAllSeedPacks();
            classicBlueprints = new ClassicBlueprintController[seedPacks.Length];
            for (int i = 0; i < seedPacks.Length; i++)
            {
                var controller = CreateClassicSeedController(i);
                if (controller == null)
                    continue;
                var seriSeed = serializable.classicBlueprints[i];
                if (seriSeed == null)
                    throw new SerializationException($"Could not find classic blueprint data at index {i} in the level state data.");
                controller.LoadFromSerializable(seriSeed);
                controller.UpdateFrame(0);
            }
            UpdateUIConveyorBlueprintCount();
            var conveyorSeedPacks = Level.GetAllConveyorSeedPacks();
            for (int i = 0; i < conveyorSeedPacks.Length; i++)
            {
                var controller = CreateConveyorSeedController(i);
                if (controller == null)
                    continue;
                var seriSeed = serializable.conveyorBlueprints[i];
                if (seriSeed == null)
                    throw new SerializationException($"Could not find conveyor blueprint data at index {i} in the level state data.");
                controller.LoadFromSerializable(seriSeed);
                controller.UpdateFrame(0);
            }
        }

        #region 传送带
        public void SetUIConveyorMode(bool mode)
        {
            UI.Blueprints.SetConveyorMode(mode);
            UpdateUIClassicBlueprintCount();
            UpdateUIConveyorBlueprintCount();
        }
        public float GetConveyorSpeed()
        {
            return conveyorSpeed;
        }
        #endregion

        #region UI层

        #region 经典模式
        private void UpdateUIClassicBlueprintCount()
        {
            var count = Level.GetSeedSlotCount();
            Array.Resize(ref classicBlueprints, count);
            UI.Blueprints.SetClassicBlueprintSlotCount(count);
        }
        public void DestroyClassicBlueprintAt(int index)
        {
            UI.Blueprints.DestroyClassicBlueprintAt(index);
        }

        #endregion

        #region 传送带模式
        public void DestroyConveyorBlueprintAt(int index)
        {
            UI.Blueprints.DestroyConveyorBlueprintAt(index);
        }
        public void SetConveyorBlueprintUIPosition(int index, float position)
        {
            UI.Blueprints.SetConveyorBlueprintNormalizedPosition(index, position);
        }
        private void UpdateUIConveyorBlueprintCount()
        {
            UI.Blueprints.SetConveyorBlueprintSlotCount(Level.GetConveyorSlotCount());
        }
        #endregion

        #endregion

        [SerializeField]
        private float conveyorSpeed = 1;
        private ClassicBlueprintController[] classicBlueprints = Array.Empty<ClassicBlueprintController>();
        private List<ConveyorBlueprintController> conveyorBlueprints = new List<ConveyorBlueprintController>();
    }
    [Serializable]
    public class SerializableLevelBlueprintController : SerializableLevelControllerPart
    {
        public SerializableBlueprintController[] classicBlueprints;
        public SerializableBlueprintController[] conveyorBlueprints;
    }
    public interface ILevelBlueprintRuntimeUI
    {
        void SetBlueprintsActive(bool active);
        void SetConveyorMode(bool mode);
        Blueprint CreateClassicBlueprint();
        void InsertClassicBlueprint(int index, Blueprint blueprint);
        void SetClassicBlueprintSlotCount(int count);
        void DestroyClassicBlueprintAt(int index);
        void ForceAlignBlueprint(int index);

        Blueprint ConveyBlueprint();
        void InsertConveyorBlueprint(int index, Blueprint blueprint);
        void SetConveyorBlueprintSlotCount(int count);
        void DestroyConveyorBlueprintAt(int index);
        void SetConveyorBlueprintNormalizedPosition(int index, float position);
    }

    public interface ILevelUI
    {
        void SetReceiveRaycasts(bool receive);
        void SetBlueprintsSortingToChoosing(bool choosing);
        ILevelBlueprintRuntimeUI Blueprints { get; }
        LevelUIBlueprintChoose BlueprintChoose { get; }
    }
}
