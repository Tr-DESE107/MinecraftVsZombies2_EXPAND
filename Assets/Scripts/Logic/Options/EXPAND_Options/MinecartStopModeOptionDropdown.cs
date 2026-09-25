#nullable enable

using System.Collections.Generic;
using MukioI18n;
using MVZ2Logic;
using MVZ2Logic.Definitions;
using MVZ2Logic.Options;

namespace MVZ2.GameContent.Options
{
    [AutoOptionWidgetDefinition(LogicOptionWidgetNames.minecartStopMode)]
    public class MinecartStopModeOptionDropdown : OptionDropdownDefinition
    {
        public MinecartStopModeOptionDropdown(string nsp, string name) : base(nsp, name)
        {
        }
        public override int GetValue(IOptionContext context)
        {
            return Global.Options.GetMinecartStopMode();
        }
        public override void FillItems(IOptionContext context, List<string> items)
        {
            var localization = Global.Localization;
            items.Add(localization.GetTextParticular(LABEL_INSTANT, CONTEXT));
            items.Add(localization.GetTextParticular(LABEL_GLIDE, CONTEXT));
        }
        public override void OnValueChanged(IOptionContext context, int index)
        {
            Global.Options.SetOptionInt(LogicOptionItemID.minecartStopMode, index);
            Global.Options.SaveOptions();
        }
        public const string CONTEXT = "option.minecart_stop_mode";
        [TranslateMsg("重装兵器停止方式", CONTEXT)]
        public const string LABEL_INSTANT = "立即停止";
        [TranslateMsg("重装兵器停止方式", CONTEXT)]
        public const string LABEL_GLIDE = "惯性滑行";
    }
}
