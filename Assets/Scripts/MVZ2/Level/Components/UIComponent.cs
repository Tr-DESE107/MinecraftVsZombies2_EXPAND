using System;
using MVZ2Logic.Level.Components;
using PVZEngine;
using PVZEngine.Entities;
using PVZEngine.Level;
using UnityEngine;

namespace MVZ2.Level.Components
{
    public partial class UIComponent : MVZ2Component, IUIComponent
    {
        public UIComponent(LevelEngine level, LevelController controller) : base(level, componentID, controller)
        {
        }
        public Vector3 ScreenToLawnPositionByZ(Vector2 screenPosition, float z)
        {
            return Controller.ScreenToLawnPositionByZ(screenPosition, z);
        }
        public Vector3 ScreenToLawnPositionByY(Vector2 screenPosition, float y)
        {
            return Controller.ScreenToLawnPositionByY(screenPosition, y);
        }
        public Vector3 ScreenToLawnPositionByRelativeY(Vector2 screenPosition, float relativeY)
        {
            return Controller.ScreenToLawnPositionByRelativeY(screenPosition, relativeY);
        }
        public void ShakeScreen(float startAmplitude, float endAmplitude, int time)
        {
            Main.ShakeManager.AddShake(startAmplitude * Controller.LawnToTransScale, endAmplitude * Controller.LawnToTransScale, time / (float)Level.TPS);
        }
        public void ShowMoney()
        {
            Controller.ShowMoney();
        }
        public void SetMoneyFade(bool fade)
        {
            Controller.SetMoneyFade(fade);
        }
        public void SetEnergyActive(bool visible) => Controller.EnergyActive = visible;
        public bool IsEnergyActive() => Controller.EnergyActive;
        public void SetBlueprintsActive(bool visible) => Controller.BlueprintsActive = visible;
        public bool AreBlueprintsActive() => Controller.BlueprintsActive;
        public void SetPickaxeActive(bool visible) => Controller.PickaxeActive = visible;
        public bool IsPickaxeActive() => Controller.PickaxeActive;
        public void SetStarshardActive(bool visible) => Controller.StarshardActive = visible;
        public bool IsStarshardActive() => Controller.StarshardActive;
        public void SetTriggerActive(bool visible) => Controller.TriggerActive = visible;
        public bool IsTriggerActive() => Controller.TriggerActive;

        public void SetHintArrowPointToBlueprint(int index)
        {
            var levelUI = Controller.GetUIPreset();
            levelUI.SetHintArrowPointToBlueprint(index);
            TargetType = HintArrowTargetType.Blueprint;
            TargetID = index;
        }
        public void SetHintArrowPointToPickaxe()
        {
            var levelUI = Controller.GetUIPreset();
            levelUI.SetHintArrowPointToPickaxe();
            TargetType = HintArrowTargetType.Pickaxe;
            TargetID = 0;
        }
        public void SetHintArrowPointToTrigger()
        {
            var levelUI = Controller.GetUIPreset();
            levelUI.SetHintArrowPointToTrigger();
            TargetType = HintArrowTargetType.Trigger;
            TargetID = 0;
        }
        public void SetHintArrowPointToStarshard()
        {
            var levelUI = Controller.GetUIPreset();
            levelUI.SetHintArrowPointToStarshard();
            TargetType = HintArrowTargetType.Starshard;
            TargetID = 0;
        }
        public void SetHintArrowPointToEntity(Entity entity)
        {
            var levelUI = Controller.GetUIPreset();
            var entityCtrl = Controller.GetEntityController(entity);
            levelUI.SetHintArrowPointToEntity(entityCtrl.transform, entity.GetScaledSize().y);
            TargetType = HintArrowTargetType.Entity;
            TargetID = entity.ID;
        }
        public void HideHintArrow()
        {
            var levelUI = Controller.GetUIPreset();
            levelUI.HideHintArrow();
            TargetType = HintArrowTargetType.None;
            TargetID = 0;
        }
        public void PauseGame(int level = 0)
        {
            Controller.PauseGame(level);
        }
        public void ResumeGame(int level = 0)
        {
            Controller.ResumeGame(level);
        }
        public void SetUIAndInputDisabled(bool disabled)
        {
            Controller.SetUIAndInputDisabled(disabled);
        }
        public void ShowDialog(string title, string desc, string[] options, Action<int> onSelect = null)
        {
            Main.Scene.ShowDialog(title, desc, options, onSelect);
        }
        public override ISerializableLevelComponent ToSerializable()
        {
            return new SerializableUIComponent()
            {
                targetType = (int)TargetType,
                targetID = TargetID
            };
        }
        public override void LoadSerializable(ISerializableLevelComponent seri)
        {
            if (seri is not SerializableUIComponent comp)
                return;
            TargetType = (HintArrowTargetType)comp.targetType;
            TargetID = comp.targetID;
        }
        public override void PostLevelLoad()
        {
            base.PostLevelLoad();
            switch (TargetType)
            {
                case HintArrowTargetType.Entity:
                    {
                        if (TargetID <= 0)
                            break;
                        var entity = Level.FindEntityByID(TargetID);
                        if (entity == null)
                            break;
                        SetHintArrowPointToEntity(entity);
                    }
                    break;
                case HintArrowTargetType.Blueprint:
                    {
                        var index = (int)TargetID;
                        if (index < 0 || index >= Level.GetSeedSlotCount())
                            break;
                        SetHintArrowPointToBlueprint(index);
                    }
                    break;
                case HintArrowTargetType.Pickaxe:
                    {
                        SetHintArrowPointToPickaxe();
                    }
                    break;
                case HintArrowTargetType.Starshard:
                    {
                        SetHintArrowPointToStarshard();
                    }
                    break;
            }
        }
        public void SetProgressBarToBoss(NamespaceID barStyle)
        {
            Controller.SetProgressToBoss(barStyle);
        }
        public void SetProgressBarToStage()
        {
            Controller.SetProgressToStage();
        }
        public void SetAreaModelPreset(string preset)
        {
            Controller.SetModelPreset(preset);
        }
        public void TriggerModelAnimator(string name)
        {
            Controller.TriggerModelAnimator(name);
        }
        public void SetModelAnimatorBool(string name, bool value)
        {
            Controller.SetModelAnimatorBool(name, value);
        }
        public void SetModelAnimatorInt(string name, int value)
        {
            Controller.SetModelAnimatorInt(name, value);
        }
        public void SetModelAnimatorFloat(string name, float value)
        {
            Controller.SetModelAnimatorFloat(name, value);
        }
        public void UpdateLevelName()
        {
            Controller.UpdateLevelName();
        }
        public void FlickerEnergy()
        {
            Controller.FlickerEnergy();
        }
        public HintArrowTargetType TargetType { get; private set; }
        public long TargetID { get; private set; }
        public static readonly NamespaceID componentID = new NamespaceID(Vanilla.VanillaMod.spaceName, "ui");
    }
    [Serializable]
    public class SerializableUIComponent : ISerializableLevelComponent
    {
        public int targetType;
        public long targetID;
    }
    public enum HintArrowTargetType
    {
        None,
        Blueprint,
        Pickaxe,
        Trigger,
        Entity,
        Starshard,
    }
}