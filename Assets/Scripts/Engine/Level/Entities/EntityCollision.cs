namespace PVZEngine.Entities
{
    public class EntityCollision
    {
        public EntityCollision(EntityCollider collider, EntityCollider otherCollider)
        {
            Collider = collider;
            OtherCollider = otherCollider;
        }
        public override bool Equals(object obj)
        {
            if (obj is not EntityCollision other)
                return false;
            return Collider == other.Collider && OtherCollider == other.OtherCollider;
        }
        public override int GetHashCode()
        {
            return Collider.GetHashCode() * 31 + OtherCollider.GetHashCode();
        }
        public static bool operator ==(EntityCollision lhs, EntityCollision rhs)
        {
            return lhs.Equals(rhs);
        }
        public static bool operator !=(EntityCollision lhs, EntityCollision rhs)
        {
            return !(lhs == rhs);
        }

        public EntityCollider Collider { get; set; }
        public EntityCollider OtherCollider { get; set; }
        public Entity Entity => Collider.Entity;
        public Entity Other => OtherCollider.Entity;
    }
}
