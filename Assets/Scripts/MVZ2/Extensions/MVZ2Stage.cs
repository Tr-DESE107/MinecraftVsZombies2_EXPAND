using System;
using MVZ2.GameContent;
using MVZ2.Level;
using MVZ2.Level.Components;
using MVZ2.Managers;
using MVZ2.Vanilla;
using PVZEngine.Definitions;
using PVZEngine.Level;
using UnityEngine;

namespace MVZ2.Extensions
{
    public static class MVZ2Stage
    {
        public static string GetStartTransition(this StageDefinition stage)
        {
            return stage.GetProperty<string>(BuiltinStageProps.START_TRANSITION);
        }
        public static LevelCameraPosition GetStartCameraPosition(this StageDefinition stage)
        {
            return (LevelCameraPosition)stage.GetProperty<int>(BuiltinStageProps.START_CAMERA_POSITION);
        }
    }
}
