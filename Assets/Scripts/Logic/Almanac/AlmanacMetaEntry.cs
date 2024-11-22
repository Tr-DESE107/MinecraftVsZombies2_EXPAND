using System.Xml;
using PVZEngine;

namespace MVZ2Logic.Almanacs
{
    public class AlmanacMetaEntry
    {
        public NamespaceID id;
        public string text;
        public int index = -1;
        public bool IsEmpty()
        {
            return !NamespaceID.IsValid(id);
        }
    }
}
