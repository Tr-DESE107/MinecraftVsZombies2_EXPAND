namespace PVZEngine
{
    public static class Callbacks
    {
        public readonly static CallbackActionList<Entity> PostEntityInit = new();
        public readonly static CallbackActionList<Entity> PostEntityUpdate = new();
        public readonly static CallbackActionList<Entity> OnEntityContactGround = new();
        public readonly static CallbackActionList<Entity> OnEntityLeaveGround = new();
    }
}
