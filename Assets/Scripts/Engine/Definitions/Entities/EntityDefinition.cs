namespace PVZEngine
{
    public abstract class EntityDefinition : Definition
    {
        public virtual void Init(Entity entity) { }
        public virtual void Update(Entity entity) { }
        public virtual void PostTakeDamage(DamageResult bodyResult, DamageResult armorResult) { }
        public virtual void PostContactGround(Entity entity) { }
        public virtual void PostLeaveGround(Entity entity) { }
        public virtual void PostCollision(Entity entity, Entity other, int state) { }
        public virtual void PostDeath(Entity entity, DamageInfo damageInfo) { }
        public virtual void PostRemove(Entity entity) { }
        public abstract int Type { get; }
    }
}
