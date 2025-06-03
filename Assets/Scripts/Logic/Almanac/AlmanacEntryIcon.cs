using PVZEngine;

namespace MVZ2Logic.Almanacs
{
    public struct AlmanacEntryTagInfo
    {
        public NamespaceID tagID;
        public string enumValue;

        public AlmanacEntryTagInfo(NamespaceID tagID, string enumValue)
        {
            this.tagID = tagID;
            this.enumValue = enumValue;
        }
        public AlmanacEntryTagInfo(NamespaceID tagID) : this(tagID, null)
        {
        }
    }
}
