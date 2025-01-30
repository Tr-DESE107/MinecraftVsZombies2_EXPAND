using MukioI18n;
using PVZEngine;

namespace MVZ2.Vanilla
{
    public static class VanillaStrings
    {
        [TranslateMsg("教程无法使用提示")]
        public const string TOOLTIP_DISABLE_MESSAGE = "无法使用";
        [TranslateMsg("教程无法使用提示")]
        public const string TOOLTIP_DECREPIFY = "受到衰老诅咒！";

        [TranslateMsg("不能放置器械提示", CONTEXT_ADVICE)]
        public const string ADVICE_CANNOT_PLACE_ON_STATUES = "你不能在雕像上放置器械";
        [TranslateMsg("不能放置器械提示", CONTEXT_ADVICE)]
        public const string ADVICE_PLACE_LILYPAD_FIRST = "请先放睡莲";
        [TranslateMsg("不能放置器械提示", CONTEXT_ADVICE)]
        public const string ADVICE_CANNOT_PLACE_ON_WATER = "你不能在水上放置该器械";
        [TranslateMsg("不能放置器械提示", CONTEXT_ADVICE)]
        public const string ADVICE_CANNOT_PLACE_ON_PLANE = "你不能在薄面上放置该器械";
        [TranslateMsg("不能放置器械提示", CONTEXT_ADVICE)]
        public const string ADVICE_CANNOT_PLACE_ON_LAND = "你不能在陆地上放置该器械";
        [TranslateMsg("不能放置器械提示", CONTEXT_ADVICE)]
        public const string ADVICE_ONLY_PLACE_ON_CAN_SLEEP = "只能放置在可以睡觉的器械上";
        [TranslateMsg("不能放置器械提示", CONTEXT_ADVICE)]
        public const string ADVICE_ONLY_UPGRADE = "只能放置在目标器械上";
        [TranslateMsg("梦魇战斗提示", CONTEXT_ADVICE)]
        public const string ADVICE_CLICK_TO_DRAG_CRUSHING_WALLS = "点击屏幕阻止碾压墙！";
        [TranslateMsg("获得新制品提示", CONTEXT_ADVICE)]
        public const string ADVICE_YOU_FOUND_A_NEW_ARTIFACT = "你找到了新制品！";

        [TranslateMsg("无尽模式的提示", CONTEXT_ADVICE)]
        public const string ADVICE_MORE_ENEMIES_APPROACHING = "更多的敌人要来了！";
        [TranslateMsg("预览战场的提示", CONTEXT_ADVICE)]
        public const string ADVICE_CLICK_TO_CONTINUE = "点击以继续";

        [TranslateMsg("不推荐使用的提示")]
        public const string NOT_RECOMMONEDED_IN_LEVEL = "不推荐在这关使用";

        [TranslateMsg]
        public const string UI_PURCHASE = "购买";
        [TranslateMsg]
        public const string UI_CONFIRM_BUY_7TH_SLOT = "购买第七个器械槽位？";
        [TranslateMsg]
        public const string UI_CONFIRM_TUTORIAL = "是否进行新手教程？";
        [TranslateMsg]
        public const string UI_TUTORIAL = "新手教程";

        [TranslateMsg("输入名称对话框的错误信息")]
        public const string ERROR_MESSAGE_NAME_EMPTY = "用户名不能为空";
        [TranslateMsg("输入名称对话框的错误信息")]
        public const string ERROR_MESSAGE_NAME_DUPLICATE = "已经存在该用户名";
        [TranslateMsg("输入名称对话框的错误信息")]
        public const string ERROR_MESSAGE_CANNOT_CANCEL_NAME_INPUT = "第一次游戏必须输入用户名";
        [TranslateMsg("删除用户时的警告，{0}为名称")]
        public const string WARNING_DELETE_USER = "确认删除用户{0}吗？\n该用户所有的数据都将被删除！";


        [TranslateMsg("冷却时间", CONTEXT_RECHARGE_TIME)]
        public const string RECHARGE_NONE = "无";
        [TranslateMsg("冷却时间", CONTEXT_RECHARGE_TIME)]
        public const string RECHARGE_SHORT = "短";
        [TranslateMsg("冷却时间", CONTEXT_RECHARGE_TIME)]
        public const string RECHARGE_LONG = "长";
        [TranslateMsg("冷却时间", CONTEXT_RECHARGE_TIME)]
        public const string RECHARGE_VERY_LONG = "很长";


        [TranslateMsg("对话档案对话框标题", CONTEXT_ARCHIVE)]
        public const string ARCHIVE_BRANCH = "剧情分支";
        [TranslateMsg("对话档案对话框内容", CONTEXT_ARCHIVE)]
        public const string ARCHIVE_WHETHER_HAS_ENOUGH_MONEY = "是否拥有足够的金钱？";
        [TranslateMsg("对话档案对话框标题", CONTEXT_ARCHIVE)]
        public const string ARCHIVE_TALK_END = "对话结束";
        [TranslateMsg("对话档案对话框内容", CONTEXT_ARCHIVE)]
        public const string ARCHIVE_REPLAY = "是否重新播放？";


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
        [TranslateMsg("通用文本")]
        public const string CONFIRM = "确认";

