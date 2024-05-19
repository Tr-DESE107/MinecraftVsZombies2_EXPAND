namespace PVZEngine
{
    public static class Callbacks
    {
        public readonly static CallbackActionList<Entity> PostEntityInit = new();
        public readonly static CallbackActionList<Entity> PostEntityUpdate = new();
        public readonly static CallbackActionList<Entity> PostEntityContactGround = new();
        public readonly static CallbackActionList<Entity> PostEntityLeaveGround = new();
        public readonly static CallbackActionList<Entity, Entity, int> PostEntityCollision = new();
        public readonly static CallbackActionList<Entity, DamageEffectList, EntityReference> PostEntityDeath = new();
    }
}
