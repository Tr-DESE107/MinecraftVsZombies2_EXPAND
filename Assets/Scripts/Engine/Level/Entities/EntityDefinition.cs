using System.Collections.Generic;
using System.Linq;
using PVZEngine.Armors;
using PVZEngine.Auras;
using PVZEngine.Base;
using PVZEngine.Damages;
using PVZEngine.Modifiers;
using UnityEngine;

namespace PVZEngine.Entities
{
    public abstract class EntityDefinition : Definition
    {
        public EntityDefinition(string nsp, string name) : base(nsp, name)
        {
        }
        public void AddBehaviour(EntityBehaviourDefinition behaviour)
        {
            behaviours.Add(behaviour);
        }
        public bool RemoveBehaviour(EntityBehaviourDefinition behaviour)
        {
            return behaviours.Remove(behaviour);
        }
        public bool HasBehaviour(EntityBehaviourDefinition behaviour)
        {
            return behaviours.Contains(behaviour);
        }
        public bool HasBehaviour(NamespaceID id)
        {
            return behaviours.Exists(b => b.GetID() == id);
        }
        public bool HasBehaviour<T>()
        {
            return behaviours.Exists(b => b is T);
        }
        public EntityBehaviourDefinition GetBehaviour(NamespaceID id)
        {
            return behaviours.FirstOrDefault(b => b.GetID() == id);
        }
        public T GetBehaviour<T>()
        {
            foreach (var behaviour in behaviours)
            {
                if (behaviour is T tBehaviour)
                    return tBehaviour;
            }
            return default;
        }
        public EntityBehaviourDefinition[] GetBehaviours()
        {
            return behaviours.ToArray();
        }
        public T[] GetBehaviours<T>()
        {
            return behaviours.OfType<T>().ToArray();
        }
        public void Init(Entity entity) { behaviours.ForEach(b => b.Init(entity)); }
        public void Update(Entity entity) { behaviours.ForEach(b => b.Update(entity)); }
        public void PreTakeDamage(DamageInput input) { behaviours.ForEach(b => b.PreTakeDamage(input)); }
        public void PostTakeDamage(DamageOutput result) { behaviours.ForEach(b => b.PostTakeDamage(result)); }
        public void PostContactGround(Entity entity, Vector3 velocity) { behaviours.ForEach(b => b.PostContactGround(entity, velocity)); }
        public void PostLeaveGround(Entity entity) { behaviours.ForEach(b => b.PostLeaveGround(entity)); }
        public bool PreCollision(EntityCollision collision)
        {
            foreach (var behaviour in behaviours)
            {
                if (!behaviour.PreCollision(collision))
                    return false;
            }
            return true;
        }
        public void PostCollision(EntityCollision collision, int state) { behaviours.ForEach(b => b.PostCollision(collision, state)); }
        public void PostDeath(Entity entity, DeathInfo deathInfo) { behaviours.ForEach(b => b.PostDeath(entity, deathInfo)); }
        public void PostRemove(Entity entity) { behaviours.ForEach(b => b.PostRemove(entity)); }
        public void PostEquipArmor(Entity entity, Armor armor) { behaviours.ForEach(b => b.PostEquipArmor(entity, armor)); }
        public void PostDestroyArmor(Entity entity, Armor armor, ArmorDamageResult damage) { behaviours.ForEach(b => b.PostDestroyArmor(entity, armor, damage)); }
        public void PostRemoveArmor(Entity entity, Armor armor) { behaviours.ForEach(b => b.PostRemoveArmor(entity, armor)); }
        public NamespaceID GetModelID()
        {
            NamespaceID id;
            if (!TryGetProperty<NamespaceID>(EngineEntityProps.MODEL_ID, out id) || !NamespaceID.IsValid(id))
            {
                id = GetID().ToModelID(EngineModelID.TYPE_ENTITY);
            }
            foreach (var behaviour in behaviours)
            {
                id = behaviour.GetModelID(id);
            }
            return id;
        }
        public AuraEffectDefinition[] GetAuras()
        {
            return behaviours.SelectMany(b => b.GetAuras()).ToArray();
        }
        public PropertyModifier[] GetModifiers()
        {
            return behaviours.SelectMany(b => b.GetModifiers()).ToArray();
        }
        public abstract int Type { get; }
        private List<EntityBehaviourDefinition> behaviours = new List<EntityBehaviourDefinition>();
    }
}
