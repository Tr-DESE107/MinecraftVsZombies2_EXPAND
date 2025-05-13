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
    public abstract class EntityDefinition : Definition, ICachedDefinition
    {
        public EntityDefinition(string nsp, string name) : base(nsp, name)
        {
        }
        public void AddBehaviourID(NamespaceID behaviour)
        {
            behaviours.Add(behaviour);
        }
        public bool RemoveBehaviourID(NamespaceID behaviour)
        {
            return behaviours.Remove(behaviour);
        }
        public bool HasBehaviourID(EntityBehaviourDefinition behaviour)
        {
            return HasBehaviour(behaviour.GetID());
        }
        public bool HasBehaviourID(NamespaceID id)
        {
            return behaviours.Contains(id);
        }
        public bool HasBehaviour(EntityBehaviourDefinition behaviour)
        {
            return behaviourCaches.Contains(behaviour);
        }
        public bool HasBehaviour(NamespaceID id)
        {
            return behaviourCaches.Exists(b => b.GetID() == id);
        }
        void ICachedDefinition.CacheContents(IGameContent content)
        {
            behaviourCaches.Clear();
            foreach (var behaviourID in behaviours)
            {
                var behaviour = content.GetEntityBehaviourDefinition(behaviourID);
                if (behaviour == null)
                    continue;
                behaviourCaches.Add(behaviour);
            }
        }
        void ICachedDefinition.ClearCaches()
        {
            behaviourCaches.Clear();
        }
        public T GetBehaviour<T>()
        {
            foreach (var behaviour in behaviourCaches)
            {
                if (behaviour is T tBehaviour)
                    return tBehaviour;
            }
            return default;
        }
        public EntityBehaviourDefinition GetBehaviourAt(int behaviour)
        {
            return behaviourCaches[behaviour];
        }
        public int GetBehaviourCount()
        {
            return behaviourCaches.Count;
        }
        public T[] GetBehaviours<T>()
        {
            return behaviourCaches.OfType<T>().ToArray();
        }
        public void Init(Entity entity)
        {
            foreach (var behaviour in behaviourCaches)
            {
                behaviour.Init(entity);
            }
        }
        public void Update(Entity entity)
        {
            foreach (var behaviour in behaviourCaches)
            {
                behaviour.Update(entity);
            }
        }
        public void PreTakeDamage(DamageInput input, CallbackResult result)
        {
            foreach (var behaviour in behaviourCaches)
            {
                behaviour.PreTakeDamage(input, result);
                if (result.IsBreakRequested)
                    break;
            }
        }
        public void PostTakeDamage(DamageOutput result)
        {
            foreach (var behaviour in behaviourCaches)
            {
                behaviour.PostTakeDamage(result);
            }
        }
        public void PostContactGround(Entity entity, Vector3 velocity)
        {
            foreach (var behaviour in behaviourCaches)
            {
                behaviour.PostContactGround(entity, velocity);
            }
        }
        public void PostLeaveGround(Entity entity)
        {
            foreach (var behaviour in behaviourCaches)
            {
                behaviour.PostLeaveGround(entity);
            }
        }
        public void PreCollision(EntityCollision collision, CallbackResult callbackResult)
        {
            foreach (var behaviour in behaviourCaches)
            {
                behaviour.PreCollision(collision, callbackResult);
                if (callbackResult.IsBreakRequested)
                    break;
            }
        }
        public void PostCollision(EntityCollision collision, int state)
        {
            foreach (var behaviour in behaviourCaches)
            {
                behaviour.PostCollision(collision, state);
            }
        }
        public void PostDeath(Entity entity, DeathInfo deathInfo)
        {
            foreach (var behaviour in behaviourCaches)
            {
                behaviour.PostDeath(entity, deathInfo);
            }
        }
        public void PostRemove(Entity entity)
        {
            foreach (var behaviour in behaviourCaches)
            {
                behaviour.PostRemove(entity);
            }
        }
        public void PostEquipArmor(Entity entity, NamespaceID slot, Armor armor)
        {
            foreach (var behaviour in behaviourCaches)
            {
                behaviour.PostEquipArmor(entity, slot, armor);
            }
        }
        public void PostDestroyArmor(Entity entity, NamespaceID slot, Armor armor, ArmorDestroyInfo damage)
        {
            foreach (var behaviour in behaviourCaches)
            {
                behaviour.PostDestroyArmor(entity, slot, armor, damage);
            }
        }
        public void PostRemoveArmor(Entity entity, NamespaceID slot, Armor armor)
        {
            foreach (var behaviour in behaviourCaches)
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
            foreach (var behaviour in behaviourCaches)
            {
                id = behaviour.GetModelID(id);
            }
            return id;
        }
        public AuraEffectDefinition[] GetAuras()
        {
            return behaviourCaches.SelectMany(b => b.GetAuras()).ToArray();
        }
        public PropertyModifier[] GetModifiers()
        {
            return behaviourCaches.SelectMany(b => b.GetModifiers()).ToArray();
        }
        public sealed override string GetDefinitionType() => EngineDefinitionTypes.ENTITY;
        public abstract int Type { get; }
        private List<NamespaceID> behaviours = new List<NamespaceID>();
        private List<EntityBehaviourDefinition> behaviourCaches = new List<EntityBehaviourDefinition>();
    }
}
