using PVZEngine.Callbacks;

namespace PVZEngine.Level
{
    public static class LevelCallbacks
    {
        public readonly static CallbackActionList<Entity> PostEntityInit = new();
        public readonly static CallbackActionList<Entity> PostEntityUpdate = new();
        public readonly static CallbackActionList<Entity> PostEntityContactGround = new();
        public readonly static CallbackActionList<Entity> PostEntityLeaveGround = new();
        public readonly static CallbackActionList<DamageResult, DamageResult> PostEntityTakeDamage = new();
        public readonly static CallbackActionList<Entity, Entity, int> PostEntityCollision = new();
        public readonly static CallbackActionList<Entity, DamageInfo> PostEntityDeath = new();
        public readonly static CallbackActionList<Entity> PostEntityRemove = new();
        public readonly static CallbackActionList<Entity, Armor> PostEquipArmor = new();
        public readonly static CallbackActionList<Entity, Armor, DamageResult> PostDestroyArmor = new();
        public readonly static CallbackActionList<Entity, Armor> PostRemoveArmor = new();


        public readonly static CallbackActionList<LevelEngine> PostLevelStart = new();
        public readonly static CallbackActionList<LevelEngine> PostLevelUpdate = new();
        public readonly static CallbackActionList<LevelEngine> PostPrepareForBattle = new();
        public readonly static CallbackActionList<LevelEngine, int> PostWave = new();
        public readonly static CallbackActionList<LevelEngine> PostHugeWave = new();
        public readonly static CallbackActionList<LevelEngine> PostFinalWave = new();

        public readonly static CallbackActionList<Entity> PostEnemySpawned = new();
    }
}
