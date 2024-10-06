using System;
using System.Collections.Generic;
using System.Linq;
using PVZEngine.Definitions;
using PVZEngine.Level.Triggers;
using PVZEngine.Modifiers;
using PVZEngine.Serialization;
using PVZEngine.Triggers;

namespace PVZEngine.Level
{
    public class Buff
    {
        public Buff(LevelEngine level, BuffDefinition definition)
        {
            Level = level;
            Definition = definition;
            triggers.AddRange(definition.GetTriggerCaches().Select(t => new BuffTrigger(this, t)));
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
        public BuffTrigger[] GetTriggers()
        {
            return triggers.ToArray();
        }
        internal void AddToTarget(IBuffTarget target)
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
        internal void RemoveFromTarget()
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
        public void Remove()
        {
            if (Target == null)
                return;
            Target.RemoveBuff(this);
        }
        public SerializableBuff Serialize()
        {
            return new SerializableBuff()
            {
                definitionID = Definition.GetID(),
                propertyDict = propertyDict.Serialize()
            };
        }
        public static Buff Deserialize(SerializableBuff seri, LevelEngine level, IBuffTarget target)
        {
            var definition = level.ContentProvider.GetBuffDefinition(seri.definitionID);
            var buff = new Buff(level, definition);
            buff.Target = target;
            buff.propertyDict = PropertyDictionary.Deserialize(seri.propertyDict);
            return buff;
        }
        public LevelEngine Level { get; }
        public BuffDefinition Definition { get; }
        public IBuffTarget Target { get; private set; }
        private PropertyDictionary propertyDict = new PropertyDictionary();
        private List<BuffTrigger> triggers = new List<BuffTrigger>();
    }
}
