using MVZ2.Mainmenu;
using MVZ2.UI;

namespace MVZ2.Options
{
    using static MVZ2.UI.OptionsDialog;
    public class OptionsLogicMainmenu : OptionsLogic
    {
        public OptionsLogicMainmenu(OptionsDialog dialog, MainmenuController controller) : base(dialog)
        {
            mainmenu = controller;
        }
        public override void InitDialog()
        {
            base.InitDialog();

            dialog.SetButtonActive(ButtonType.LeaveLevel, false);
            dialog.SetButtonActive(ButtonType.Restart, false);
        }
        private MainmenuController mainmenu;
    }
}
