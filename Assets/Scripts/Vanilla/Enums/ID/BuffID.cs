using MVZ2.Vanilla;
using PVZEngine;

namespace MVZ2.GameContent
{
    public static class BuffNames
    {
        public const string randomEnemySpeed = "random_enemy_speed";
        public const string damageColor = "damage_color";
        public const string productionColor = "production_color";
    }
    public static class BuffID
    {
        public static readonly NamespaceID randomEnemySpeed = Get(BuffNames.randomEnemySpeed);
        public static readonly NamespaceID damageColor = Get(BuffNames.damageColor);
        public static readonly NamespaceID productionColor = Get(BuffNames.productionColor);
        private static NamespaceID Get(string name)
        {
            return new NamespaceID(VanillaMod.spaceName, name);
        }
    }
}
