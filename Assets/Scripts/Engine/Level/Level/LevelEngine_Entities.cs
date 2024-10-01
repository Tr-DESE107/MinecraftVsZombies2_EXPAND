using System;
using System.Collections.Generic;
using System.Linq;
using PVZEngine.Definitions;
using UnityEngine;

namespace PVZEngine.Level
{
    public partial class LevelEngine
    {
        internal void RemoveEntity(Entity entity)
        {
            entities.Remove(entity);
            OnEntityRemove?.Invoke(entity);
        }
        public void CollisionUpdate(Entity ent1, int mask, Entity[] entities)
        {
            var bounds = GetCachedBounds(ent1);
            foreach (var ent2 in entities)
            {
                if (ent1 == ent2)
                    continue;
                if (!EntityCollision.CanCollide(mask, ent2))
                    continue;
                if (!bounds.Intersects(GetCachedBounds(ent2)))
                    continue;
                ent1.Collide(ent2);
            }
            ent1.ClearCollision();
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
            var entityDef = ContentProvider.GetEntityDefinition(entityRef);
            if (entityDef == null)
                return null;
            return Spawn(entityDef, pos, spawner);
        }
        public Entity Spawn<T>(Vector3 pos, Entity spawner) where T : EntityDefinition
        {
            var entityDef = ContentProvider.GetEntityDefinition<T>();
            if (entityDef == null)
                return null;
            return Spawn(entityDef, pos, spawner);
        }
        public Entity FindEntityByID(long id)
        {
            return entities.FirstOrDefault(e => e.ID == id);
        }
        public Entity[] GetEntities(params int[] filterTypes)
        {
            if (filterTypes == null || filterTypes.Length <= 0)
                return entities.ToArray();
            return FindEntities(e => filterTypes.Contains(e.Type));
        }
        public Entity[] FindEntities(EntityDefinition def)
        {
            if (def == null)
                return Array.Empty<Entity>();
            return FindEntities(e => e.Definition == def);
        }
        public Entity[] FindEntities(NamespaceID id)
        {
            if (id == null)
                return Array.Empty<Entity>();
            return FindEntities(e => e.Definition.GetID() == id);
        }
        public Entity[] FindEntities(Func<Entity, bool> predicate)
        {
            return entities.Where(predicate).ToArray();
        }
        public bool EntityExists(EntityDefinition def)
        {
            return entities.Exists(e => e.Definition == def);
        }
        public bool EntityExists(NamespaceID id)
        {
            return entities.Exists(e => e.Definition.GetID() == id);
        }
        public bool EntityExists(Predicate<Entity> predicate)
        {
            return entities.Exists(predicate);
        }
        private Bounds GetCachedBounds(Entity entity)
        {
            if (!collisionCachedBounds.TryGetValue(entity.ID, out var bounds))
            {
                bounds = entity.GetBounds();
                collisionCachedBounds.Add(entity.ID, bounds);
            }
            return bounds;
        }
        private long AllocEntityID()
        {
            long id = currentEntityID;
            currentEntityID++;
            return id;
        }
        #region 事件
        public Action<Entity> OnEntitySpawn;
        public Action<Entity> OnEntityRemove;
        #endregion
        private long currentEntityID = 1;
        private List<Entity> entities = new List<Entity>();
        private Dictionary<long, Bounds> collisionCachedBounds = new Dictionary<long, Bounds>();
    }
}