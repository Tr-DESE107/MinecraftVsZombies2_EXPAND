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
        }
        public EntityDefinition GetEntityDefinition(NamespaceID entityRef)
        {
            return entityDefinitions.FirstOrDefault(d => d.GetReference() == entityRef);
        }
        public T GetEntityDefinition<T>() where T : EntityDefinition
        {
            return entityDefinitions.OfType<T>().FirstOrDefault();
        }
        public ShellDefinition GetShellDefinition(NamespaceID entityRef)
        {
            return shellDefinitions.FirstOrDefault(d => d.GetReference() == entityRef);
        }

        public Entity Spawn(EntityDefinition entityDef, Vector3 pos, Entity spawner)
        {
            Entity spawned;
            int id = AllocEntityID();
            switch (entityDef.Type)
            {
                case EntityTypes.CONTRAPTION:
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
            spawned.Pos = pos;
            entities.Add(spawned);
            spawned.Init(spawner);
            return spawned;
        }
        public Entity Spawn(NamespaceID entityRef, Vector3 pos, Entity spawner)
        {
            var entityDef = GetEntityDefinition(entityRef);
            return Spawn(entityDef, pos, spawner);
        }

        public void CreatePreviewEnemies(IList<NamespaceID> validEnemies)
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
                            pos = new Vector3(miscRandom.Next(MIN_PREVIEW_X, MAX_PREVIEW_X), 0, miscRandom.Next(MIN_PREVIEW_Y, MAX_PREVIEW_Y));

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
        public float GetUnitLaneZ(int row)
        {
            return GetRowZ(row, 0.16f);
        }
        public float GetColumnX(int column)
        {
            return LEFT_BORDER + column * GRID_SIZE;
        }

        public float GetRowZ(int row, float zOffset)
        {
            float centerZ = (SCREEN_HEIGHT - GetTopZOffset()) * 0.5f;
            float extent = GetMaxLaneCount() * GRID_SIZE * 0.5f;
            return centerZ + extent - row * GRID_SIZE + zOffset;
        }
        public void SpawnCarts()
        {
            var cartRef = AreaDefinition.GetProperty<NamespaceID>(AreaProperties.CART_REFERENCE);

            var carts = GetEntities().OfType<Cart>();
            for (int i = 0; i < GetMaxLaneCount(); i++)
            {
                if (carts.Any(c => c.GetLane() == i && c.State != CartStates.TRIGGERED))
                    continue;
                Cart cart = Spawn(cartRef, new Vector3(CART_START_X - i * 10, 0, GetUnitLaneZ(i)), null) as Cart;
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

        public float GetBorderX(bool right)
        {
            return right ? RIGHT_BORDER : LEFT_BORDER;
        }
        public float GetAttackBorderX(bool right)
        {
            return right ? ATTACK_RIGHT_BORDER : ATTACK_LEFT_BORDER;
        }
        public float GetPickupBorderX(bool right)
        {
            return right ? PICKUP_RIGHT_BORDER : PICKUP_LEFT_BORDER;
        }
        public float GetScreenHeight()
        {
            return SCREEN_HEIGHT;
        }
        public float GetEnemyRightBorderX()
        {
            return ENEMY_RIGHT_BORDER;
        }
        private int AllocEntityID()
        {
            int id = currentEntityID;
            currentEntityID++;
            return id;
        }
        private int currentEntityID = 1;
        private const float CART_START_X = 150;
        private const float TOP_Z_OFFSET = 80;
        private const float SCREEN_WIDTH = 800;
        private const float SCREEN_HEIGHT = 600;
        private const float LEFT_BORDER = 220;
        private const float RIGHT_BORDER = LEFT_BORDER + SCREEN_WIDTH;
        private const float MIN_PREVIEW_X = 1080;
        private const float MAX_PREVIEW_X = 1300;
        private const float MIN_PREVIEW_Y = 50;
        private const float MAX_PREVIEW_Y = 450;
        private const float GRID_SIZE = 80;
        private const float LAWN_HEIGHT = 600;
        private const float LEVEL_WIDTH = 1400;
        private const float PICKUP_LEFT_BORDER = LEFT_BORDER + 50;
        private const float PICKUP_RIGHT_BORDER = RIGHT_BORDER - 50;
        private const float ATTACK_LEFT_BORDER = LEFT_BORDER;
        private const float ATTACK_RIGHT_BORDER = RIGHT_BORDER;
        private const float ENEMY_RIGHT_BORDER = RIGHT_BORDER + 60;
        private List<EntityDefinition> entityDefinitions = new List<EntityDefinition>();
        private List<ShellDefinition> shellDefinitions = new List<ShellDefinition>();
    }
}