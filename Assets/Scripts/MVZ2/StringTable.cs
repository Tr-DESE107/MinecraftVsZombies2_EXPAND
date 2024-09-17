using MukioI18n;

namespace MVZ2
{
    public static class StringTable
    {
        public const string CONTEXT_DIFFICULTY = "difficulty";
        [TranslateMsg("关卡难度", CONTEXT_DIFFICULTY)]
        public const string DIFFICULTY_EASY = "简单";
        [TranslateMsg("关卡难度", CONTEXT_DIFFICULTY)]
        public const string DIFFICULTY_NORMAL = "普通";
        [TranslateMsg("关卡难度", CONTEXT_DIFFICULTY)]
        public const string DIFFICULTY_HARD = "困难";
        [TranslateMsg("关卡难度", CONTEXT_DIFFICULTY)]
        public const string DIFFICULTY_UNKNOWN = "未知难度";

        public const string CONTEXT_LEVEL_NAME = "levelname";
        [TranslateMsg("关卡名称", CONTEXT_LEVEL_NAME)]
        public const string LEVEL_NAME_UNKNOWN = "未知关卡";
        [TranslateMsg("关卡名称", CONTEXT_LEVEL_NAME)]
        public const string LEVEL_NAME_PROLOGUE = "序章";
        [TranslateMsg("关卡名称", CONTEXT_LEVEL_NAME)]
        public const string LEVEL_NAME_HALLOWEEN = "万圣夜";

        [TranslateMsg("按钮文本")]
        public const string BUTTON_TEXT_BACK_TO_MAP = "返回地图";
        [TranslateMsg("按钮文本")]
        public const string BUTTON_TEXT_BACK_TO_MAINMENU = "返回主菜单";

        [TranslateMsg("游戏内文本提示")]
        public const string TOOLTIP_NOT_ENOUGH_ENERGY = "能量不足";
        [TranslateMsg("游戏内文本提示")]
        public const string TOOLTIP_RECHARGING = "重新充能中…";
        [TranslateMsg("游戏内文本提示")]
        public const string TOOLTIP_DIG_CONTRAPTION = "挖掉器械";

        public const string CONTEXT_ERROR = "error";
        public const string CONTEXT_DEATH_MESSAGE = "death_message";
        public const string CONTEXT_ENTITY_NAME = "entity.name";
        public const string CONTEXT_ENTITY_TOOLTIP = "entity.tooltip";
    }
}
