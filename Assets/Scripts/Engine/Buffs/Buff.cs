using System;
using Newtonsoft.Json;
using PVZEngine.Serialization;

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
        public void AddToTarget(IBuffTarget target)
        {
            if (Target != null)
                return;
            Target = target;
            foreach (var modifier in GetModifiers())
            {
                modifier.PostAdd(this);
            }
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
        }
        public SerializableBuff Serialize()
        {
            return new SerializableBuff()
            {
                definitionID = Definition.GetID(),
                target = Target.SerializeBuffTarget(),
                propertyDict = propertyDict.Serialize()
            };
        }
        public static Buff Deserialize(SerializableBuff seri, Level level)
        {
            var definition = level.GetBuffDefinition(seri.definitionID);
            var buff = new Buff(definition);
            buff.Target = seri.target.DeserializeBuffTarget(level);
            buff.propertyDict = PropertyDictionary.Deserialize(seri.propertyDict, level);
            return buff;
        }
        public BuffDefinition Definition { get; }
        public IBuffTarget Target { get; private set; }
        private PropertyDictionary propertyDict = new PropertyDictionary();
    }
}
