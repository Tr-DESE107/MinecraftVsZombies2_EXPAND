using System;
using System.Collections.Generic;
using System.Linq;
using PVZEngine.Base;
using PVZEngine.Entities;
using PVZEngine.Level.Collisions;
using UnityEngine;

namespace PVZEngine.Level
{
    public partial class LevelEngine
    {
        private void AddEntity(Entity entity)
        {
        }
        internal void RemoveEntity(Entity entity)
        {
            var id = entity.ID;
            entities.Remove(id);
            entityTrash.Add(id, entity);
            collisionSystem.DestroyEntity(entity);
            OnEntityRemove?.Invoke(entity);
        }
        public Entity Spawn(EntityDefinition entityDef, Vector3 pos, Entity spawner, SpawnParams param = null)
        {
            return Spawn(entityDef, pos, spawner, entityRandom.Next(), param);
        }
        public Entity Spawn(NamespaceID entityRef, Vector3 pos, Entity spawner, SpawnParams param = null)
        {
            var entityDef = Content.GetEntityDefinition(entityRef);
            if (entityDef == null)
                return null;
            return Spawn(entityDef, pos, spawner, param);
        }
        public Entity Spawn(EntityDefinition entityDef, Vector3 pos, Entity spawner, int seed, SpawnParams param = null)
        {
            long id = AllocEntityID();
            var spawned = new Entity(this, id, new EntityReferenceChain(spawner), entityDef, seed);
            spawned.Position = pos;
            collisionSystem.InitEntity(spawned);
            if (param != null)
            {
                param.Apply(spawned);
            }
            entities.Add(id, spawned);
            OnEntitySpawn?.Invoke(spawned);
            spawned.Init(spawner);
            return spawned;
        }
        public Entity Spawn(NamespaceID entityRef, Vector3 pos, Entity spawner, int seed, SpawnParams param = null)
        {
            var entityDef = Content.GetEntityDefinition(entityRef);
            if (entityDef == null)
                return null;
            return Spawn(entityDef, pos, spawner, seed, param);
        }

        #region 查询
        public Entity[] GetEntities(params int[] filterTypes)
        {
            if (filterTypes == null || filterTypes.Length <= 0)
                return entities.Values.ToArray();
            return FindEntities(predicate);

            bool predicate(Entity e)
            {
                return filterTypes.Contains(e.Type);
            }
        }
        public Entity[] FindEntities(EntityDefinition def)
        {
            if (def == null)
                return Array.Empty<Entity>();
            return FindEntities(predicate);

            bool predicate(Entity e)
            {
                return e.Definition == def;
            }
        }
        public Entity[] FindEntities(NamespaceID id)
        {
            if (!NamespaceID.IsValid(id))
                return Array.Empty<Entity>();
            return FindEntities(predicate);

            bool predicate(Entity e)
            {
                return e.IsEntityOf(id);
            }
        }
        public Entity[] FindEntities(Func<Entity, bool> predicate)
        {
            return entities.Values.Where(predicate).ToArray();
        }
        public IEnumerable<Entity> EnumerateEntities()
        {
            foreach (var pair in entities)
            {
                yield return pair.Value;
            }
        }
        public int GetEntityCount(EntityDefinition def)
        {
            if (def == null)
                return 0;
            return GetEntityCount(predicate);

            bool predicate(Entity e)
            {
                return e.Definition == def;
            }
        }
        public int GetEntityCount(NamespaceID id)
        {
            if (!NamespaceID.IsValid(id))
                return 0;
            return GetEntityCount(predicate);

            bool predicate(Entity e)
            {
                return e.IsEntityOf(id);
            }
        }
        public int GetEntityCount(Func<Entity, bool> predicate)
        {
            int count = 0;
            foreach (var pair in entities)
            {
                if (predicate(pair.Value))
                {
                    count++;
                }
            }
            return count;
        }
        public void FindEntitiesNonAlloc(Func<Entity, bool> predicate, List<Entity> results)
        {
            foreach (var pair in entities)
            {
                if (predicate(pair.Value))
                {
                    results.Add(pair.Value);
                }
            }
        }
        public Entity FindEntityByID(long id)
        {
            if (entities.TryGetValue(id, out var entity))
                return entity;
            return FindEntityInTrash(id);
        }
        public Entity FindFirstEntity(EntityDefinition def)
        {
            if (def == null)
                return null;
            return FindFirstEntity(predicate);

            bool predicate(Entity e)
            {
                return e.Definition == def;
            }
        }
        public Entity FindFirstEntity(NamespaceID id)
        {
            if (!NamespaceID.IsValid(id))
                return null;
            return FindFirstEntity(predicate);

            bool predicate(Entity e)
            {
                return e.IsEntityOf(id);
            }
        }
        public Entity FindFirstEntity(Func<Entity, bool> predicate)
        {
            foreach (var pair in entities)
            {
                var entity = pair.Value;
                if (predicate(entity))
                    return entity;
            }
            return null;
        }
        public Entity FindFirstEntityWithTheLeast<TKey>(Func<Entity, bool> predicate, Func<Entity, TKey> keySelector)
        {
            Entity targetEntity = null;
            TKey targetKey = default;
            var comparer = Comparer<TKey>.Default;
            foreach (var pair in entities)
            {
                var entity = pair.Value;
                if (!predicate(entity))
                    continue;
                var key = keySelector(entity);
                if (targetEntity != null && comparer.Compare(targetKey, key) < 0)
                    continue;
                targetEntity = entity;
                targetKey = key;
            }
            return targetEntity;
        }
        public Entity FindFirstEntityWithTheMost<TKey>(Func<Entity, bool> predicate, Func<Entity, TKey> keySelector)
        {
            Entity targetEntity = null;
            TKey targetKey = default;
            var comparer = Comparer<TKey>.Default;
            foreach (var pair in entities)
            {
                var entity = pair.Value;
                if (!predicate(entity))
                    continue;
                var key = keySelector(entity);
                if (targetEntity != null && comparer.Compare(targetKey, key) > 0)
                    continue;
                targetEntity = entity;
                targetKey = key;
            }
            return targetEntity;
        }
        public bool EntityExists(long id)
        {
            return EntityExists(predicate);

            bool predicate(Entity e)
            {
                return e.ID == id;
            }
        }
        public bool EntityExists(EntityDefinition def)
        {
            return EntityExists(predicate);

            bool predicate(Entity e)
            {
                return e.Definition == def;
            }
        }
        public bool EntityExists(NamespaceID id)
        {
            return EntityExists(predicate);

            bool predicate(Entity e)
            {
                return e.IsEntityOf(id);
            }
        }
        public bool EntityExists(Predicate<Entity> predicate)
        {
            foreach (var pair in entities)
            {
                var entity = pair.Value;
                if (predicate(entity))
                    return true;
            }
            return false;
        }
        #endregion

