#nullable enable

using PVZEngine;

namespace MVZ2Logic.Blueprints
{
    public static class LogicBlueprintStyles
    {
        public static readonly NamespaceID normal = Get("normal");
        public static readonly NamespaceID commandBlock = Get("command_block");
        public static readonly NamespaceID upgrade = Get("upgrade");
        public static NamespaceID Get(string name)
        {
            return new NamespaceID(Global.BuiltinNamespace, name);
        }
    }
}