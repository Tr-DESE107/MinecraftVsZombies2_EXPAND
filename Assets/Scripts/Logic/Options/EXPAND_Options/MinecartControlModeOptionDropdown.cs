#nullable enable  
  
using System.Collections.Generic;  
using MukioI18n;  
using MVZ2Logic;  
using MVZ2Logic.Definitions;  
using MVZ2Logic.Options;  
  
namespace MVZ2.GameContent.Options  
{  
    [AutoOptionWidgetDefinition(LogicOptionWidgetNames.minecartControlMode)]  
    public class MinecartControlModeOptionDropdown : OptionDropdownDefinition  
    {  
        public MinecartControlModeOptionDropdown(string nsp, string name) : base(nsp, name)  
        {  
        }  
        public override int GetValue(IOptionContext context)  
        {  
            return Global.Options.GetMinecartControlMode();  
        }  
        public override void FillItems(IOptionContext context, List<string> items)  
        {  
            var localization = Global.Localization;  
            items.Add(localization.GetTextParticular(LABEL_MOUSE, CONTEXT));  
            items.Add(localization.GetTextParticular(LABEL_KEYBOARD, CONTEXT));  
        }  
        public override void OnValueChanged(IOptionContext context, int index)  
        {  
            Global.Options.SetOptionInt(LogicOptionItemID.minecartControlMode, index);  
            Global.Options.SaveOptions();  
        }  
        public const string CONTEXT = "option.minecart_control_mode";  
        [TranslateMsg("重装兵器移动方式", CONTEXT)]  
        public const string LABEL_MOUSE = "跟随鼠标";  
        [TranslateMsg("重装兵器移动方式", CONTEXT)]  
        public const string LABEL_KEYBOARD = "键盘/按钮";  
    }  
}
