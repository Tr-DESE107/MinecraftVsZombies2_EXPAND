using PVZEngine.Base;
using PVZEngine.Level;

namespace PVZEngine.Definitions
{
    public abstract class EntityDefinition : Definition
    {
        public EntityDefinition(string nsp, string name) : base(nsp, name)
        {
        }
        public virtual void Init(Entity entity) { }
        public virtual void Update(Entity entity) { }
        public virtual void PostTakeDamage(DamageResult bodyResult, DamageResult armorResult) { }
        public virtual void PostContactGround(Entity entity) { }
        public virtual void PostLeaveGround(Entity entity) { }
        public virtual void PostCollision(Entity entity, Entity other, int state) { }
        public virtual void PostDeath(Entity entity, DamageInfo damageInfo) { }
        public virtual void PostRemove(Entity entity) { }
        public virtual void PostEquipArmor(Entity entity, Armor slot) { }
        public virtual void PostDestroyArmor(Entity entity, Armor slot, DamageResult damage) { }
        public virtual void PostRemoveArmor(Entity entity, Armor slot) { }
        public virtual NamespaceID GetModelID()
        {
            return GetID().ToModelID("entity");
        }
        public abstract int Type { get; }
    }
}
