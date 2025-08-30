using System;
using PVZEngine.Damages;
using PVZEngine.Level;
using UnityEngine;

namespace PVZEngine.Entities
{
    public static class EngineEntityExt
    {
        public static void SetBehaviourField<T>(this Entity entity, NamespaceID id, PropertyKey<T> name, T value)
        {
            entity.SetProperty(name, value);
        }
        public static T GetBehaviourField<T>(this Entity entity, NamespaceID id, PropertyKey<T> name)
        {
            return entity.GetProperty<T>(name);
        }
        public static void SetBehaviourField<T>(this Entity entity, PropertyKey<T> name, T value)
        {
            entity.SetProperty(name, value);
        }
        public static T GetBehaviourField<T>(this Entity entity, PropertyKey<T> name)
        {
            return entity.GetProperty<T>(name);
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
        public static Entity Spawn(this Entity entity, NamespaceID id, Vector3 position, SpawnParams param)
        {
            return entity.Level.Spawn(id, position, entity, param);
        }
        public static Entity Spawn(this Entity entity, NamespaceID id, Vector3 position)
        {
            return entity.Level.Spawn(id, position, entity);
        }
        public static Entity Spawn(this Entity entity, NamespaceID id, Vector3 position, int seed, SpawnParams param = null)
        {
            return entity.Level.Spawn(id, position, entity, seed, param);
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
        public static bool IsEntitySpawnedByEntity(this ILevelSourceReference reference, LevelEngine level, Func<ILevelSourceReference, EntityDefinition, bool> predicate, bool trackableOnly = true)
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
                if (source is not EntitySourceReference entitySource)
                    break;
                var definition = level.Content.GetEntityDefinition(entitySource.DefinitionID);
                if (definition == null)
                    break;
                if (predicate(source, definition))
                {
                    return true;
                }
                if (trackableOnly && !definition.CanEntityTrackSpawnSource())
                {
                    break;
                }
                source = source.Parent;
            }
            return false;
        }
        public static bool CanEntityTrackSpawnSource(this EntityDefinition definition)
        {
            return definition.Type == EntityTypes.PROJECTILE || definition.Type == EntityTypes.EFFECT || definition.Type == EntityTypes.PICKUP;
        }
    }
    public enum FactionTarget
    {
        Any,
        Friendly,
        Hostile
    }
}
