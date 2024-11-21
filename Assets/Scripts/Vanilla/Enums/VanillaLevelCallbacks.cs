using PVZEngine;
using PVZEngine.Callbacks;
using PVZEngine.Damages;
using PVZEngine.Entities;

namespace MVZ2.Vanilla
{
    public static class VanillaLevelCallbacks
    {
        public readonly static NamespaceID PRE_ENTITY_TAKE_DAMAGE = Get("pre_entity_take_damage");
        public readonly static NamespaceID PRE_PICKUP_COLLECT = Get("pre_pickup_collect");
        public readonly static CallbackActionList<Entity> PostContraptionEvoked = new();
        public readonly static CallbackActionList<DamageResult, DamageResult> PostEntityTakeDamage = new();
        private static NamespaceID Get(string path)
        {
            return new NamespaceID("mvz2", path);
        }
    }
}