        #region 碰撞检测
        public void UpdateEntityCollisionDetection(Entity entity)
        {
            collisionSystem.UpdateEntityDetection(entity);
        }
        public void UpdateEntityCollisionPosition(Entity entity)
        {
            collisionSystem.UpdateEntityPosition(entity);
        }
        public void UpdateEntityCollisionSize(Entity entity)
        {
            collisionSystem.UpdateEntitySize(entity);
        }
        public IEntityCollider[] OverlapBox(Vector3 center, Vector3 size, int faction, int hostileMask, int friendlyMask)
        {
            return collisionSystem.OverlapBox(center, size, faction, hostileMask, friendlyMask);
        }
        public void OverlapBoxNonAlloc(Vector3 center, Vector3 size, int faction, int hostileMask, int friendlyMask, List<IEntityCollider> results)
        {
            collisionSystem.OverlapBoxNonAlloc(center, size, faction, hostileMask, friendlyMask, results);
        }
        public IEntityCollider[] OverlapSphere(Vector3 center, float radius, int faction, int hostileMask, int friendlyMask)
        {
            return collisionSystem.OverlapSphere(center, radius, faction, hostileMask, friendlyMask);
        }
        public void OverlapSphereNonAlloc(Vector3 center, float radius, int faction, int hostileMask, int friendlyMask, List<IEntityCollider> results)
        {
            collisionSystem.OverlapSphereNonAlloc(center, radius, faction, hostileMask, friendlyMask, results);
        }
        public IEntityCollider[] OverlapCapsule(Vector3 point0, Vector3 point1, float radius, int faction, int hostileMask, int friendlyMask)
        {
            return collisionSystem.OverlapCapsule(point0, point1, radius, faction, hostileMask, friendlyMask);
        }
        public void OverlapCapsuleNonAlloc(Vector3 point0, Vector3 point1, float radius, int faction, int hostileMask, int friendlyMask, List<IEntityCollider> results)
        {
            collisionSystem.OverlapCapsuleNonAlloc(point0, point1, radius, faction, hostileMask, friendlyMask, results);
        }
        public IEntityCollider AddEntityCollider(Entity entity, ColliderConstructor info)
        {
            return collisionSystem.CreateCustomCollider(entity, info);
        }
        public bool RemoveEntityCollider(Entity entity, string name)
        {
            return collisionSystem.RemoveCollider(entity, name);
        }
        public IEntityCollider GetEntityCollider(Entity entity, string name)
        {
            return collisionSystem.GetCollider(entity, name);
        }
        public void GetEntityCurrentCollisions(Entity entity, List<EntityCollision> collisions)
        {
            collisionSystem.GetCurrentCollisions(entity, collisions);
        }
        #endregion

        private long AllocEntityID()
        {
            long id = currentEntityID;
            currentEntityID++;
            return id;
        }
        private Entity FindEntityInTrash(long id)
        {
            if (entityTrash.TryGetValue(id, out var entity))
                return entity;
            return null;
        }
        private void ClearEntityTrash()
        {
            entityTrash.Clear();
        }

        private void UpdateEntities()
        {
            entityUpdateBuffer.Clear();
            entityUpdateBuffer.CopyFrom(entities.OrderBy(e => e.Key).Select(e => e.Value));
            for (int i = 0; i < entityUpdateBuffer.Count; i++)
            {
                var entity = entityUpdateBuffer[i];
                entity.Update();
            }
        }
        #region 碰撞
        private void CollisionUpdate()
        {
            collisionSystem.Update();
        }
        #endregion

        #region 事件
        public Action<Entity> OnEntitySpawn;
        public Action<Entity> OnEntityRemove;
        #endregion
        private long currentEntityID = 1;
        private SortedDictionary<long, Entity> entities = new SortedDictionary<long, Entity>();
        private Dictionary<long, Entity> entityTrash = new Dictionary<long, Entity>();
        private ArrayBuffer<Entity> entityUpdateBuffer = new ArrayBuffer<Entity>(2048);
        private ICollisionSystem collisionSystem;
    }
}