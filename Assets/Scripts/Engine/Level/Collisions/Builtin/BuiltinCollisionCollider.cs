using System;
using System.Collections.Generic;
using System.Linq;
using PVZEngine.Level;
using PVZEngine.Level.Collisions;
using Tools.Mathematics;
using UnityEngine;

namespace PVZEngine.Entities
{
    public class BuiltinCollisionCollider : IEntityCollider, IQuadTreeNodeObject
    {
        public BuiltinCollisionCollider(Entity entity, string name, Hitbox hitbox)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException($"The name of an BuiltinCollisionCollider cannot be null or empty.");
            Entity = entity;
            Name = name;
            this.hitbox = hitbox;
            ReevaluateBounds();
        }
        private BuiltinCollisionCollider()
        {
        }
        public void SetEnabled(bool enabled)
        {
            Enabled = enabled;
            UpdateEnabled();
        }
        public void SetIgnored(bool ignored)
        {
            Ignored = ignored;
            UpdateEnabled();
        }
        private void UpdateEnabled()
        {
            var newValue = Enabled && !Ignored;

            if (newValue != lastEnabledState)
            {
                lastEnabledState = newValue;
                if (newValue)
                {
                    OnEnabled?.Invoke(this);
                }
                else
                {
                    OnDisabled?.Invoke(this);
                }
            }
        }
        public void ReevaluateBounds()
        {
            hitbox.ReevaluateBounds();
            bottomRect = hitbox.GetLocalBounds().GetBottomRect();
        }
        public bool GetCollisionTime(Vector3 prevPosition, BuiltinCollisionCollider target, float precision, out float collisionTime)
        {
            return GetCollisionTime(prevPosition, target.hitbox.GetBounds(), precision, out collisionTime);
        }
        public bool GetCollisionTime(Vector3 prevPosition, Bounds target, float precision, out float collisionTime)
        {
            var oldPosition = prevPosition + hitbox.GetLocalCenter();
            oldPosition = ((Vector3)Vector3Int.FloorToInt(oldPosition * precision)) / precision;

            var newPosition = hitbox.GetBoundsCenter();
            newPosition = ((Vector3)Vector3Int.FloorToInt(newPosition * precision)) / precision;

            var targetBounds = target;
            targetBounds.center = ((Vector3)Vector3Int.FloorToInt(targetBounds.center * precision)) / precision;
            return MathTool.CalculateAABBCollisionTime(oldPosition, newPosition, hitbox.GetBoundsSize(), targetBounds, out collisionTime);
        }

        #region 检测
        public bool CheckSphere(Vector3 center, float radius)
        {
            return hitbox.IsInSphere(center, radius);
        }
        #endregion

        #region 碰撞箱
        public void DoCollision(BuiltinCollisionCollider other, Vector3 offset)
        {
            var hitbox1 = hitbox;
            var hitbox2 = other.hitbox;
            if (hitbox1.DoCollision(hitbox2, offset, out var seperation))
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
                    collision.Checked = true;
                }
                return;
            }
        }
        public Bounds GetBoundingBox()
        {
            return hitbox.GetBounds();
        }
        public Vector3 GetCenter()
        {
            return hitbox.GetBoundsCenter();
        }
        public Vector3 GetPosition()
        {
            return hitbox.GetPosition();
        }
        public Rect GetCollisionRect(float rewind = 0)
        {
            var rect = bottomRect;
            var entityPos = Entity.Position;
            var entityMotion = entityPos - Entity.PreviousPosition;
            var offset = entityPos - entityMotion * rewind;
            rect.x += offset.x;
            rect.y += offset.z;
            return rect;
        }
        public Hitbox GetHitbox()
        {
            return hitbox;
        }
        #endregion

        #region 碰撞
        public void GetCollisions(List<EntityCollision> collisions)
        {
            collisions.AddRange(collisionList);
        }
        public void ExitCollision()
        {
            exitBuffer.Clear();
            exitBuffer.AddRange(collisionList);
            foreach (var collision in exitBuffer)
            {
                if (!collision.Checked)
                {
                    if (collision != null)
                    {
                        CallPostCollision(collision, EntityCollisionHelper.STATE_EXIT);
                    }
                    collisionList.Remove(collision);
                }
                collision.Checked = false;
            }
        }
        private bool CallPreCollision(EntityCollision collision)
        {
            return Entity.PreCollision(collision);
        }
        private void CallPostCollision(EntityCollision collision, int state)
        {
            Entity.PostCollision(collision, state);
        }
        #endregion

        #region 序列化
        public EntityColliderReference ToReference()
        {
            return new EntityColliderReference(Entity.ID, Name);
        }
        public SerializableEntityCollider ToSerializable()
        {
            var seri = new SerializableEntityCollider()
            {
                name = Name,
                enabled = Enabled,
                armorSlot = ArmorSlot,
                collisionList = collisionList.Select(c => c.ToSerializable()).ToArray(),
            };
            if (hitbox is CustomHitbox custom)
            {
                seri.updateMode = (int)ColliderUpdateMode.Custom;
                seri.customSize = custom.GetSize();
                seri.customOffset = custom.GetOffset();
                seri.customPivot = custom.GetPivot();
            }
            else
            {
                seri.updateMode = (int)ColliderUpdateMode.Main;
            }
            return seri;
        }
        public static BuiltinCollisionCollider FromSerializable(ISerializableCollisionCollider seri, Entity entity)
        {
            var collider = new BuiltinCollisionCollider();
            collider.Entity = entity;
            collider.Name = seri.Name;
            collider.SetEnabled(seri.Enabled);
            collider.ArmorSlot = seri.ArmorSlot;

            var updateMode = (ColliderUpdateMode)seri.UpdateMode;
            if (updateMode == ColliderUpdateMode.Custom)
            {
                var hitbox = new CustomHitbox(entity);
                hitbox.SetSize(seri.CustomSize);
                hitbox.SetOffset(seri.CustomOffset);
                hitbox.SetPivot(seri.CustomPivot);
                collider.hitbox = hitbox;
            }
            else if (updateMode == ColliderUpdateMode.Main)
            {
                collider.hitbox = new EntityHitbox(entity);
            }
            collider.ReevaluateBounds();
            return collider;
        }
        public void LoadCollisions(LevelEngine level, ISerializableCollisionCollider seri)
        {
            collisionList = seri.Collisions.Select(s => EntityCollision.FromSerializable(s, level)).ToList();
        }
        #endregion

        bool IEquatable<IQuadTreeNodeObject>.Equals(IQuadTreeNodeObject other)
        {
            if (other is not BuiltinCollisionCollider collider)
                return false;
            return this == collider;
        }

        public override string ToString()
        {
            return $"{Entity}[{Name}]";
        }

        public event Action<BuiltinCollisionCollider> OnEnabled;
        public event Action<BuiltinCollisionCollider> OnDisabled;
        public bool Enabled { get; private set; } = true;
        public bool Ignored { get; private set; } = false;
        public string Name { get; set; }
        public Entity Entity { get; set; }
        public NamespaceID ArmorSlot { get; set; }
        private bool lastEnabledState = true;
        private Rect bottomRect;
        private Hitbox hitbox;
        private List<EntityCollision> collisionList = new List<EntityCollision>();
        private List<EntityCollision> exitBuffer = new List<EntityCollision>();
    }
}
