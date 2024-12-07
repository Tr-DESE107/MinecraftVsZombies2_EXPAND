namespace PVZEngine.Entities
{
    public class SerializableEntityHitbox : SerializableHitbox
    {
        public SerializableEntityHitbox(EntityHitbox hitbox) : base(hitbox)
        {
        }
        public override Hitbox ToDeserialized(Entity entity)
        {
            var hitbox = new EntityHitbox(entity);
            DeserializeProperties(hitbox);
            return hitbox;
        }
    }
}
