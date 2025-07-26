using System.Collections.Generic;
using MVZ2Logic.Almanacs;
using PVZEngine;
using PVZEngine.Callbacks;

namespace MVZ2Logic.Callbacks
{
    public static class LogicCallbacks
    {
        public struct GetAlmanacEntryTagsParams
        {
            public string category;
            public NamespaceID entryID;
            public List<AlmanacEntryTagInfo> tags;

            public GetAlmanacEntryTagsParams(string category, NamespaceID entryID, List<AlmanacEntryTagInfo> tags)
            {
                this.category = category;
                this.entryID = entryID;
                this.tags = tags;
            }
        }
        public struct GetInnateBlueprintsParams
        {
            public List<NamespaceID> list;
        }
        public struct GetInnateArtifactsParams
        {
            public List<NamespaceID> list;
        }
        public readonly static CallbackType<GetAlmanacEntryTagsParams> GET_ALMANAC_ENTRY_TAGS = new();
        public readonly static CallbackType<GetInnateBlueprintsParams> GET_INNATE_BLUEPRINTS = new();
        public readonly static CallbackType<GetInnateArtifactsParams> GET_INNATE_ARTIFACTS = new();
        public readonly static CallbackType<EmptyCallbackParams> GET_BLUEPRINT_SLOT_COUNT = new();
        public readonly static CallbackType<StringCallbackParams> IS_SPECIAL_USER_NAME = new();
    }
}
