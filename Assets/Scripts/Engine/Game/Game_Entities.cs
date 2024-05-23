using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PVZEngine
{
    public partial class Game
    {
        private List<Entity> entities = new List<Entity>();

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
                    spawned = new Contraption(this, id, entityRandom.Next());
                    break;
                case EntityTypes.ENEMY:
                    spawned = new Enemy(this, id, entityRandom.Next());
                    break;
                case EntityTypes.OBSTACLE:
                    spawned = new Obstacle(this, id, entityRandom.Next());
                    break;
                case EntityTypes.BOSS:
                    spawned = new Boss(this, id, entityRandom.Next());
                    break;
                case EntityTypes.CART:
                    spawned = new Cart(this, id, entityRandom.Next());
                    break;
                case EntityTypes.PICKUP:
                    spawned = new Pickup(this, id, entityRandom.Next());
                    break;
                case EntityTypes.PROJECTILE:
                    spawned = new Projectile(this, id, entityRandom.Next());
                    break;
                default:
                    spawned = new Effect(this, id, effectRandom.Next());
                    break;
            }
            spawned.Definition = entityDef;
            spawned.Pos = pos;
            entities.Add(spawned);
            spawned.Init(spawner);
            OnEntitySpawn?.Invoke(spawned);
            return spawned;
        }
        public Entity Spawn(NamespaceID entityRef, Vector3 pos, Entity spawner)
        {
            var entityDef = GetEntityDefinition(entityRef);
            if (entityDef == null)
                return null;
            return Spawn(entityDef, pos, spawner);
        }

        public void CreatePreviewEnemies(IList<NamespaceID> validEnemies, Rect region)
        {
            List<Enemy> createdEnemies = new List<Enemy>();

            int loopTimes = 0;

            while (true)
            {
                for (int i = 0; i < validEnemies.Count; i++)
                {
                    var entityRef = validEnemies[i];

                    int times = 1;
                    for (int time = 0; time < times; time++)
                    {
                        bool around;
                        Vector3 pos;
                        do
                        {
                            pos = new Vector3(miscRandom.Next(region.xMin, region.xMax), 0, miscRandom.Next(region.yMin, region.yMax));

                            around = false;
                            for (int e = 0; e < createdEnemies.Count; e++)
                            {
                                Vector3 createdPos = createdEnemies[e].Pos;
                                if (Vector3.Distance(createdPos, pos) < 80)
                                {
                                    around = true;
                                    break;
                                }
                            }
                        }
                        while (around);

                        Enemy enm = Spawn(entityRef, pos, null) as Enemy;
                        enm.SetPreviewMode();
                        createdEnemies.Add(enm);

                        if (createdEnemies.Count >= Mathf.Max(6, validEnemies.Count + 3))
                        {
                            return;
                        }
                    }
                }

                loopTimes++;
                if (loopTimes > 1024)
                {
                    Debug.Log("次数超过上限，跳出循环。");
                    return;
                }
            }
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
    }
}