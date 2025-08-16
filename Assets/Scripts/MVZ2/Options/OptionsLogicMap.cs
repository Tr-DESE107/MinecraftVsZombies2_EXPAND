using MVZ2.Map;
using MVZ2.UI;

namespace MVZ2.Options
{
    using static MVZ2.UI.OptionsDialog;
    public class OptionsLogicMap : OptionsLogic
    {
        public OptionsLogicMap(MapController map, OptionsDialog dialog) : base(dialog)
        {
            this.map = map;
        }
        public override void InitDialog()
        {
            base.InitDialog();

            dialog.SetButtonActive(ButtonType.LeaveLevel, false);
            dialog.SetButtonActive(ButtonType.Restart, false);
        }
        private MapController map;
    }
}
