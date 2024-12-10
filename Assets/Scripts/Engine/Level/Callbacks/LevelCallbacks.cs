using PVZEngine.Armors;
using PVZEngine.Damages;
using PVZEngine.Entities;
using PVZEngine.Level;
using PVZEngine.Triggers;
using UnityEngine;

namespace PVZEngine.Callbacks
{
    public static class LevelCallbacks
    {

        public delegate void PostEntityInit(Entity entity);
        public delegate void PostEntityUpdate(Entity entity);
        public delegate void PostEntityContactGround(Entity entity, Vector3 Velocity);
        public delegate void PostEntityLeaveGround(Entity entity);
        public delegate void PostEntityCollision(EntityCollision collision, int state);
        public delegate void PostEntityDeath(Entity entity, DamageInput damageInfo);
        public delegate void PostEntityRemove(Entity entity);
        public delegate void PostEquipArmor(Entity entity, Armor armor);
        public delegate void PostDestroyArmor(Entity entity, Armor armor, ArmorDamageResult damageResult);
        public delegate void PostRemoveArmor(Entity entity, Armor armor);

        public delegate void PostLevelSetup(LevelEngine level);
        public delegate void PostLevelStart(LevelEngine level);
        public delegate void PostLevelUpdate(LevelEngine level);
        public delegate void PostLevelClear(LevelEngine level);
        public delegate void PostPrepareForBattle(LevelEngine level);
        public delegate void PostWave(LevelEngine level, int wave);
        public delegate void PostHugeWaveEvent(LevelEngine level);
        public delegate void PostFinalWaveEvent(LevelEngine level);
        public delegate void PostGameOver(LevelEngine level, int type, Entity killer, string message);
        public delegate void PostEnemySpawned(Entity entity);

        public readonly static CallbackReference<PostEntityInit> POST_ENTITY_INIT = new();
        public readonly static CallbackReference<PostEntityUpdate> POST_ENTITY_UPDATE = new();
        public readonly static CallbackReference<PostEntityContactGround> POST_ENTITY_CONTACT_GROUND = new();
        public readonly static CallbackReference<PostEntityLeaveGround> POST_ENTITY_LEAVE_GROUND = new();
        public readonly static CallbackReference<PostEntityCollision> POST_ENTITY_COLLISION = new();
        public readonly static CallbackReference<PostEntityDeath> POST_ENTITY_DEATH = new();
        public readonly static CallbackReference<PostEntityRemove> POST_ENTITY_REMOVE = new();
        public readonly static CallbackReference<PostEquipArmor> POST_EQUIP_ARMOR = new();
        public readonly static CallbackReference<PostDestroyArmor> POST_DESTROY_ARMOR = new();
        public readonly static CallbackReference<PostRemoveArmor> POST_REMOVE_ARMOR = new();


        public readonly static CallbackReference<PostLevelSetup> POST_LEVEL_SETUP = new();
        public readonly static CallbackReference<PostLevelStart> POST_LEVEL_START = new();
        public readonly static CallbackReference<PostLevelUpdate> POST_LEVEL_UPDATE = new();
        public readonly static CallbackReference<PostLevelClear> POST_LEVEL_CLEAR = new();
        public readonly static CallbackReference<PostPrepareForBattle> POST_PREPARE_FOR_BATTLE = new();
        public readonly static CallbackReference<PostWave> POST_WAVE = new();
        public readonly static CallbackReference<PostHugeWaveEvent> POST_HUGE_WAVE_EVENT = new();
        public readonly static CallbackReference<PostFinalWaveEvent> POST_FINAL_WAVE_EVENT = new();
        public readonly static CallbackReference<PostGameOver> POST_GAME_OVER = new();
        public readonly static CallbackReference<PostEnemySpawned> POST_ENEMY_SPAWNED = new();
    }
}
