using PVZEngine;

namespace MVZ2Logic.Almanacs
{
    public interface IAlmanacMetaEntry
    {
        NamespaceID ID { get; }
        string Text { get; }
        int Index { get; }
        public bool IsEmpty()
        {
            return !NamespaceID.IsValid(ID);
        }
    }
}
