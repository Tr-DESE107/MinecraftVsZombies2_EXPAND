using MVZ2.Vanilla;
using PVZEngine;

namespace MVZ2.GameContent
{
    public static class SoundID
    {
        public readonly static NamespaceID armorUp = Get("armor_up");
        public readonly static NamespaceID awooga = Get("awooga");
        public readonly static NamespaceID buzzer = Get("buzzer");
        public readonly static NamespaceID dirtRise = Get("dirt_rise");
        public readonly static NamespaceID fire = Get("fire");
        public readonly static NamespaceID finalWave = Get("final_wave");
        public readonly static NamespaceID grass = Get("grass");
        public readonly static NamespaceID hugeWave = Get("huge_wave");
        public readonly static NamespaceID leatherHit = Get("leather_hit");
        public readonly static NamespaceID mineExplode = Get("mine_explode");
        public readonly static NamespaceID minecart = Get("minecart");
        public readonly static NamespaceID points = Get("points");
        public readonly static NamespaceID pick = Get("pick");
        public readonly static NamespaceID pickaxe = Get("pickaxe");
        public readonly static NamespaceID shot = Get("shot");
        public readonly static NamespaceID siren = Get("siren");
        public readonly static NamespaceID slice = Get("slice");
        public readonly static NamespaceID shieldHit = Get("shield_hit");
        public readonly static NamespaceID splat = Get("splat");
        public readonly static NamespaceID stone = Get("stone");
        public readonly static NamespaceID tap = Get("tap");
        public readonly static NamespaceID throwSound = Get("throw");
        public readonly static NamespaceID zombieDeath = Get("zombie_death");
        private static NamespaceID Get(string name)
        {
            return new NamespaceID(VanillaMod.spaceName, name);
        }
    }
}
