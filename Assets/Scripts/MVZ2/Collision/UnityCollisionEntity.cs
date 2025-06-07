﻿using System.Collections.Generic;
using System.Linq;
using MVZ2.Collision;
using PVZEngine.Entities;
using PVZEngine.Level;
using PVZEngine.Level.Collisions;
using UnityEngine;

namespace MVZ2.Collisions
{
    public class UnityCollisionEntity : MonoBehaviour
    {
        public void Init(Entity entity)
        {
            Entity = entity;
            gameObject.name = Entity.ToString();
            gameObject.layer = UnityCollisionHelper.ToObjectLayer(Entity.Type);
            UpdateEntity();
        }
        public void ResetEntity()
        {
            foreach (var collider in colliders)
            {
                if (collider)
                {
                    collider.gameObject.SetActive(false);
                    RecycleCollider(collider);
                }
            }
            colliders.Clear();
        }
        public void RecycleColliders()
        {
            foreach (var collider in recyclingColliders)
            {
                RecycleCollider(collider);
            }
            recyclingColliders.Clear();
        }
        public void Simulate()
        {
            foreach (var collider in colliders)
            {
                collider.Simulate();
            }
        }
        public void UpdateEntity()
        {
            UpdateEntityDetection();
            UpdateEntityPosition();
            UpdateEntitySize();
        }
        public void UpdateEntityDetection()
        {
            var detection = Entity.GetCollisionDetection();
            bool active = detection != EntityCollisionHelper.DETECTION_IGNORE;
            if (gameObject.activeSelf != active)
            {
                gameObject.SetActive(active);
            }
        }
        public void UpdateEntityPosition()
        {
            var pos = Entity.Position;
            rigid.position = pos;
            transform.position = pos;
        }
        public void UpdateEntitySize()
        {
            foreach (var collider in colliders)
            {
                collider.UpdateEntitySize();
            }
        }
        private UnityEntityCollider CreateCollider(string name)
        {
            UnityEntityCollider collider;
            if (disabledColliders.Count > 0)
            {
                collider = disabledColliders.Dequeue();
            }
            else
            {
                collider = Instantiate(colliderTemplate, colliderRoot).GetComponent<UnityEntityCollider>();
            }
            collider.gameObject.SetActive(true);
            collider.gameObject.layer = gameObject.layer;
            collider.Init(Entity, name);
            collider.UpdateEntitySize();
            colliders.Add(collider);
            return collider;
        }
        public UnityEntityCollider CreateMainCollider(string name)
        {
            var collider = CreateCollider(name);
            collider.SetMain();
            return collider;
        }
        public UnityEntityCollider CreateCustomCollider(ColliderConstructor cons)
        {
            var collider = CreateCollider(cons.name);
            collider.SetCustom(cons);
            return collider;
        }
        public bool DestroyCollider(string name)
        {
            var collider = colliders.FirstOrDefault(c => c.Name == name);
            if (collider && colliders.Remove(collider))
            {
                collider.gameObject.SetActive(false);
                recyclingColliders.Add(collider);
                return true;
            }
            return false;
        }
        public void GetCollisions(List<EntityCollision> collisions)
        {
            foreach (var collider in colliders)
            {
                collider.GetCollisions(collisions);
            }
        }
        public UnityEntityCollider GetCollider(string name)
        {
            return colliders.FirstOrDefault(c => c.Name == name);
        }
        private void RecycleCollider(UnityEntityCollider collider)
        {
            disabledColliders.Enqueue(collider);
            collider.ResetCollider();
        }

        #region 序列化
        public SerializableUnityCollisionEntity ToSerializable()
        {
            return new SerializableUnityCollisionEntity()
            {
                id = Entity.ID,
                colliders = colliders.Select(c => c.ToSerializable()).ToArray()
            };
        }
        public void LoadFromSerializable(SerializableUnityCollisionEntity seri, Entity entity)
        {
            Entity = entity;
            foreach (var extraCollider in seri.colliders)
            {
                var collider = CreateCollider(extraCollider.name);
                collider.LoadFromSerializable(extraCollider, entity);
            }
            UpdateEntity();
        }
        public void LoadCollisions(LevelEngine level, SerializableUnityCollisionEntity seri)
        {
            for (int i = 0; i < colliders.Count; i++)
            {
                var colliderSeri = seri.colliders[i];
                var collider = colliders[i];
                collider.LoadCollisions(level, colliderSeri);
            }
        }
        #endregion
        public Entity Entity { get; private set; }
        [SerializeField]
        private Rigidbody rigid;
        [SerializeField]
        private List<UnityEntityCollider> colliders = new List<UnityEntityCollider>();
        [SerializeField]
        private List<UnityEntityCollider> recyclingColliders = new List<UnityEntityCollider>();
        [SerializeField]
        private Queue<UnityEntityCollider> disabledColliders = new Queue<UnityEntityCollider>();
        [SerializeField]
        private GameObject colliderTemplate;
        [SerializeField]
        private Transform colliderRoot;
    }
    public class SerializableUnityCollisionEntity
    {
        public long id;
        public SerializableUnityEntityCollider[] colliders;
    }
}
