using MukioI18n;

namespace MVZ2.Localization
{
    public static class StringTable
    {
        [TranslateMsg("通用的是")]
        public const string YES = "是";
        [TranslateMsg("通用的否")]
        public const string NO = "否";
        [TranslateMsg("通用的返回")]
        public const string BACK = "返回";
        [TranslateMsg("通用文本")]
        public const string RESTART = "重新开始";
        [TranslateMsg("通用文本")]
        public const string BACK_TO_MAP = "返回地图";
        [TranslateMsg("通用文本")]
        public const string BACK_TO_MAINMENU = "返回主菜单";
        [TranslateMsg("通用文本")]
        public const string ERROR = "错误";
        [TranslateMsg("通用文本")]
        public const string WARNING = "警告";
        [TranslateMsg("通用文本")]
        public const string QUIT = "退出";

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

        public const string CONTEXT_DEATH_MESSAGE = "death_message";
        [TranslateMsg("死亡信息-未知", CONTEXT_DEATH_MESSAGE)]
        public const string DEATH_MESSAGE_UNKNOWN = "你死了！";

        public const string CONTEXT_ERROR = "error";
        public const string CONTEXT_ENTITY_NAME = "entity.name";
        public const string CONTEXT_ENTITY_TOOLTIP = "entity.tooltip";
        public const string CONTEXT_LANGUAGE_NAME = "language_name";
    }
}
