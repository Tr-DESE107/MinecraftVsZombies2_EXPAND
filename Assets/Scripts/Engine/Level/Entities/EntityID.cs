using System;
using MongoDB.Bson.Serialization.Attributes;

namespace PVZEngine.Level
{
    [Serializable]
    public class EntityID
    {
        public EntityID()
        {

        }
        public EntityID(int id)
        {
            this.id = id;
        }
        public EntityID(Entity entity) : this(entity?.ID ?? 0)
        {
        }
        public Entity GetEntity(LevelEngine game)
        {
            return game.FindEntityByID(ID);
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
            return lhs.Equals(rhs);
        }
        public static bool operator !=(EntityID lhs, EntityID rhs)
        {
            return !(lhs == rhs);
        }
        [BsonIgnore]
        public int ID => id;
        [BsonElement("id")]
        private int id;
    }
}