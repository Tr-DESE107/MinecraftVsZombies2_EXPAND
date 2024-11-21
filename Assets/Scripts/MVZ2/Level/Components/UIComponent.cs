using System;
using MVZ2.GameContent;
using PVZEngine;
using PVZEngine.Entities;
using PVZEngine.Level;

namespace MVZ2.Level.Components
{
    public partial class UIComponent : MVZ2Component, IUIComponent
    {
        public UIComponent(LevelEngine level, LevelController controller) : base(level, componentID, controller)
        {
        }
        public void ShakeScreen(float startAmplitude, float endAmplitude, int time)
        {
            Main.ShakeManager.AddShake(startAmplitude * Controller.LawnToTransScale, endAmplitude * Controller.LawnToTransScale, time / (float)Level.TPS);
        }
        public void ShowDialog(string title, string desc, string[] options, Action<int> onSelect)
        {
            Main.Scene.ShowDialog(title, desc, options, onSelect);
        }
        public void ShowMoney()
        {
            Controller.ShowMoney();
        }
        public void SetBlueprintsActive(bool visible)
        {
            Controller.BlueprintsActive = visible;
        }
        public void SetPickaxeActive(bool visible)
        {
            Controller.PickaxeActive = visible;
        }
        public void SetStarshardActive(bool visible)
        {
            Controller.StarshardActive = visible;
        }
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
        public HintArrowTargetType TargetType { get; private set; }
        public long TargetID { get; private set; }
        public static readonly NamespaceID componentID = new NamespaceID(Builtin.spaceName, "ui");
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
        Entity,
        Starshard,
    }
}