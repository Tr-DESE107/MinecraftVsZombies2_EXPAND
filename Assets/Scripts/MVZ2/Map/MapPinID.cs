using MVZ2Logic;
using PVZEngine;

namespace MVZ2.Map
{
    public static class MapPinID
    {
        public static readonly NamespaceID halloween = Get("halloween");
        public static readonly NamespaceID dream = Get("dream");
        public static readonly NamespaceID kourindou = Get("kourindou");
        public static readonly NamespaceID teruharijou = Get("teruharijou");
        public static NamespaceID Get(string path)
        {
            return new NamespaceID(Global.BuiltinNamespace, path);
        }
    }
}
