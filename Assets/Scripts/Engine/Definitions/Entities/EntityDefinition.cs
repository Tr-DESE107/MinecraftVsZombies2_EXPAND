namespace PVZEngine
{
    public abstract class EntityDefinition : Definition
    {
        public virtual void Init(Entity entity) { }
        public virtual void Update(Entity entity) { }
        public virtual void PostContactGround(Entity entity) { }
        public virtual void PostLeaveGround(Entity entity) { }
        public virtual void PostEntityCollisionStay(Entity entity, Entity other, bool actively) { }
        public virtual void PostEntityCollisionExit(Entity entity, Entity other, bool actively) { }
        public abstract int Type { get; }
    }
}
