using System.Collections.Generic;
using System.Linq;
using MVZ2.Collision;
using PVZEngine.Base;
using PVZEngine.Entities;
using PVZEngine.Level;
using PVZEngine.Level.Collisions;
using UnityEngine;

namespace MVZ2.Collisions
{
    public class UnityCollisionSystem : MonoBehaviour, ICollisionSystem
    {
        void ICollisionSystem.Update()
        {
            ClearEntityTrash();
            Physics.Simulate(1);
            simulateBuffer.Clear();
            simulateBuffer.CopyFrom(entities.Values);
            for (int i = 0; i < simulateBuffer.Count; i++)
            {
                var entity = simulateBuffer[i];
                entity.Simulate();
                entity.RecycleColliders();
            }
        }
        public void InitEntity(Entity entity)
        {
            var col = CreateCollisionEntity(entity);
            col.CreateMainCollider(EntityCollisionHelper.NAME_MAIN);
            entities.Add(entity.ID, col);
        }
        public void UpdateEntityDetection(Entity entity)
        {
            var ent = GetCollisionEntity(entity);
            ent?.UpdateEntityDetection();
        }
        public void UpdateEntityPosition(Entity entity)
        {
            var ent = GetCollisionEntity(entity);
            ent?.UpdateEntityPosition();
        }
        public void UpdateEntitySize(Entity entity)
        {
            var ent = GetCollisionEntity(entity);
            ent?.UpdateEntitySize();
        }
        public void DestroyEntity(Entity entity)
        {
            DestroyCollisionEntity(entity);
        }
        public void GetCurrentCollisions(Entity entity, List<EntityCollision> collisions)
        {
            var collisionEnt = GetCollisionEntity(entity);
            if (!collisionEnt)
                return;
            collisionEnt.GetCollisions(collisions);
        }
        private UnityCollisionEntity CreateCollisionEntity(Entity entity)
        {
            UnityCollisionEntity ent;
            if (disabledEntities.Count > 0)
            {
                ent = disabledEntities.Dequeue();
            }
            else
            {
                ent = Instantiate(collisionEntityTemplate, entityRoot).GetComponent<UnityCollisionEntity>();
            }
            ent.gameObject.SetActive(true);
            ent.Init(entity);
            return ent;
        }
        private void DestroyCollisionEntity(Entity entity)
        {
            var collisionEnt = GetCollisionEntity(entity);
            if (collisionEnt && entities.Remove(entity.ID))
            {
                collisionEnt.gameObject.SetActive(false);
                entityTrash.Add(entity.ID, collisionEnt);
            }
        }
        private UnityCollisionEntity GetCollisionEntity(Entity entity)
        {
            if (entities.TryGetValue(entity.ID, out var ent))
                return ent;
            return GetCollisionEntityInTrash(entity.ID);
        }
        private UnityCollisionEntity GetCollisionEntityInTrash(long id)
        {
            if (entityTrash.TryGetValue(id, out var entity))
                return entity;
            return null;
        }
        private void ClearEntityTrash()
        {
            foreach (var pair in entityTrash)
            {
                var ent = pair.Value;
                if (!ent)
                    continue;
                disabledEntities.Enqueue(ent);
                ent.ResetEntity();
            }
            entityTrash.Clear();
        }

        #region 碰撞体
        public IEntityCollider CreateCustomCollider(Entity entity, ColliderConstructor cons)
        {
            var collisionEnt = GetCollisionEntity(entity);
            if (!collisionEnt)
                return null;
            return collisionEnt.CreateCustomCollider(cons);
        }
        public bool RemoveCollider(Entity entity, string name)
        {
            var collisionEnt = GetCollisionEntity(entity);
            if (!collisionEnt)
                return false;
            return collisionEnt.DestroyCollider(name);
        }
        public UnityEntityCollider GetCollider(Entity entity, string name)
        {
            var collisionEnt = GetCollisionEntity(entity);
            if (!collisionEnt)
                return null;
            return collisionEnt.GetCollider(name);
        }
        IEntityCollider ICollisionSystem.GetCollider(Entity entity, string name) => GetCollider(entity, name);
        #endregion

