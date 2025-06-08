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
            Physics.Simulate(1);
            simulateBuffer.Clear();
            simulateBuffer.CopyFrom(entities.Values);
            for (int i = 0; i < simulateBuffer.Count; i++)
            {
                var entity = simulateBuffer[i];
                entity.Simulate();
                entity.RecycleColliders();
            }
            RecycleEntities();
        }
        public void InitEntity(Entity entity)
        {
            var col = CreateCollisionEntity(entity);
            col.CreateMainCollider(EntityCollisionHelper.NAME_MAIN);
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
        private void RecycleEntities()
        {
            foreach (var entity in recyclingEntities)
            {
                disabledEntities.Enqueue(entity);
                entity.ResetEntity();
            }
            recyclingEntities.Clear();
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
            entities.Add(entity.ID, ent);
            return ent;
        }
        private void DestroyCollisionEntity(Entity entity)
        {
            var collisionEnt = GetCollisionEntity(entity);
            if (collisionEnt && entities.Remove(entity.ID))
            {
                collisionEnt.gameObject.SetActive(false);
                recyclingEntities.Add(collisionEnt);
            }
        }
        private UnityCollisionEntity GetCollisionEntity(Entity entity)
        {
            if (entities.TryGetValue(entity.ID, out var ent))
                return ent;
            return null;
        }

        #region 碰撞体
        public IEntityCollider AddCollider(Entity entity, ColliderConstructor cons)
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
                if (EntityCollisionHelper.CanCollide(hostileMask, ent))
                {
                    if (ent.IsHostile(faction))
                    {
                        results.Add(collider);
                    }
                }
                else
                {
                    if (ent.IsFriendly(faction))
                    {
                        results.Add(collider);
                    }
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
                if (EntityCollisionHelper.CanCollide(hostileMask, ent))
                {
                    if (ent.IsHostile(faction))
                    {
                        results.Add(collider);
                    }
                }
                else
                {
                    if (ent.IsFriendly(faction))
                    {
                        results.Add(collider);
                    }
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
                if (EntityCollisionHelper.CanCollide(hostileMask, ent))
                {
                    if (ent.IsHostile(faction))
                    {
                        results.Add(collider);
                    }
                }
                else
                {
                    if (ent.IsFriendly(faction))
                    {
                        results.Add(collider);
                    }
                }
            }
        }
        #endregion

        public SerializableUnityCollisionSystem ToSerializable()
        {
            var seri = new SerializableUnityCollisionSystem();
            var seriEntities = new List<SerializableUnityCollisionEntity>();
            foreach (var pair in entities)
            {
                var ent = pair.Value;
                if (!ent || ent.Entity == null)
                    continue;
                var entity = ent.ToSerializable();
                seriEntities.Add(entity);
            }
            seri.entities = seriEntities.ToArray();
            return seri;
        }
        public void LoadFromSerializable(LevelEngine level, SerializableUnityCollisionSystem seri)
        {
            foreach (var seriEnt in seri.entities)
            {
                var ent = level.FindEntityByID(seriEnt.id);
                if (ent == null)
                    continue;
                var colEntity = CreateCollisionEntity(ent);
                colEntity.LoadFromSerializable(seriEnt, ent);
            }
            foreach (var pair in entities)
            {
                var seriEnt = seri.entities.FirstOrDefault(e => e.id == pair.Key);
                if (seriEnt == null)
                    continue;
                pair.Value.LoadCollisions(level, seriEnt);
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
        private List<UnityCollisionEntity> recyclingEntities = new List<UnityCollisionEntity>();
        private Queue<UnityCollisionEntity> disabledEntities = new Queue<UnityCollisionEntity>();
    }
    public class SerializableUnityCollisionSystem : ISerializableCollisionSystem
    {
        public SerializableUnityCollisionEntity[] entities;
    }
}
