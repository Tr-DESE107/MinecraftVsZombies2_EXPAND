using MukioI18n;

namespace MVZ2.Vanilla
{
    public static class VanillaStrings
    {
        [TranslateMsg("教程无法使用提示")]
        public const string TOOLTIP_DISABLE_MESSAGE = "无法使用";

        [TranslateMsg("不能在雕像上放置器械提示", CONTEXT_ADVICE_ERROR)]
        public const string ADVICE_CANNOT_PLACE_ON_STATUES = "你不能在雕像上放置器械";

        [TranslateMsg]
        public const string UI_PURCHASE = "购买";
        [TranslateMsg]
        public const string UI_CONFIRM_BUY_7TH_SLOT = "购买第七个器械槽位？";
        [TranslateMsg]
        public const string UI_CONFIRM_TUTORIAL = "是否进行新手教程？";
        [TranslateMsg]
        public const string UI_TUTORIAL = "新手教程";


        [TranslateMsg("冷却时间", CONTEXT_RECHARGE_TIME)]
        public const string RECHARGE_NONE = "无";
        [TranslateMsg("冷却时间", CONTEXT_RECHARGE_TIME)]
        public const string RECHARGE_SHORT = "短";
        [TranslateMsg("冷却时间", CONTEXT_RECHARGE_TIME)]
        public const string RECHARGE_LONG = "长";
        [TranslateMsg("冷却时间", CONTEXT_RECHARGE_TIME)]
        public const string RECHARGE_VERY_LONG = "很长";

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
        [TranslateMsg("通用文本")]
        public const string CONTINUE = "继续";

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
        [TranslateMsg("关卡名称，{0}为关卡名，{1}为冒险模式天数", CONTEXT_LEVEL_NAME)]
        public const string LEVEL_NAME_DAY_TEMPLATE = "{0} - 第{1}天";

        public const string CONTEXT_DEATH_MESSAGE = "death_message";
        [TranslateMsg("死亡信息-未知", CONTEXT_DEATH_MESSAGE)]
        public const string DEATH_MESSAGE_UNKNOWN = "你死了！";

        [TranslateMsg("实体名称-未知", CONTEXT_ENTITY_NAME)]
        public const string UNKNOWN_ENTITY_NAME = "？？？";

        [TranslateMsg("实体说明-未知", CONTEXT_ENTITY_TOOLTIP)]
        public const string UNKNOWN_ENTITY_TOOLTIP = "？？？";
        public static string GetAlmanacNameContext(string category)
        {
            return $"{category}.name";
        }
        public static string GetAlmanacDescriptionContext(string category)
        {
            return $"{category}.description";
        }
        public const string CONTEXT_ADVICE_ERROR = "advice.error";
        public const string CONTEXT_ERROR = "error";
        public const string CONTEXT_ENTITY_NAME = "entity.name";
        public const string CONTEXT_ENTITY_TOOLTIP = "entity.tooltip";
        public const string CONTEXT_ALMANAC = "almanac";
        public const string CONTEXT_ALMANAC_GROUP_NAME = "almanac.group_name";
        public const string CONTEXT_LANGUAGE_NAME = "language_name";
        public const string CONTEXT_RECHARGE_TIME = "recharge_time";
    }
}
