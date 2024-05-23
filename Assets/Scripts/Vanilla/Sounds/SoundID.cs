using PVZEngine;

namespace MVZ2.GameContent
{
    public static class SoundID
    {
        public readonly static NamespaceID buzzer = Get("buzzer");
        public readonly static NamespaceID fire = Get("fire");
        public readonly static NamespaceID grass = Get("grass");
        public readonly static NamespaceID points = Get("points");
        public readonly static NamespaceID pick = Get("pick");
        public readonly static NamespaceID pickaxe = Get("pickaxe");
        public readonly static NamespaceID shot = Get("shot");
        public readonly static NamespaceID slice = Get("slice");
        public readonly static NamespaceID splat = Get("splat");
        public readonly static NamespaceID stone = Get("stone");
        public readonly static NamespaceID tap = Get("tap");
        public readonly static NamespaceID throwSound = Get("throw");
        public readonly static NamespaceID zombieDeath = Get("zombie_death");
        private static NamespaceID Get(string name)
        {
            return new NamespaceID("mvz2", name);
        }
    }
}
