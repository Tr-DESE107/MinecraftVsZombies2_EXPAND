using PVZEngine.Entities;

namespace PVZEngine.Level
{
    public class SpawnParams
    {
        public void SetProperty(PropertyKey key, object value)
        {
            properties.SetProperty(key, value);
        }
        public void Apply(Entity entity)
        {
            foreach (var property in properties.GetPropertyNames())
            {
                entity.SetProperty(property, properties.GetProperty(property));
            }
        }
        private PropertyDictionary properties = new PropertyDictionary();
    }
}
