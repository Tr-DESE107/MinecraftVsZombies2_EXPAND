namespace PVZEngine
{
    public class Buff
    {
        public Buff(BuffDefinition definition)
        {
            Definition = definition;
        }
        public T GetProperty<T>(string name)
        {
            return propertyDict.GetProperty<T>(name);
        }
        public void SetProperty(string name, object value)
        {
            propertyDict.SetProperty(name, value);
        }
        public PropertyModifier[] GetModifiers()
        {
            return Definition.GetModifiers();
        }
        public void AddToEntity(Entity entity)
        {
            if (Entity != null)
                return;
            Entity = entity;
            foreach (var modifier in GetModifiers())
            {
                modifier.PostAdd(entity, this);
            }
        }
        public void RemoveFromEntity()
        {
            if (Entity == null)
                return;
            foreach (var modifier in GetModifiers())
            {
                modifier.PostRemove(Entity, this);
            }
            Entity = null;
        }
        public BuffDefinition Definition { get; }
        public Entity Entity { get; private set; }
        private PropertyDictionary propertyDict = new PropertyDictionary();
    }
}
