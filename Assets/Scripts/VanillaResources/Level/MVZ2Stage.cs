using PVZEngine.Definitions;

namespace MVZ2Logic.Level
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