        #region 检测
        public IEntityCollider[] OverlapBox(Vector3 center, Vector3 size, int faction, int hostileMask, int friendlyMask)
        {
            var results = new List<IEntityCollider>();
            OverlapBoxNonAlloc(center, size, faction, hostileMask, friendlyMask, results);
            return results.ToArray();
        }
        public void OverlapBoxNonAlloc(Vector3 center, Vector3 size, int faction, int hostileMask, int friendlyMask, List<IEntityCollider> results)
        {
            var combinedMask = hostileMask | friendlyMask;
            var layerMask = UnityCollisionHelper.ToObjectLayerMask(combinedMask);

            var colliderCount = Physics.OverlapBoxNonAlloc(center, size * 0.5f, overlapBuffer, Quaternion.identity, layerMask, QueryTriggerInteraction.Collide);
            for (int i = 0; i < colliderCount; i++)
            {
                var col = overlapBuffer[i];
                var collider = col.GetComponent<UnityEntityCollider>();
                if (results.Contains(collider))
                    continue;
                var ent = collider.Entity;
                if (EntityCollisionHelper.CanCollide(hostileMask, ent) && ent.IsHostile(faction))
                {
                    results.Add(collider);
                }
                else if (EntityCollisionHelper.CanCollide(friendlyMask, ent) && ent.IsFriendly(faction))
                {
                    results.Add(collider);
                }
            }
        }
        public IEntityCollider[] OverlapSphere(Vector3 center, float radius, int faction, int hostileMask, int friendlyMask)
        {
            var results = new List<IEntityCollider>();
            OverlapSphereNonAlloc(center, radius, faction, hostileMask, friendlyMask, results);
            return results.ToArray();
        }
        public void OverlapSphereNonAlloc(Vector3 center, float radius, int faction, int hostileMask, int friendlyMask, List<IEntityCollider> results)
        {
            var combinedMask = hostileMask | friendlyMask;
            var layerMask = UnityCollisionHelper.ToObjectLayerMask(combinedMask);

            var colliderCount = Physics.OverlapSphereNonAlloc(center, radius, overlapBuffer, layerMask, QueryTriggerInteraction.Collide);
            for (int i = 0; i < colliderCount; i++)
            {
                var col = overlapBuffer[i];
                var collider = col.GetComponent<UnityEntityCollider>();
                if (results.Contains(collider))
                    continue;
                var ent = collider.Entity;
                if (EntityCollisionHelper.CanCollide(hostileMask, ent) && ent.IsHostile(faction))
                {
                    results.Add(collider);
                }
                else if (EntityCollisionHelper.CanCollide(friendlyMask, ent) && ent.IsFriendly(faction))
                {
                    results.Add(collider);
                }
            }
        }
        public IEntityCollider[] OverlapCapsule(Vector3 point0, Vector3 point1, float radius, int faction, int hostileMask, int friendlyMask)
        {
            var results = new List<IEntityCollider>();
            OverlapCapsuleNonAlloc(point0, point1, radius, faction, hostileMask, friendlyMask, results);
            return results.ToArray();
        }
        public void OverlapCapsuleNonAlloc(Vector3 point0, Vector3 point1, float radius, int faction, int hostileMask, int friendlyMask, List<IEntityCollider> results)
        {
            var combinedMask = hostileMask | friendlyMask;
            var layerMask = UnityCollisionHelper.ToObjectLayerMask(combinedMask);

            var colliderCount = Physics.OverlapCapsuleNonAlloc(point0, point1, radius, overlapBuffer, layerMask, QueryTriggerInteraction.Collide);
            for (int i = 0; i < colliderCount; i++)
            {
                var col = overlapBuffer[i];
                var collider = col.GetComponent<UnityEntityCollider>();
                if (results.Contains(collider))
                    continue;
                var ent = collider.Entity;
                if (EntityCollisionHelper.CanCollide(hostileMask, ent) && ent.IsHostile(faction))
                {
                    results.Add(collider);
                }
                else if (EntityCollisionHelper.CanCollide(friendlyMask, ent) && ent.IsFriendly(faction))
                {
                    results.Add(collider);
                }
            }
        }
        #endregion

