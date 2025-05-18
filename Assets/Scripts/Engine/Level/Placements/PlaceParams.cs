namespace PVZEngine.Placements
{
    public class PlaceParams
    {
        public void SetProperty(PropertyKey key, object value)
        {
            properties.SetProperty(key, value);
        }
        public T GetProperty<T>(PropertyKey key)
        {
            return properties.GetProperty<T>(key);
        }
        private PropertyDictionary properties = new PropertyDictionary();
    }
}
