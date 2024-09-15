using System;
using log4net.Core;
using MVZ2;
using MVZ2.GameContent;
using MVZ2.Level;
using PVZEngine.Definitions;

namespace PVZEngine.Level
{
    public partial class UIComponent : MVZ2Component
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
            var levelUI = Controller.GetLevelUI();
            levelUI.ResetMoneyFadeTime();
        }
        public void SetHintArrowPointToBlueprint(int index)
        {
            var levelUI = Controller.GetLevelUI();
            levelUI.SetHintArrowPointToBlueprint(index);
        }
        public void SetHintArrowPointToPickaxe()
        {
            var levelUI = Controller.GetLevelUI();
            levelUI.SetHintArrowPointToPickaxe();
        }
        public void SetHintArrowPointToEntity(Entity entity)
        {
            var levelUI = Controller.GetLevelUI();
            var entityCtrl = Controller.GetEntityController(entity);
            levelUI.SetHintArrowPointToEntity(entityCtrl.transform, entity.GetScaledSize().y);
        }
        public void HideHintArrow()
        {
            var levelUI = Controller.GetLevelUI();
            levelUI.HideHintArrow();
        }
        public static readonly NamespaceID componentID = new NamespaceID(Builtin.spaceName, "ui");
    }
}