        public SerializableUnityCollisionSystem ToSerializable()
        {
            var seri = new SerializableUnityCollisionSystem();
            seri.entities = entities.Values.Where(e => e && e.Entity != null).Select(e => e.ToSerializable()).ToArray();
            seri.entityTrash = entityTrash.Values.Where(e => e && e.Entity != null).Select(e => e.ToSerializable()).ToArray();
            return seri;
        }
        public void LoadFromSerializable(LevelEngine level, ISerializableCollisionSystem seri)
        {
            // Load Entities.
            if (seri.Entities != null)
            {
                foreach (var seriEnt in seri.Entities)
                {
                    var ent = level.FindEntityByID(seriEnt.ID);
                    var colEntity = CreateCollisionEntity(ent);
                    colEntity.LoadFromSerializable(seriEnt, ent);
                    entities.Add(ent.ID, colEntity);
                }
            }

            if (seri.EntityTrash != null)
            {
                foreach (var seriEnt in seri.EntityTrash)
                {
                    var ent = level.FindEntityByID(seriEnt.ID);
                    var colEntity = CreateCollisionEntity(ent);
                    colEntity.LoadFromSerializable(seriEnt, ent);
                    colEntity.gameObject.SetActive(false);
                    entityTrash.Add(ent.ID, colEntity);
                }
            }

            // Load Collisions.
            if (seri.Entities != null)
            {
                for (int i = 0; i < seri.Entities.Length; i++)
                {
                    var ent = entities[i];
                    ISerializableCollisionEntity seriEnt = seri.Entities[i];
                    if (seriEnt == null)
                        continue;
                    ent.LoadCollisions(level, seriEnt);
                }
            }
            if (seri.EntityTrash != null)
            {
                for (int i = 0; i < seri.EntityTrash.Length; i++)
                {
                    var ent = entityTrash[i];
                    ISerializableCollisionEntity seriEnt = seri.EntityTrash[i];
                    if (seriEnt == null)
                        continue;
                    ent.LoadCollisions(level, seriEnt);
                }
            }
        }
        ISerializableCollisionSystem ICollisionSystem.ToSerializable()
        {
            return ToSerializable();
        }
        void ICollisionSystem.LoadFromSerializable(LevelEngine level, ISerializableCollisionSystem seri)
        {
            if (seri is not SerializableUnityCollisionSystem sys)
                return;
            LoadFromSerializable(level, sys);
        }
        [SerializeField]
        private GameObject collisionEntityTemplate;
        [SerializeField]
        private Transform entityRoot;
        private Collider[] overlapBuffer = new Collider[2048];
        private ArrayBuffer<UnityCollisionEntity> simulateBuffer = new ArrayBuffer<UnityCollisionEntity>(2048);
        private Dictionary<long, UnityCollisionEntity> entities = new Dictionary<long, UnityCollisionEntity>();
        private Dictionary<long, UnityCollisionEntity> entityTrash = new Dictionary<long, UnityCollisionEntity>();
        private Queue<UnityCollisionEntity> disabledEntities = new Queue<UnityCollisionEntity>();
    }
    public class SerializableUnityCollisionSystem : ISerializableCollisionSystem
    {
        public SerializableUnityCollisionEntity[] entities;
        public SerializableUnityCollisionEntity[] entityTrash;

        ISerializableCollisionEntity[] ISerializableCollisionSystem.Entities => entities;

        ISerializableCollisionEntity[] ISerializableCollisionSystem.EntityTrash => entityTrash;
    }
}
