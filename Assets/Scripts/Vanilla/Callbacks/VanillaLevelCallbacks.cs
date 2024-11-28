using PVZEngine;
using PVZEngine.Callbacks;
using PVZEngine.Damages;
using PVZEngine.Entities;

namespace MVZ2.Vanilla.Callbacks
{
    public static class VanillaLevelCallbacks
    {
        public readonly static NamespaceID PRE_ENTITY_TAKE_DAMAGE = Get("pre_entity_take_damage");
        public readonly static NamespaceID PRE_PICKUP_COLLECT = Get("pre_pickup_collect");
        public readonly static NamespaceID POST_PROJECTILE_HIT = Get("post_projectile_hit");
        public readonly static NamespaceID POST_CONTRAPTION_TRIGGER = Get("post_contraption_trigger");

        public readonly static NamespaceID CAN_CONTRAPTION_SACRIFICE = Get("can_contraption_sacrifice");
        public readonly static NamespaceID GET_CONTRAPTION_SACRIFICE_FUEL = Get("get_contraption_sacrifice_fuel");
        public readonly static NamespaceID PRE_CONTRAPTION_SACRIFICE = Get("pre_contraption_sacrifice");
        public readonly static NamespaceID POST_CONTRAPTION_SACRIFICE = Get("post_contraption_sacrifice");
        public readonly static CallbackActionList<Entity> PostContraptionEvoked = new();
        public readonly static CallbackActionList<DamageResult, DamageResult> PostEntityTakeDamage = new();
        private static NamespaceID Get(string path)
        {
            return new NamespaceID("mvz2", path);
        }
    }
}
