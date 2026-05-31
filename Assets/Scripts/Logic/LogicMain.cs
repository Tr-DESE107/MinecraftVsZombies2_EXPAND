#nullable enable

using MukioI18n;
using UnityEngine;

namespace MVZ2Logic
{
    public static class LogicMain
    {
        public static string GetFloatPercentageText(float value)
        {
            return Global.Localization.GetText(VALUE_PERCENT, Mathf.RoundToInt(value * 100));
        }
        [TranslateMsg("值，{0}为百分数")]
        public const string VALUE_PERCENT = "{0}%";
    }
}