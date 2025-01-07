using System;
using System.Collections.Generic;
using System.Linq;
using PVZEngine.Level;
using UnityEngine;

namespace PVZEngine.Entities
{
    public class EntityCollider
    {
        public EntityCollider(Entity entity, string name, params Hitbox[] hitboxes)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException($"The name of an EntityCollider cannot be null or empty.");
            Entity = entity;
            Name = name;
            this.hitboxes.AddRange(hitboxes);
        }
        private EntityCollider()
        {
        }
        #region 碰撞箱
        public void Update()
        {
            foreach (var hitbox in hitboxes)
            {
                hitbox.Update();
            }
        }
        public void DoCollision(EntityCollider other, Vector3 selfMotion, Vector3 otherMotion, int checkPoints)
        {
            for (int i1 = 0; i1 < hitboxes.Count; i1++)
            {
                var hitbox1 = hitboxes[i1];
                for (int i2 = 0; i2 < other.hitboxes.Count; i2++)
                {
                    var hitbox2 = other.hitboxes[i2];
                    if (hitbox1.DoCollision(hitbox2, selfMotion, otherMotion, checkPoints, out var seperation))
                    {
                        var collision = collisionList.Find(c => c.OtherCollider == other);
                        bool enter = false;
                        if (collision == null)
                        {
                            enter = true;
                            collision = new EntityCollision(this, other);
                        }
                        collision.Seperation = seperation;
                        if (CallPreCollision(collision))
                        {
                            if (enter)
                            {
                                collisionList.Add(collision);
                                CallPostCollision(collision, EntityCollisionHelper.STATE_ENTER);
                            }
                            else
                            {
                                CallPostCollision(collision, EntityCollisionHelper.STATE_STAY);
                            }
                            collisionThisTick.Add(collision);
                        }
                        return;
                    }
                }
            }
        }
        public void AddHitbox(Hitbox hitbox)
        {
            hitboxes.Add(hitbox);
        }
        public void RemoveHitbox(Hitbox hitbox)
        {
            hitboxes.Remove(hitbox);
        }
        public Hitbox GetHitbox(int index)
        {
            return hitboxes[index];
        }
        public int GetHitboxCount()
        {
            return hitboxes.Count;
        }
        #endregion

        #region 碰撞
        public IEnumerable<EntityCollision> GetCollisions()
        {
            return collisionList.ToArray();
        }
        public void ExitCollision(LevelEngine level)
        {
            var notCollided = collisionList.Except(collisionThisTick).ToArray();
            foreach (var collision in notCollided)
            {
                if (collision != null)
                {
                    CallPostCollision(collision, EntityCollisionHelper.STATE_EXIT);
                }
                collisionList.Remove(collision);
            }
            collisionThisTick.Clear();
        }
        private bool CallPreCollision(EntityCollision collision)
        {
            return PreCollision?.Invoke(collision) ?? true;
        }
        private void CallPostCollision(EntityCollision collision, int state)
        {
            PostCollision?.Invoke(collision, state);
        }
        #endregion

        #region 序列化
        public EntityColliderReference ToReference()
        {
            return new EntityColliderReference(Entity.ID, Name);
        }
        public SerializableEntityCollider ToSerializable()
        {
            return new SerializableEntityCollider()
            {
                name = Name,
                enabled = Enabled,
                hitboxes = hitboxes.Select(h => h.ToSerializable()).ToArray(),
                collisionList = collisionList.Select(c => c.ToSerializable()).ToArray(),
            };
        }
        public static EntityCollider FromSerializable(SerializableEntityCollider seri, Entity entity)
        {
            var collider = new EntityCollider();
            collider.Entity = entity;
            collider.Name = seri.name;
            collider.Enabled = seri.enabled;
            collider.hitboxes = seri.hitboxes.Select(h => h.ToDeserialized(entity)).ToList();
            return collider;
        }
        public void LoadCollisions(LevelEngine level, SerializableEntityCollider seri)
        {
            collisionList = seri.collisionList.Select(s => EntityCollision.FromSerializable(s, level)).ToList();
        }
        #endregion

        public event Func<EntityCollision, bool> PreCollision;
        public event Action<EntityCollision, int> PostCollision;
        public bool Enabled { get; set; } = true;
        public string Name { get; set; }
        public Entity Entity { get; set; }
        private List<Hitbox> hitboxes = new List<Hitbox>();
        private List<EntityCollision> collisionThisTick = new List<EntityCollision>();
        private List<EntityCollision> collisionList = new List<EntityCollision>();
    }
    public class CollisionResult
    {
        public Vector3 seperation;
        public int selfHitbox;
        public int otherHitbox;
    }
}
