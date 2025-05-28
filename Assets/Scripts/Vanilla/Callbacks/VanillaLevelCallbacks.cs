using MVZ2.HeldItems;
using MVZ2.Vanilla.Entities;
using PVZEngine;
using PVZEngine.Buffs;
using PVZEngine.Callbacks;
using PVZEngine.Damages;
using PVZEngine.Definitions;
using PVZEngine.Entities;
using PVZEngine.Grids;
using PVZEngine.SeedPacks;

namespace MVZ2.Vanilla.Callbacks
{
    public static class VanillaLevelCallbacks
    {
        public struct PreTakeDamageParams
        {
            public DamageInput input;
        }
        public struct PostTakeDamageParams
        {
            public DamageOutput output;
        }
        public struct PreHealParams
        {
            public HealInput input;
        }
        public struct PostHealParams
        {
            public HealOutput output;
        }

        public struct PlaceEntityParams
        {
            public LawnGrid grid;
            public NamespaceID entityID;

            public PlaceEntityParams(LawnGrid grid, NamespaceID entityID)
            {
                this.grid = grid;
                this.entityID = entityID;
            }
        }
        public struct PostPlaceEntityParams
        {
            public LawnGrid grid;
            public Entity entity;

            public PostPlaceEntityParams(LawnGrid grid, Entity entity)
            {
                this.grid = grid;
                this.entity = entity;
            }
        }
        public struct PostUseEntityBlueprintParams
        {
            public Entity entity;
            public SeedDefinition definition;
            public SeedPack blueprint;
            public IHeldItemData heldData;

            public PostUseEntityBlueprintParams(Entity entity, SeedDefinition definition, SeedPack blueprint, IHeldItemData heldData)
            {
                this.entity = entity;
                this.definition = definition;
                this.blueprint = blueprint;
                this.heldData = heldData;
            }
        }
        public struct PostEntityCharmParams
        {
            public Entity entity;
            public Buff buff;

            public PostEntityCharmParams(Entity entity, Buff buff)
            {
                this.entity = entity;
                this.buff = buff;
            }
        }

        public struct ContraptionSacrificeValueParams
        {
            public Entity entity;
            public Entity soulFurnace;

            public ContraptionSacrificeValueParams(Entity entity, Entity soulFurnace)
            {
                this.entity = entity;
                this.soulFurnace = soulFurnace;
            }
        }
        public struct ContraptionSacrificeParams
        {
            public Entity entity;
            public Entity soulFurnace;
            public int fuel;

            public ContraptionSacrificeParams(Entity entity, Entity soulFurnace, int fuel)
            {
                this.entity = entity;
                this.soulFurnace = soulFurnace;
                this.fuel = fuel;
            }
        }

        public struct WaterInteractionParams
        {
            public Entity entity;
            public int action;
        }

        public struct EnemyMeleeAttackParams
        {
            public Entity enemy;
            public Entity target;
            public float amount;

            public EnemyMeleeAttackParams(Entity enemy, Entity target, float amount)
            {
                this.enemy = enemy;
                this.target = target;
                this.amount = amount;
            }
        }

        public struct PreProjectileHitParams
        {
            public ProjectileHitInput hit;
            public DamageInput damage;
        }
        public struct PostProjectileHitParams
        {
            public ProjectileHitOutput hit;
            public DamageOutput damage;
        }


        public readonly static CallbackType<LevelCallbackParams> POST_HUGE_WAVE_APPROACH = new();
        public readonly static CallbackType<LevelCallbackParams> POST_FINAL_WAVE = new();

        public readonly static CallbackType<PreTakeDamageParams> PRE_ENTITY_TAKE_DAMAGE = new();
        public readonly static CallbackType<PostTakeDamageParams> POST_ENTITY_TAKE_DAMAGE = new();
        public readonly static CallbackType<PreHealParams> PRE_ENTITY_HEAL = new();
        public readonly static CallbackType<PostHealParams> POST_ENTITY_HEAL = new();

        public readonly static CallbackType<PlaceEntityParams> PRE_PLACE_ENTITY = new();
        public readonly static CallbackType<PostPlaceEntityParams> POST_PLACE_ENTITY = new();
        public readonly static CallbackType<PostUseEntityBlueprintParams> POST_USE_ENTITY_BLUEPRINT = new();
        public readonly static CallbackType<PostEntityCharmParams> POST_ENTITY_CHARM = new();

        public readonly static CallbackType<EntityCallbackParams> POST_CONTRAPTION_TRIGGER = new();
        public readonly static CallbackType<EntityCallbackParams> POST_CONTRAPTION_EVOKE = new();
        public readonly static CallbackType<EntityCallbackParams> POST_CONTRAPTION_DESTROY = new();
        public readonly static CallbackType<EntityCallbackParams> POST_CONTRAPTION_DETONATE = new();
        public readonly static CallbackType<ContraptionSacrificeValueParams> CAN_CONTRAPTION_SACRIFICE = new();
        public readonly static CallbackType<ContraptionSacrificeValueParams> GET_CONTRAPTION_SACRIFICE_FUEL = new();
        public readonly static CallbackType<ContraptionSacrificeParams> PRE_CONTRAPTION_SACRIFICE = new();
        public readonly static CallbackType<ContraptionSacrificeParams> POST_CONTRAPTION_SACRIFICE = new();

        public readonly static CallbackType<WaterInteractionParams> POST_WATER_INTERACTION = new();

        public readonly static CallbackType<EntityCallbackParams> ENEMY_DROP_REWARDS = new();
        public readonly static CallbackType<EntityCallbackParams> PRE_ENEMY_NEUTRALIZE = new();
        public readonly static CallbackType<EntityCallbackParams> POST_ENEMY_NEUTRALIZE = new();
        public readonly static CallbackType<EntityCallbackParams> PRE_ENEMY_FAINT = new();
        public readonly static CallbackType<EntityCallbackParams> POST_ENEMY_FAINT = new();
        public readonly static CallbackType<EntityCallbackParams> POST_OBSIDIAN_FIRST_AID = new();
        public readonly static CallbackType<EnemyMeleeAttackParams> POST_ENEMY_MELEE_ATTACK = new();

        public readonly static CallbackType<EntityCallbackParams> CAN_PICKUP_COLLECT = new();

        public readonly static CallbackType<EntityCallbackParams> POST_PROJECTILE_SHOT = new();
        public readonly static CallbackType<PreProjectileHitParams> PRE_PROJECTILE_HIT = new();
        public readonly static CallbackType<PostProjectileHitParams> POST_PROJECTILE_HIT = new();

        public readonly static CallbackType<EntityCallbackParams> POST_USE_STARSHARD = new();
    }
}
