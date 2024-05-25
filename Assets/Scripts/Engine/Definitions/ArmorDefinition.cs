namespace PVZEngine
{
    public class ArmorDefinition : Definition
    {
        public virtual void PostUpdate(Armor armor) { }
        public NamespaceID GetModelID()
        {
            return GetID().ToModelID(ModelID.TYPE_ARMOR);
        }
    }
}
