using PVZEngine.Armors;
using PVZEngine.Base;
using PVZEngine.Damages;
using PVZEngine.Level;
using UnityEngine;

namespace PVZEngine.Entities
{
    public abstract class EntityBehaviourDefinition : Definition
    {
        protected EntityBehaviourDefinition(string nsp, string name) : base(nsp, name)
        {
        }
        public virtual void Init(Entity entity) { }
        public virtual void Update(Entity entity) { }
        public virtual void PostTakeDamage(DamageOutput result) { }
        public virtual void PostContactGround(Entity entity, Vector3 velocity) { }
        public virtual void PostLeaveGround(Entity entity) { }
        public virtual void PostCollision(Entity entity, Entity other, int state) { }
        public virtual void PostDeath(Entity entity, DamageInput damageInfo) { }
        public virtual void PostRemove(Entity entity) { }
        public virtual void PostEquipArmor(Entity entity, Armor slot) { }
        public virtual void PostDestroyArmor(Entity entity, Armor slot, ArmorDamageResult result) { }
        public virtual void PostRemoveArmor(Entity entity, Armor slot) { }
        public virtual NamespaceID GetModelID(NamespaceID origin)
        {
            return origin;
        }
        public T GetEntityProperty<T>(Entity entity, string name)
        {
            return entity.GetProperty<T>($"{GetID()}/{name}");
        }
        public void SetEntityProperty(Entity entity, string name, object value)
        {
            entity.SetProperty($"{GetID()}/{name}", value);
        }
        public virtual NamespaceID GetMatchEntityID()
        {
            return GetID();
        }
    }
}