        [TranslateMsg("游戏内文本提示")]
        public const string TOOLTIP_NOT_ENOUGH_ENERGY = "能量不足";
        [TranslateMsg("游戏内文本提示")]
        public const string TOOLTIP_RECHARGING = "重新充能中…";
        [TranslateMsg("游戏内文本提示")]
        public const string TOOLTIP_DIG_CONTRAPTION = "挖掉器械";
        [TranslateMsg("游戏内文本提示")]
        public const string TOOLTIP_TRIGGER_CONTRAPTION = "触发器械";

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
        [TranslateMsg("关卡名称，{0}为关卡名，{1}为冒险模式天数", CONTEXT_LEVEL_NAME)]
        public const string LEVEL_NAME_DAY_TEMPLATE = "{0} - 第{1}天";
        [TranslateMsg("关卡名称，{0}为关卡名，{1}为无尽模式轮数", CONTEXT_LEVEL_NAME)]
        public const string LEVEL_NAME_ENDLESS_FLAGS_TEMPLATE = "{0} - 第{1}轮";

        public const string CONTEXT_DEATH_MESSAGE = "death_message";
        [TranslateMsg("死亡信息-未知", CONTEXT_DEATH_MESSAGE)]
        public const string DEATH_MESSAGE_UNKNOWN = "你死了！";
        [TranslateMsg("死亡信息-未知", CONTEXT_DEATH_MESSAGE)]
        public const string DEATH_MESSAGE_CRUSHING_WALLS = "<color=red>来到我们之中吧</color>";

        [TranslateMsg("实体名称-未知", CONTEXT_ENTITY_NAME)]
        public const string UNKNOWN_ENTITY_NAME = "？？？";

        [TranslateMsg("蓝图选项名称-未知", CONTEXT_OPTION_NAME)]
        public const string UNKNOWN_OPTION_NAME = "？？？";

        [TranslateMsg("实体说明-未知", CONTEXT_ENTITY_TOOLTIP)]
        public const string UNKNOWN_ENTITY_TOOLTIP = "？？？";

        [TranslateMsg("制品名称-未知", CONTEXT_ARTIFACT_NAME)]
        public const string UNKNOWN_ARTIFACT_NAME = "？？？";

        [TranslateMsg("制品说明-未知", CONTEXT_ARTIFACT_TOOLTIP)]
        public const string UNKNOWN_ARTIFACT_TOOLTIP = "？？？";

        [TranslateMsg("怪物说明-还没有遇到", CONTEXT_ALMANAC)]
        public const string NOT_ENCOUNTERED_YET = "（还没有遇到）";


        [TranslateMsg("音乐名-无", CONTEXT_MUSIC_NAME)]
        public const string MUSIC_NAME_NONE = "无";

        public static string GetAlmanacNameContext(string category)
        {
            return $"{category}.name";
        }
        public static string GetAlmanacDescriptionContext(string category)
        {
            return $"{category}.description";
        }
        public static string GetTalkTextContext(NamespaceID groupID)
        {
            return $"talk-{groupID.SpaceName}:{groupID.Path}";
        }
        public const string CONTEXT_ACHIEVEMENT = "achievement";
        public const string CONTEXT_ARCHIVE = "archive";
        public const string CONTEXT_ARCHIVE_TAG_NAME = "archive.tagname";
        public const string CONTEXT_ADVICE = "advice";
        public const string CONTEXT_ERROR = "error";
        public const string CONTEXT_ENTITY_NAME = "entity.name";
        public const string CONTEXT_CHARACTER_NAME = "character.name";
        public const string CONTEXT_MUSIC_NAME = "music.name";
        public const string CONTEXT_ENTITY_TOOLTIP = "entity.tooltip";
        public const string CONTEXT_ARTIFACT_NAME = "artifact.name";
        public const string CONTEXT_ARTIFACT_TOOLTIP = "artifact.tooltip";
        public const string CONTEXT_ALMANAC = "almanac";
        public const string CONTEXT_ALMANAC_GROUP_NAME = "almanac.group_name";
        public const string CONTEXT_LANGUAGE_NAME = "language_name";
        public const string CONTEXT_OPTION_NAME = "option.name";
        public const string CONTEXT_RECHARGE_TIME = "recharge_time";
        public const string CONTEXT_STAT_CATEGORY = "stat_category";
        public const string CONTEXT_STORE_TALK = "store_talk";
        public const string CONTEXT_TALK = "talk";
    }
}
