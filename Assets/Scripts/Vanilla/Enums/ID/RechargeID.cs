using MVZ2.Vanilla;
using PVZEngine;

namespace MVZ2.GameContent
{
    public static class RechargeNames
    {
        public const string none = "none";
        public const string shortTime = "short";
        public const string longTime = "long";
        public const string veryLongTime = "very_long";
    }
    public static class RechargeID
    {
        public static readonly NamespaceID none = Get(RechargeNames.none);
        public static readonly NamespaceID shortTime = Get(RechargeNames.shortTime);
        public static readonly NamespaceID longTime = Get(RechargeNames.longTime);
        public static readonly NamespaceID veryLongTime = Get(RechargeNames.veryLongTime);
        private static NamespaceID Get(string name)
        {
            return new NamespaceID(VanillaMod.spaceName, name);
        }
    }
}
