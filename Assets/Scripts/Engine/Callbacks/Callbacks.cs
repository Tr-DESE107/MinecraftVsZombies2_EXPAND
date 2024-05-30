namespace PVZEngine
{
    public static class Callbacks
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


        public readonly static CallbackActionList<Level> PostLevelStart = new();
        public readonly static CallbackActionList<Level> PostLevelUpdate = new();
        public readonly static CallbackActionList<Level> PostPrepareForBattle = new();
        public readonly static CallbackActionList<Level, int> PostWave = new();
        public readonly static CallbackActionList<Level> PostHugeWave = new();
        public readonly static CallbackActionList<Level> PostFinalWave = new();

        public readonly static CallbackActionList<Entity> PostEnemySpawned = new();
    }
}
