using System.Linq;
using PVZEngine.Definitions;
using PVZEngine.Modifiers;
using PVZEngine.Serialization;

namespace PVZEngine.Level
{
    public class Buff
    {
        public Buff(BuffDefinition definition)
        {
            Definition = definition;
        }
        public void Update()
        {
            if (Definition != null)
            {
                Definition.PostUpdate(this);
            }
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
        public PropertyModifier[] GetModifiers(string propName)
        {
            return Definition.GetModifiers(propName);
        }
        public void AddToTarget(IBuffTarget target)
        {
            if (Target != null)
                return;
            Target = target;
            foreach (var modifier in GetModifiers())
            {
                modifier.PostAdd(this);
            }
            Definition.PostAdd(this);
        }
        public void RemoveFromTarget()
        {
            if (Target == null)
                return;
            foreach (var modifier in GetModifiers())
            {
                modifier.PostRemove(this);
            }
            Target = null;
            Definition.PostRemove(this);
        }
        public SerializableBuff Serialize()
        {
            return new SerializableBuff()
            {
                definitionID = Definition.GetID(),
                propertyDict = propertyDict.Serialize()
            };
        }
        public static Buff Deserialize(SerializableBuff seri, IContentProvider provider, IBuffTarget target)
        {
            var definition = provider.GetBuffDefinition(seri.definitionID);
            var buff = new Buff(definition);
            buff.Target = target;
            buff.propertyDict = PropertyDictionary.Deserialize(seri.propertyDict);
            return buff;
        }
        public BuffDefinition Definition { get; }
        public IBuffTarget Target { get; private set; }
        private PropertyDictionary propertyDict = new PropertyDictionary();
    }
}
