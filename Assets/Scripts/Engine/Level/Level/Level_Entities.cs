using System;
using System.Collections.Generic;
using System.Linq;
using PVZEngine.Definitions;
using UnityEngine;

namespace PVZEngine.LevelManaging
{
    public partial class Level
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
            int id = AllocEntityID();
            var spawned = new Entity(this, id, new EntityReferenceChain(spawner), entityDef, entityRandom.Next());
            spawned.Pos = pos;
            entities.Add(spawned);
            OnEntitySpawn?.Invoke(spawned);
            spawned.Init(spawner);
            return spawned;
        }
        public Entity Spawn(NamespaceID entityRef, Vector3 pos, Entity spawner)
        {
            var entityDef = Game.GetEntityDefinition(entityRef);
            if (entityDef == null)
                return null;
            return Spawn(entityDef, pos, spawner);
        }
        public Entity Spawn<T>(Vector3 pos, Entity spawner) where T : EntityDefinition
        {
            var entityDef = Game.GetEntityDefinition<T>();
            if (entityDef == null)
                return null;
            return Spawn(entityDef, pos, spawner);
        }
        public Entity FindEntityByID(int id)
        {
            return entities.FirstOrDefault(e => e.ID == id);
        }
        public Entity[] GetEntities(params int[] filterTypes)
        {
            if (filterTypes == null || filterTypes.Length <= 0)
                return entities.ToArray();
            return FindEntities(e => filterTypes.Contains(e.Type));
        }
        public Entity[] FindEntities(Func<Entity, bool> predicate)
        {
            return entities.Where(predicate).ToArray();
        }
        public void Explode(Vector3 center, float radius, int faction, float amount, DamageEffectList effects, EntityReferenceChain source)
        {
            foreach (Entity entity in GetEntities())
            {
                if (entity.IsEnemy(faction) && Detection.IsInSphere(entity, center, radius))
                {
                    entity.TakeDamage(amount, effects, source);
                }
            }
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
        private int AllocEntityID()
        {
            int id = currentEntityID;
            currentEntityID++;
            return id;
        }
        #region 事件
        public Action<Entity> OnEntitySpawn;
        public Action<Entity> OnEntityRemove;
        #endregion
        private int currentEntityID = 1;
        private List<Entity> entities = new List<Entity>();
        private Dictionary<int, Bounds> collisionCachedBounds = new Dictionary<int, Bounds>();
    }
    public struct ColliderInfo
    {
        public ColliderInfo(Entity entity)
        {
            this.entity = entity;
            mask = entity.CollisionMask;
        }
        public Entity entity;
        public int mask;
    }
}