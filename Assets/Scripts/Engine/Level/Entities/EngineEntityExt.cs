using System;
using PVZEngine.Damages;
using PVZEngine.Level;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

namespace PVZEngine.Entities
{
    public static class EngineEntityExt
    {
        public static void SetBehaviourField(this Entity entity, NamespaceID id, string name, object value)
        {
            entity.SetField(id.ToString(), name, value);
        }
        public static T GetBehaviourField<T>(this Entity entity, NamespaceID id, string name)
        {
            return entity.GetField<T>(id.ToString(), name);
        }
        public static ShellDefinition GetShellDefinition(this Entity entity)
        {
            var shellID = entity.GetShellID();
            return entity.Level.Content.GetShellDefinition(shellID);
        }
        public static NamespaceID GetDefinitionID(this Entity entity)
        {
            return entity?.Definition?.GetID();
        }
        public static Entity Spawn(this Entity entity, NamespaceID id, Vector3 position)
        {
            return entity.Level.Spawn(id, position, entity);
        }
        public static Entity Spawn(this Entity entity, NamespaceID id, Vector3 position, int seed)
        {
            return entity.Level.Spawn(id, position, entity, seed);
        }
        public static bool IsHostile(int faction1, int faction2)
        {
            return faction1 != faction2;
        }
        public static bool IsFriendly(int faction1, int faction2)
        {
            return faction1 == faction2;
        }
        public static bool IsFactionTarget(this Entity entity, Entity other, FactionTarget target)
        {
            var faction2 = other.Cache.Faction;
            return IsFactionTarget(entity, faction2, target);
        }
        public static bool IsFactionTarget(this Entity entity, int faction2, FactionTarget target)
        {
            var faction1 = entity.Cache.Faction;
            return IsFactionTarget(faction1, faction2, target);
        }
        public static bool IsFactionTarget(int faction1, int faction2, FactionTarget target)
        {
            switch (target)
            {
                case FactionTarget.Any:
                    return true;
                case FactionTarget.Friendly:
                    return IsFriendly(faction1, faction2);
                case FactionTarget.Hostile:
                    return IsHostile(faction1, faction2);
            }
            return false;
        }
        public static bool ExistsAndAlive(this Entity entity)
        {
            return entity != null && entity.Exists() && !entity.IsDead;
        }
        public static bool IsEntitySourceOf(this EntityReferenceChain reference, LevelEngine level, Func<EntityReferenceChain, EntityDefinition, bool> predicate)
        {
            if (reference == null)
                return false;
            if (level == null)
                throw new ArgumentNullException(nameof(level));
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            var source = reference;
            while (source != null)
            {
                var defID = source.DefinitionID;
                var entityDef = level.Content.GetEntityDefinition(defID);
                if (entityDef != null)
                {
                    if (predicate(source, entityDef))
                    {
                        return true;
                    }
                    if (entityDef.Type != EntityTypes.PROJECTILE && entityDef.Type != EntityTypes.EFFECT && entityDef.Type != EntityTypes.PICKUP)
                    {
                        break;
                    }
                }
                source = source.SpawnerReference;
            }
            return false;
        }
    }
    public enum FactionTarget
    {
        Any,
        Friendly,
        Hostile
    }
}
