using System.Collections.Generic;
using System.Linq;
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
        public EntityBehaviourDefinition GetBehaviourAt(int behaviour)
        {
            return behaviours[behaviour];
        }
        public int GetBehaviourCount()
        {
            return behaviours.Count;
        }
        public T[] GetBehaviours<T>()
        {
            return behaviours.OfType<T>().ToArray();
        }
        public void Init(Entity entity)
        {
            foreach (var behaviour in behaviours)
            {
                behaviour.Init(entity);
            }
        }
        public void Update(Entity entity)
        {
            foreach (var behaviour in behaviours)
            {
                behaviour.Update(entity);
            }
        }
        public void PreTakeDamage(DamageInput input, CallbackResult result)
        {
            foreach (var behaviour in behaviours)
            {
                behaviour.PreTakeDamage(input, result);
                if (result.IsBreakRequested)
                    break;
            }
        }
        public void PostTakeDamage(DamageOutput result)
        {
            foreach (var behaviour in behaviours)
            {
                behaviour.PostTakeDamage(result);
            }
        }
        public void PostContactGround(Entity entity, Vector3 velocity)
        {
            foreach (var behaviour in behaviours)
            {
                behaviour.PostContactGround(entity, velocity);
            }
        }
        public void PostLeaveGround(Entity entity)
        {
            foreach (var behaviour in behaviours)
            {
                behaviour.PostLeaveGround(entity);
            }
        }
        public void PreCollision(EntityCollision collision, CallbackResult callbackResult)
        {
            foreach (var behaviour in behaviours)
            {
                behaviour.PreCollision(collision, callbackResult);
                if (callbackResult.IsBreakRequested)
                    break;
            }
        }
        public void PostCollision(EntityCollision collision, int state)
        {
            foreach (var behaviour in behaviours)
            {
                behaviour.PostCollision(collision, state);
            }
        }
        public void PostDeath(Entity entity, DeathInfo deathInfo)
        {
            foreach (var behaviour in behaviours)
            {
                behaviour.PostDeath(entity, deathInfo);
            }
        }
        public void PostRemove(Entity entity)
        {
            foreach (var behaviour in behaviours)
            {
                behaviour.PostRemove(entity);
            }
        }
        public void PostEquipArmor(Entity entity, NamespaceID slot, Armor armor)
        {
            foreach (var behaviour in behaviours)
            {
                behaviour.PostEquipArmor(entity, slot, armor);
            }
        }
        public void PostDestroyArmor(Entity entity, NamespaceID slot, Armor armor, ArmorDestroyInfo damage)
        {
            foreach (var behaviour in behaviours)
            {
                behaviour.PostDestroyArmor(entity, slot, armor, damage);
            }
        }
        public void PostRemoveArmor(Entity entity, NamespaceID slot, Armor armor)
        {
            foreach (var behaviour in behaviours)
            {
                behaviour.PostRemoveArmor(entity, slot, armor);
            }
        }
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
        public sealed override string GetDefinitionType() => EngineDefinitionTypes.ENTITY;
        public abstract int Type { get; }
        private List<EntityBehaviourDefinition> behaviours = new List<EntityBehaviourDefinition>();
    }
}
