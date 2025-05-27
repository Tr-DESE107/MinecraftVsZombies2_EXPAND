namespace PVZEngine.Placements
{
    public class PlaceParams
    {
        public void SetProperty<T>(PropertyKey<T> key, T value)
        {
            properties.SetProperty(key, value);
        }
        public T GetProperty<T>(PropertyKey<T> key)
        {
            return properties.GetProperty<T>(key);
        }
        private PropertyDictionary properties = new PropertyDictionary();
    }
}
