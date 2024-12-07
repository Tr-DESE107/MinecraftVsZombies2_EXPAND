namespace PVZEngine.Entities
{
    public abstract class SerializableHitbox
    {
        public SerializableHitbox(Hitbox hitbox)
        {
        }
        public abstract Hitbox ToDeserialized(Entity entity);
        protected void DeserializeProperties(Hitbox hitbox)
        {
        }
        public bool enabled;
        public NamespaceID[] tags;
    }
}
