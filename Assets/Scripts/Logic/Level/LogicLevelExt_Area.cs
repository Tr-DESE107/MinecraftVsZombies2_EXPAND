﻿using MVZ2Logic.Level.Components;
using PVZEngine.Level;
using PVZEngine.Models;

namespace MVZ2Logic.Level
{
    public static partial class LogicLevelExt
    {
        public static IAreaComponent GetAreaComponent(this LevelEngine level)
        {
            return level.GetComponent<IAreaComponent>();
        }
        public static IModelInterface GetAreaModelInterface(this LevelEngine level)
        {
            var component = level.GetAreaComponent();
            return component.GetAreaModelInterface();
        }
    }
}
