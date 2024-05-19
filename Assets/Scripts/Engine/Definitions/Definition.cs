namespace PVZEngine
{
    public abstract class Definition
    {
        public T GetProperty<T>(string name)
        {
            return propertyDict.GetProperty<T>(name);
        }
        public NamespaceID GetReference()
        {
            return new NamespaceID(Namespace, Name);
        }
        public string Namespace { get; set; }
        public string Name { get; set; }
        private PropertyDictionary propertyDict = new PropertyDictionary();
    }
}
