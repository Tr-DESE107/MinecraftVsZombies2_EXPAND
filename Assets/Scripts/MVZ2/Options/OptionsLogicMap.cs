using MVZ2.UI;

namespace MVZ2.Options
{
    using static MVZ2.UI.OptionsDialog;
    public class OptionsLogicMap : OptionsLogic
    {
        public OptionsLogicMap(OptionsDialog dialog) : base(dialog)
        {
        }
        public override void InitDialog()
        {
            base.InitDialog();

            dialog.SetButtonActive(ButtonType.LeaveLevel, false);
            dialog.SetButtonActive(ButtonType.Restart, false);
        }
    }
}
