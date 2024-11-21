using System;
using MongoDB.Bson.Serialization.Attributes;
using PVZEngine.Level;

namespace PVZEngine.Entities
{
    [Serializable]
    public class EntityID
    {
        public EntityID()
        {

        }
        public EntityID(long id)
        {
            this.id = id;
        }
        public EntityID(Entity entity) : this(entity?.ID ?? 0)
        {
        }
        public Entity GetEntity(LevelEngine game)
        {
            if (entityCache == null)
            {
                entityCache = game.FindEntityByID(ID);
            }
            return entityCache;
        }
        public override bool Equals(object obj)
        {
            if (obj is EntityID entityRef)
            {
                return ID == entityRef.ID;
            }
            return base.Equals(obj);
        }
        public override int GetHashCode()
        {
            return ID.GetHashCode();
        }
        public static bool operator ==(EntityID lhs, EntityID rhs)
        {
            if (lhs is null)
                return rhs is null;
            return lhs.Equals(rhs);
        }
        public static bool operator !=(EntityID lhs, EntityID rhs)
        {
            return !(lhs == rhs);
        }
        [BsonIgnore]
        public long ID => id;
        [BsonElement("id")]
        private long id;

        [BsonIgnore]
        private Entity entityCache;
    }
}