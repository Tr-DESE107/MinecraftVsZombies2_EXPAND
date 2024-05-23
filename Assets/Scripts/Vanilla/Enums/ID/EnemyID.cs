using MVZ2.Vanilla;
using PVZEngine;

namespace MVZ2.GameContent
{
    public static class EnemyNames
    {
        public const string zombie = "zombie";
    }
    public static class EnemyID
    {
        public static readonly NamespaceID zombie = Get(EnemyNames.zombie);
        private static NamespaceID Get(string name)
        {
            return new NamespaceID(VanillaMod.spaceName, name);
        }
    }
}
