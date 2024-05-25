namespace PVZEngine
{
    public class ArmorDefinition : Definition
    {
        public ArmorDefinition(string nsp, string name) : base(nsp, name)
        {
        }
        public virtual void PostUpdate(Armor armor) { }
        public NamespaceID GetModelID()
        {
            return GetID().ToModelID(ModelID.TYPE_ARMOR);
        }
    }
}
