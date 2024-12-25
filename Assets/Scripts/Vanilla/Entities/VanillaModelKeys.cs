using PVZEngine;

namespace MVZ2.Vanilla.Models
{
    public class VanillaModelKeys
    {
        public static readonly NamespaceID shortCircuit = Get("short_circuit");
        public static readonly NamespaceID staticParticles = Get("static_particles");
        public static readonly NamespaceID dreamKeyShield = Get("dream_key_shield");
        public static readonly NamespaceID nocturnal = Get("nocturnal");
        private static NamespaceID Get(string name)
        {
            return new NamespaceID(VanillaMod.spaceName, name);
        }
    }
}
