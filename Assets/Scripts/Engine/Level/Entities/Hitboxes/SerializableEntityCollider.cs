namespace PVZEngine.Entities
{
    public class SerializableEntityCollider
    {
        public string name;
        public bool enabled;
        public NamespaceID armorSlot;
        public SerializableHitbox[] hitboxes;
        public SerializableEntityCollision[] collisionList;
    }
}
