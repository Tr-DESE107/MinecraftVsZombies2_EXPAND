namespace PVZEngine
{
    public abstract class Definition
    {
        public Definition(string nsp, string name)
        {
            Namespace = nsp;
            Name = name;
        }
        public T GetProperty<T>(string name)
        {
            return propertyDict.GetProperty<T>(name);
        }
        public NamespaceID GetID()
        {
            return new NamespaceID(Namespace, Name);
        }
        protected void SetProperty(string name, object value)
        {
            propertyDict.SetProperty(name, value);
        }
        public string Namespace { get; set; }
        public string Name { get; set; }
        protected PropertyDictionary propertyDict = new PropertyDictionary();
    }
}
