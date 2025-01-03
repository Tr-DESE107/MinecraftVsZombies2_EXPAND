using PVZEngine;

namespace MVZ2.Vanilla.Models
{
    public class VanillaModelKeys
    {
        public static readonly NamespaceID shortCircuit = Get("short_circuit");
        public static readonly NamespaceID staticParticles = Get("static_particles");
        public static readonly NamespaceID dreamKeyShield = Get("dream_key_shield");
        public static readonly NamespaceID nocturnal = Get("nocturnal");
        public static readonly NamespaceID terrorParasitized = Get("terror_parasitized");
        public static readonly NamespaceID weaknessParticles = Get("weakness_particles");
        public static readonly NamespaceID dreamAlarm = Get("dream_alarm");
        public static readonly NamespaceID parabotInsected = Get("parabot_insected");
        private static NamespaceID Get(string name)
        {
            return new NamespaceID(VanillaMod.spaceName, name);
        }
    }
}
