using System.Linq;
using MVZ2Logic.Conditions;
using PVZEngine;
using PVZEngine.Definitions;
using PVZEngine.Level;

namespace MVZ2Logic.Level
{
    [PropertyRegistryRegion(PropertyRegions.level)]
    public static class LogicStageProps
    {
        private static PropertyMeta<T> Get<T>(string name)
        {
            return new PropertyMeta<T>(name);
        }

        #region 模型预设
        public static readonly PropertyMeta<string> MODEL_PRESET = Get<string>("model_preset");
        public static string GetModelPreset(this StageDefinition stage) => stage.GetProperty<string>(MODEL_PRESET);
        public static void SetModelPreset(this StageDefinition stage, string value) => stage.SetProperty(MODEL_PRESET, value);
        #endregion

        #region 关卡类型
        public static readonly PropertyMeta<string> STAGE_TYPE = Get<string>("stage_type");
        public static string GetStageType(this StageDefinition stage) => stage.GetProperty<string>(STAGE_TYPE);
        public static void SetStageType(this StageDefinition stage, string value) => stage.SetProperty(STAGE_TYPE, value);
        #endregion

        #region 解锁条件
        public static readonly PropertyMeta<IConditionList> UNLOCK_CONDITIONS = Get<IConditionList>("unlock_conditions");
        public static IConditionList GetUnlockConditions(this StageDefinition stage) => stage.GetProperty<IConditionList>(UNLOCK_CONDITIONS);
        public static void SetUnlockConditions(this StageDefinition stage, IConditionList value) => stage.SetProperty(UNLOCK_CONDITIONS, value);
        #endregion

        #region 传送带卡池
        public static readonly PropertyMeta<IConveyorPoolEntry[]> CONVEYOR_POOL = Get<IConveyorPoolEntry[]>("conveyorPool");
        public static IConveyorPoolEntry[] GetConveyorPool(this LevelEngine game) => game.GetProperty<IConveyorPoolEntry[]>(CONVEYOR_POOL);
        #endregion

        #region 传送带速度
        public static readonly PropertyMeta<float> CONVEY_SPEED = Get<float>("conveySpeed");
        public static float GetConveySpeed(this LevelEngine game) => game.GetProperty<float>(CONVEY_SPEED);
        #endregion

        #region 对话
        public static readonly PropertyMeta<IStageTalkMeta[]> TALKS = Get<IStageTalkMeta[]>("talks");
        public static IStageTalkMeta[] GetTalks(this LevelEngine game) => game.GetProperty<IStageTalkMeta[]>(TALKS);
        public static IStageTalkMeta[] GetTalksOfType(this LevelEngine game, string type)
        {
            var talks = game.GetTalks();
            if (talks == null)
                return null;
            return talks.Where(t => t.Type == type).ToArray();
        }
        #endregion
    }
}
