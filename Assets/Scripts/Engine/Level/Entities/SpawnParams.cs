using PVZEngine.Entities;

namespace PVZEngine.Level
{
    public class SpawnParams
    {
        public void SetProperty<T>(PropertyKey<T> key, T value)
        {
            properties.SetProperty(key, value);
        }
        public void Apply(Entity entity)
        {
            foreach (var property in properties.GetPropertyNames())
            {
                entity.SetPropertyObject(property, properties.GetPropertyObject(property));
            }
        }
        private PropertyDictionary properties = new PropertyDictionary();
    }
}
