using MukioI18n;

namespace MVZ2
{
    public static class StringTable
    {
        public const string CONTEXT_DIFFICULTY = "关卡难度";
        [TranslateMsg("关卡难度", CONTEXT_DIFFICULTY)]
        public const string DIFFICULTY_EASY = "简单";
        [TranslateMsg("关卡难度", CONTEXT_DIFFICULTY)]
        public const string DIFFICULTY_NORMAL = "普通";
        [TranslateMsg("关卡难度", CONTEXT_DIFFICULTY)]
        public const string DIFFICULTY_HARD = "困难";
        [TranslateMsg("关卡难度", CONTEXT_DIFFICULTY)]
        public const string DIFFICULTY_UNKNOWN = "未知难度";

        public const string CONTEXT_LEVEL_NAME = "关卡名称";
        [TranslateMsg("关卡名称", CONTEXT_LEVEL_NAME)]
        public const string LEVEL_NAME_UNKNOWN = "未知关卡";
        [TranslateMsg("关卡名称", CONTEXT_LEVEL_NAME)]
        public const string LEVEL_NAME_PROLOGUE = "序章";
        [TranslateMsg("关卡名称", CONTEXT_LEVEL_NAME)]
        public const string LEVEL_NAME_HALLOWEEN = "万圣夜";
    }
}
