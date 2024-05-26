using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using PVZEngine.Serialization;
using UnityEngine;

namespace PVZEngine
{
    public partial class Game
    {
        internal void RemoveEntity(Entity entity)
        {
            entities.Remove(entity);
            OnEntityRemove?.Invoke(entity);
        }
        public void CollisionUpdate(Entity ent1, Entity[] entities)
        {
            foreach (var ent2 in entities)
            {
                if (ent1 == ent2)
                    continue;
                if (!EntityCollision.CanCollide(ent1, ent2))
                    continue;
                if (!ent1.GetBounds().Intersects(ent2.GetBounds()))
                    continue;
                ent1.Collide(ent2);
            }
            ent1.ClearCollision();
        }
        public Entity Spawn(EntityDefinition entityDef, Vector3 pos, Entity spawner)
        {
            Entity spawned;
            int id = AllocEntityID();
            switch (entityDef.Type)
            {
                case EntityTypes.PLANT:
                    spawned = new Contraption(this, id, entityDef, entityRandom.Next());
                    break;
                case EntityTypes.ENEMY:
                    spawned = new Enemy(this, id, entityDef, entityRandom.Next());
                    break;
                case EntityTypes.OBSTACLE:
                    spawned = new Obstacle(this, id, entityDef, entityRandom.Next());
                    break;
                case EntityTypes.BOSS:
                    spawned = new Boss(this, id, entityDef, entityRandom.Next());
                    break;
                case EntityTypes.CART:
                    spawned = new Cart(this, id, entityDef, entityRandom.Next());
                    break;
                case EntityTypes.PICKUP:
                    spawned = new Pickup(this, id, entityDef, entityRandom.Next());
                    break;
                case EntityTypes.PROJECTILE:
                    spawned = new Projectile(this, id, entityDef, entityRandom.Next());
                    break;
                default:
                    spawned = new Effect(this, id, entityDef, effectRandom.Next());
                    break;
            }
            spawned.Pos = pos;
            entities.Add(spawned);
            OnEntitySpawn?.Invoke(spawned);
            spawned.Init(spawner);
            return spawned;
        }
        public Entity Spawn(NamespaceID entityRef, Vector3 pos, Entity spawner)
        {
            var entityDef = GetEntityDefinition(entityRef);
            if (entityDef == null)
                return null;
            return Spawn(entityDef, pos, spawner);
        }
        public Entity Spawn<T>(Vector3 pos, Entity spawner) where T: EntityDefinition
        {
            var entityDef = GetEntityDefinition<T>();
            if (entityDef == null)
                return null;
            return Spawn(entityDef, pos, spawner);
        }
        public void SpawnCarts(float x)
        {
            var cartRef = AreaDefinition.GetProperty<NamespaceID>(AreaProperties.CART_REFERENCE);

            var carts = GetEntities().OfType<Cart>();
            for (int i = 0; i < GetMaxLaneCount(); i++)
            {
                if (carts.Any(c => c.GetLane() == i && c.State != CartStates.TRIGGERED))
                    continue;
                Cart cart = Spawn(cartRef, new Vector3(x - i * 10, 0, GetEntityLaneZ(i)), null) as Cart;
            }
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
        public void Explode(Vector3 center, float radius, int faction, float amount, DamageEffectList effects, EntityReference source)
        {
            foreach (Entity entity in GetEntities())
            {
                if (entity.IsEnemy(faction) && Detection.IsInSphere(entity, center, radius))
                {
                    entity.TakeDamage(amount, effects, source);
                }
            }
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
    }
}