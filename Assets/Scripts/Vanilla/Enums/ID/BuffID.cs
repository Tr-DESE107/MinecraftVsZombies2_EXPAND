using PVZEngine;

namespace MVZ2.GameContent
{
    public static class BuffID
    {
        public static readonly NamespaceID randomEnemySpeed = Get("random_enemy_speed");
        public static readonly NamespaceID damageColor = Get("damage_color");
        public static readonly NamespaceID productionColor = Get("production_color");
        private static NamespaceID Get(string name)
        {
            return new NamespaceID("mvz2", name);
        }
    }
}
