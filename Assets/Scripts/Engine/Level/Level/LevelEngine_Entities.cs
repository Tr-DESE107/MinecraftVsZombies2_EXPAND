using System;
using System.Collections.Generic;
using System.Linq;
using PVZEngine.Entities;
using UnityEngine;

namespace PVZEngine.Level
{
    public partial class LevelEngine
    {
        internal void RemoveEntity(Entity entity)
        {
            entities.Remove(entity);
            entityTrash.Add(entity);
            OnEntityRemove?.Invoke(entity);
        }
        public void CollisionUpdate(Entity ent1, Entity[] entities)
        {
            int maskHostile = ent1.CollisionMaskHostile;
            int maskFriendly = ent1.CollisionMaskFriendly;
            int ent1Faction = ent1.Cache.Faction;
            foreach (var ent2 in entities)
            {
                if (ent1 == ent2)
                    continue;
                var mask = ent2.IsHostile(ent1Faction) ? maskHostile : maskFriendly;
                if (!EntityCollisionHelper.CanCollide(mask, ent2))
                    continue;
                ent1.DoCollision(ent2);
            }
            ent1.ExitCollision(this);
        }
        public Entity Spawn(EntityDefinition entityDef, Vector3 pos, Entity spawner)
        {
            long id = AllocEntityID();
            var spawned = new Entity(this, id, new EntityReferenceChain(spawner), entityDef, entityRandom.Next());
            spawned.Position = pos;
            entities.Add(spawned);
            OnEntitySpawn?.Invoke(spawned);
            spawned.Init(spawner);
            return spawned;
        }
        public Entity Spawn(NamespaceID entityRef, Vector3 pos, Entity spawner)
        {
            var entityDef = Content.GetEntityDefinition(entityRef);
            if (entityDef == null)
                return null;
            return Spawn(entityDef, pos, spawner);
        }
        public Entity[] GetEntities(params int[] filterTypes)
        {
            if (filterTypes == null || filterTypes.Length <= 0)
                return entities.ToArray();
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
            return entities.Where(predicate).ToArray();
        }
        public void FindEntitiesNonAlloc(Func<Entity, bool> predicate, List<Entity> results)
        {
            foreach (var entity in entities)
            {
                if (predicate(entity))
                {
                    results.Add(entity);
                }
            }
        }
        public Entity FindEntityByID(long id)
        {
            foreach (var entity in entities)
            {
                if (entity.ID == id)
                    return entity;
            }
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
            foreach (var entity in entities)
            {
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
            foreach (var entity in entities)
            {
                if (!predicate(entity))
                    continue;
                var key = keySelector(entity);
                if (targetEntity != null && comparer.Compare(targetKey, key) < 0)
                    continue;
                targetEntity = entity;
            }
            return targetEntity;
        }
        public Entity FindFirstEntityWithTheMost<TKey>(Func<Entity, bool> predicate, Func<Entity, TKey> keySelector)
        {
            Entity targetEntity = null;
            TKey targetKey = default;
            var comparer = Comparer<TKey>.Default;
            foreach (var entity in entities)
            {
                if (!predicate(entity))
                    continue;
                var key = keySelector(entity);
                if (targetEntity != null && comparer.Compare(targetKey, key) > 0)
                    continue;
                targetEntity = entity;
            }
            return targetEntity;
        }
        public bool EntityExists(long id)
        {
            return entities.Exists(predicate);

            bool predicate(Entity e)
            {
                return e.ID == id;
            }
        }
        public bool EntityExists(EntityDefinition def)
        {
            return entities.Exists(predicate);

            bool predicate(Entity e)
            {
                return e.Definition == def;
            }
        }
        public bool EntityExists(NamespaceID id)
        {
            return entities.Exists(predicate);

            bool predicate(Entity e)
            {
                return e.IsEntityOf(id);
            }
        }
        public bool EntityExists(Predicate<Entity> predicate)
        {
            return entities.Exists(predicate);
        }
        private long AllocEntityID()
        {
            long id = currentEntityID;
            currentEntityID++;
            return id;
        }
        private Entity FindEntityInTrash(long id)
        {
            foreach (var entity in entityTrash)
            {
                if (entity.ID == id)
                    return entity;
            }
            return null;
        }
        private void ClearEntityTrash()
        {
            entityTrash.Clear();
        }
        #region 事件
        public Action<Entity> OnEntitySpawn;
        public Action<Entity> OnEntityRemove;
        #endregion
        private long currentEntityID = 1;
        private List<Entity> entities = new List<Entity>();
        private List<Entity> entityTrash = new List<Entity>();
    }
}