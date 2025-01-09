using System;
using System.Collections.Generic;
using MVZ2.Level.Components;
using MVZ2.UI;
using MVZ2.Vanilla.Audios;
using MVZ2Logic.Level;
using MVZ2Logic.Level.Components;
using PVZEngine;
using PVZEngine.Level;
using UnityEngine;

namespace MVZ2.Level.Components
{
    public class BlueprintComponent : MVZ2Component, IBlueprintComponent
    {
        public BlueprintComponent(LevelEngine level, LevelController controller) : base(level, ID, controller)
        {
        }
        #region 传送带
        public void SetConveyorMode(bool mode)
        {
            isConveyorMode = mode;
            Controller.BlueprintController.SetUIConveyorMode(mode);
        }
        public bool IsConveyorMode()
        {
            return isConveyorMode;
        }
        #endregion

        public override ISerializableLevelComponent ToSerializable()
        {
            return new SerializableBlueprintComponent()
            {
                isConveyorMode = isConveyorMode
            };
        }
        public override void LoadSerializable(ISerializableLevelComponent seri)
        {
            base.LoadSerializable(seri);
            if (seri is not SerializableBlueprintComponent comp)
                return;
            SetConveyorMode(comp.isConveyorMode);
        }

        private bool isConveyorMode;
        public static readonly NamespaceID ID = new NamespaceID("mvz2", "blueprints");
    }
    [Serializable]
    public class SerializableBlueprintComponent : ISerializableLevelComponent
    {
        public bool isConveyorMode;
    }
}
