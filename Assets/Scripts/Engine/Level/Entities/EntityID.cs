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
            entityFound = true;
            entityCache = entity;
        }
        public Entity GetEntity(LevelEngine game)
        {
            if (!entityFound)
            {
                entityFound = true;
                entityCache = game.FindEntityByID(ID);
            }
            return entityCache;
        }
        public bool Exists(LevelEngine game)
        {
            var entity = GetEntity(game);
            return entity != null && entity.Exists();
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
        private bool entityFound;
        [BsonIgnore]
        private Entity entityCache;
    }
}