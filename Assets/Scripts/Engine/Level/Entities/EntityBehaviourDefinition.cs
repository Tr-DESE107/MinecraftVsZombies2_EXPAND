using System.Collections.Generic;
using PVZEngine.Armors;
using PVZEngine.Auras;
using PVZEngine.Base;
using PVZEngine.Damages;
using PVZEngine.Modifiers;
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
        public virtual void PreTakeDamage(DamageInput input) { }
        public virtual void PostTakeDamage(DamageOutput result) { }
        public virtual void PostContactGround(Entity entity, Vector3 velocity) { }
        public virtual void PostLeaveGround(Entity entity) { }
        public virtual bool PreCollision(EntityCollision collision) { return true; }
        public virtual void PostCollision(EntityCollision collision, int state) { }
        public virtual void PostDeath(Entity entity, DeathInfo deathInfo) { }
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
        public AuraEffectDefinition[] GetAuras()
        {
            return auraDefinitions.ToArray();
        }
        protected void AddAura(AuraEffectDefinition aura)
        {
            auraDefinitions.Add(aura);
        }
        public PropertyModifier[] GetModifiers()
        {
            return modifiers.ToArray();
        }
        protected void AddModifier(PropertyModifier modifier)
        {
            modifiers.Add(modifier);
        }
        private List<AuraEffectDefinition> auraDefinitions = new List<AuraEffectDefinition>();
        private List<PropertyModifier> modifiers = new List<PropertyModifier>();
    }
}
