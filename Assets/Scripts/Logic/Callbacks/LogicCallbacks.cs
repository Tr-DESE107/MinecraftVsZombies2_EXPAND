using System.Collections.Generic;
using PVZEngine;
using PVZEngine.Callbacks;

namespace MVZ2Logic.Callbacks
{
    public static class LogicCallbacks
    {
        public struct GetInnateBlueprintsParams
        {
            public List<NamespaceID> list;
        }
        public readonly static CallbackType<GetInnateBlueprintsParams> GET_INNATE_BLUEPRINTS = new();
        public readonly static CallbackType<EmptyCallbackParams> GET_BLUEPRINT_SLOT_COUNT = new();
        public readonly static CallbackType<StringCallbackParams> IS_SPECIAL_USER_NAME = new();
    }
}
