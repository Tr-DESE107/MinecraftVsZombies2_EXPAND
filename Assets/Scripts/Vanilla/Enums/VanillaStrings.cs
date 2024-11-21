using MukioI18n;
using MVZ2Logic;

namespace MVZ2.Vanilla
{
    public static class VanillaStrings
    {
        [TranslateMsg("教程无法使用提示")]
        public const string TOOLTIP_DISABLE_MESSAGE = "无法使用";

        [TranslateMsg("不能在雕像上放置器械提示", StringTable.CONTEXT_ADVICE_ERROR)]
        public const string ADVICE_CANNOT_PLACE_ON_STATUES = "你不能在雕像上放置器械";
    }
}
