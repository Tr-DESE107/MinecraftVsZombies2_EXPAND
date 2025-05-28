﻿using MVZ2Logic.Level.Components;
using PVZEngine.Level;

namespace MVZ2Logic.Level
{
    public static partial class LogicLevelExt
    {
        public static IBlueprintComponent GetBlueprintComponent(this LevelEngine level)
        {
            return level.GetComponent<IBlueprintComponent>();
        }
        public static void SetConveyorMode(this LevelEngine level, bool value)
        {
            var component = level.GetBlueprintComponent();
            component.SetConveyorMode(value);
        }
        public static bool IsConveyorMode(this LevelEngine level)
        {
            var component = level.GetBlueprintComponent();
            return component.IsConveyorMode();
        }
    }
}
