using System;
using System.Collections.Generic;
using System.Linq;
using PVZEngine.Level;

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
        public bool Intersects(EntityCollider other)
        {
            foreach (var hitbox1 in hitboxes)
            {
                foreach (var hitbox2 in other.hitboxes)
                {
                    if (hitbox1.Intersects(hitbox2))
                        return true;
                }
            }
            return false;
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
        public void Collide(EntityCollision collision)
        {
            if (!collisionList.Contains(collision))
            {
                CallPostCollision(collision, EntityCollisionHelper.STATE_ENTER);
                collisionList.Add(collision);
            }
            else
            {
                CallPostCollision(collision, EntityCollisionHelper.STATE_STAY);
            }
            collisionThisTick.Add(collision);
        }
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
                collisionList = collisionList.Select(c => c.OtherCollider.ToReference()).ToArray(),
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
            collisionList = seri.collisionList.Select(s => new EntityCollision(this, s.GetCollider(level))).ToList();
        }
        #endregion

        public event Action<EntityCollision, int> PostCollision;
        public bool Enabled { get; set; } = true;
        public string Name { get; set; }
        public Entity Entity { get; set; }
        private List<Hitbox> hitboxes = new List<Hitbox>();
        private List<EntityCollision> collisionThisTick = new List<EntityCollision>();
        private List<EntityCollision> collisionList = new List<EntityCollision>();
    }
    [Serializable]
    public class EntityColliderReference
    {
        public long entityId;
        public string unitName;

        public EntityColliderReference(EntityCollider collider) : this(collider.Entity.ID, collider.Name)
        {
        }
        public EntityColliderReference(long entityId, string unitName)
        {
            this.entityId = entityId;
            this.unitName = unitName;
        }

        public EntityCollider GetCollider(LevelEngine engine)
        {
            var entity = engine.FindEntityByID(entityId);
            if (entity == null)
                return null;
            return entity.GetCollider(unitName);
        }
        public override bool Equals(object obj)
        {
            if (obj is not EntityColliderReference other)
                return false;
            return entityId == other.entityId && unitName == other.unitName;
        }
        public override int GetHashCode()
        {
            return entityId.GetHashCode() * 31 + unitName.GetHashCode();
        }
        public static bool operator ==(EntityColliderReference lhs, EntityColliderReference rhs)
        {
            return lhs.Equals(rhs);
        }
        public static bool operator !=(EntityColliderReference lhs, EntityColliderReference rhs)
        {
            return !(lhs == rhs);
        }
    }
}
