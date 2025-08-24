using MukioI18n;

namespace MVZ2Logic
{
    public static class LogicStrings
    {
        [TranslateMsg("实体名称-未知", CONTEXT_ENTITY_NAME)]
        public const string UNKNOWN_ENTITY_NAME = "？？？";

        [TranslateMsg("实体对策名称-未知", CONTEXT_ENTITY_COUNTER_NAME)]
        public const string UNKNOWN_ENTITY_COUNTER_NAME = "？？？";

        [TranslateMsg("实体说明-未知", CONTEXT_ENTITY_TOOLTIP)]
        public const string UNKNOWN_ENTITY_TOOLTIP = "？？？";

        [TranslateMsg("制品名称-未知", CONTEXT_ARTIFACT_NAME)]
        public const string UNKNOWN_ARTIFACT_NAME = "？？？";

        [TranslateMsg("制品说明-未知", CONTEXT_ARTIFACT_TOOLTIP)]
        public const string UNKNOWN_ARTIFACT_TOOLTIP = "？？？";

        [TranslateMsg("蓝图选项名称-未知", CONTEXT_OPTION_NAME)]
        public const string UNKNOWN_OPTION_NAME = "？？？";

        [TranslateMsg("死亡信息-未知", CONTEXT_DEATH_MESSAGE)]
        public const string DEATH_MESSAGE_UNKNOWN = "你死了！";

        [TranslateMsg("关卡难度", CONTEXT_DIFFICULTY)]
        public const string DIFFICULTY_UNKNOWN = "未知难度";

        public const string CONTEXT_DIFFICULTY = "difficulty";
        public const string CONTEXT_ENTITY_NAME = "entity.name";
        public const string CONTEXT_ENTITY_COUNTER_NAME = "entity_counter.name";
        public const string CONTEXT_ENTITY_TOOLTIP = "entity.tooltip";
        public const string CONTEXT_ARTIFACT_NAME = "artifact.name";
        public const string CONTEXT_ARTIFACT_TOOLTIP = "artifact.tooltip";
        public const string CONTEXT_DEATH_MESSAGE = "death_message";
        public const string CONTEXT_OPTION_NAME = "option.name";
    }
}
