using System.Collections.Generic;
using PVZEngine.Armors;
using PVZEngine.Auras;
using PVZEngine.Base;
using PVZEngine.Callbacks;
using PVZEngine.Damages;
using PVZEngine.Definitions;
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
        public virtual void PreTakeDamage(DamageInput input, CallbackResult result) { }
        public virtual void PostTakeDamage(DamageOutput result) { }
        public virtual void PostContactGround(Entity entity, Vector3 velocity) { }
        public virtual void PostLeaveGround(Entity entity) { }
        public virtual void PreCollision(EntityCollision collision, CallbackResult result) { }
        public virtual void PostCollision(EntityCollision collision, int state) { }
        public virtual void PostDeath(Entity entity, DeathInfo deathInfo) { }
        public virtual void PostRemove(Entity entity) { }
        public virtual void PostEquipArmor(Entity entity, NamespaceID slot, Armor armor) { }
        public virtual void PostDestroyArmor(Entity entity, NamespaceID slot, Armor armor, ArmorDestroyInfo result) { }
        public virtual void PostRemoveArmor(Entity entity, NamespaceID slot, Armor armor) { }
        public virtual NamespaceID GetModelID(NamespaceID origin)
        {
            return origin;
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
        public sealed override string GetDefinitionType() => EngineDefinitionTypes.ENTITY_BEHAVIOUR;
        private List<AuraEffectDefinition> auraDefinitions = new List<AuraEffectDefinition>();
        private List<PropertyModifier> modifiers = new List<PropertyModifier>();
    }
}
