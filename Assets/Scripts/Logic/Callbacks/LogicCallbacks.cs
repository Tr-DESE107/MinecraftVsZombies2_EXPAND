using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PVZEngine.Triggers;

namespace MVZ2Logic.Callbacks
{
    public static class LogicCallbacks
    {

        public delegate void GetInnateBlueprints(TriggerResultNamespaceIDList result);
        public delegate void GetBlueprintSlotCount(TriggerResultInt result);
        public delegate void IsSpecialUserName(string name, TriggerResultBoolean result);

        public readonly static CallbackReference<GetInnateBlueprints> GET_INNATE_BLUEPRINTS = new();
        public readonly static CallbackReference<GetBlueprintSlotCount> GET_BLUEPRINT_SLOT_COUNT = new();
        public readonly static CallbackReference<IsSpecialUserName> IS_SPECIAL_USER_NAME = new();
    }
}
