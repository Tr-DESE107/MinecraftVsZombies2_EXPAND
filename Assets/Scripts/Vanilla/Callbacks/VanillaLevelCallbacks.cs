using MVZ2.Vanilla.Entities;
using PVZEngine;
using PVZEngine.Damages;
using PVZEngine.Entities;
using PVZEngine.Grids;
using PVZEngine.SeedPacks;
using PVZEngine.Triggers;

namespace MVZ2.Vanilla.Callbacks
{
    public static class VanillaLevelCallbacks
    {
        public delegate void PreEntityTakeDamage(DamageInput damageInfo);
        public delegate void PostEntityTakeDamage(DamageOutput result);
        public delegate void PreEntityHeal(HealInput info);
        public delegate void PostEntityHeal(HealOutput result);

        public delegate void CanPlaceEntity(LawnGrid grid, NamespaceID entityID, TriggerResultNamespaceID error);
        public delegate void PrePlaceEntity(LawnGrid grid, NamespaceID entityID, TriggerResultBoolean cancel);
        public delegate void PostPlaceEntity(LawnGrid grid, Entity entity);
        public delegate void PostUseEntityBlueprint(SeedPack blueprint, Entity entity);

        public delegate void PostContraptionTrigger(Entity entity);
        public delegate void PostContraptionEvoke(Entity entity);
        public delegate void PostContraptionDestroy(Entity entity);
        public delegate void CanContraptionSacrifice(Entity entity, Entity soulFurnace, TriggerResultBoolean result);
        public delegate void GetContraptionSacrificeFuel(Entity entity, Entity soulFurnace, TriggerResultInt fuel);
        public delegate void PreContraptionSacrifice(Entity entity, Entity soulFurnace, int fuel);
        public delegate void PostContraptionSacrifice(Entity entity, Entity soulFurnace, int fuel);

        public delegate void PostWaterInteraction(Entity entity, int action);

        public delegate void PreEnemyNeutralize(Entity entity, TriggerResultBoolean result);
        public delegate void PostEnemyNeutralize(Entity entity);
        public delegate void EnemyDropRewards(Entity entity);

        public delegate void PrePickupCollect(Entity entity, TriggerResultBoolean result);

        public delegate void PreProjectileHit(ProjectileHitInput hit, DamageInput damage);
        public delegate void PostProjectileHit(ProjectileHitOutput hit, DamageOutput damage);



        public readonly static CallbackReference<PreEntityTakeDamage> PRE_ENTITY_TAKE_DAMAGE = new();
        public readonly static CallbackReference<PostEntityTakeDamage> POST_ENTITY_TAKE_DAMAGE = new();
        public readonly static CallbackReference<PreEntityHeal> PRE_ENTITY_HEAL = new();
        public readonly static CallbackReference<PostEntityHeal> POST_ENTITY_HEAL = new();

        public readonly static CallbackReference<CanPlaceEntity> CAN_PLACE_ENTITY = new();
        public readonly static CallbackReference<PrePlaceEntity> PRE_PLACE_ENTITY = new();
        public readonly static CallbackReference<PostPlaceEntity> POST_PLACE_ENTITY = new();
        public readonly static CallbackReference<PostUseEntityBlueprint> POST_USE_ENTITY_BLUEPRINT = new();

        public readonly static CallbackReference<PostContraptionTrigger> POST_CONTRAPTION_TRIGGER = new();
        public readonly static CallbackReference<PostContraptionEvoke> POST_CONTRAPTION_EVOKE = new();
        public readonly static CallbackReference<PostContraptionDestroy> POST_CONTRAPTION_DESTROY = new();
        public readonly static CallbackReference<CanContraptionSacrifice> CAN_CONTRAPTION_SACRIFICE = new();
        public readonly static CallbackReference<GetContraptionSacrificeFuel> GET_CONTRAPTION_SACRIFICE_FUEL = new();
        public readonly static CallbackReference<PreContraptionSacrifice> PRE_CONTRAPTION_SACRIFICE = new();
        public readonly static CallbackReference<PostContraptionSacrifice> POST_CONTRAPTION_SACRIFICE = new();

        public readonly static CallbackReference<PostWaterInteraction> POST_WATER_INTERACTION = new();

        public readonly static CallbackReference<PreEnemyNeutralize> PRE_ENEMY_NEUTRALIZE = new();
        public readonly static CallbackReference<PostEnemyNeutralize> POST_ENEMY_NEUTRALIZE = new();
        public readonly static CallbackReference<EnemyDropRewards> ENEMY_DROP_REWARDS = new();

        public readonly static CallbackReference<PrePickupCollect> PRE_PICKUP_COLLECT = new();

        public readonly static CallbackReference<PreProjectileHit> PRE_PROJECTILE_HIT = new();
        public readonly static CallbackReference<PostProjectileHit> POST_PROJECTILE_HIT = new();
    }
}
