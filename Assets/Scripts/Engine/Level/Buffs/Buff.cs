using System;
using System.Linq;
using PVZEngine.Auras;
using PVZEngine.Entities;
using PVZEngine.Level;
using PVZEngine.Models;
using PVZEngine.Modifiers;
using PVZEngine.SeedPacks;
using static UnityEngine.GraphicsBuffer;

namespace PVZEngine.Buffs
{
    public class Buff : IAuraSource, IModifierContainer
    {
        public Buff(LevelEngine level, BuffDefinition definition, long id)
        {
            ID = id;
            Level = level;
            Definition = definition;

            var auraDefs = definition.GetAuras();
            for (int i = 0; i < auraDefs.Length; i++)
            {
                var auraDef = auraDefs[i];
                auras.Add(level, new AuraEffect(auraDef, i, this));
            }
        }
        public void Update()
        {
            auras.Update();
            if (Definition != null)
            {
                Definition.PostUpdate(this);
            }
        }
        public T GetProperty<T>(PropertyKey<T> name)
        {
            return propertyDict.GetProperty<T>(name);
        }
        public void SetProperty<T>(PropertyKey<T> name, T value)
        {
            if (propertyDict.SetProperty<T>(name, value))
            {
                // 增益属性更改时，如果有利用该增益属性修改属性的修改器，调用一次属性更改时事件。
                foreach (var modifier in GetModifiers())
                {
                    if (name.Equals(modifier.UsingContainerPropertyName))
                    {
                        CallPropertyChanged(modifier.PropertyName);
                    }
                }
            }
        }
        public PropertyModifier[] GetModifiers()
        {
            return Definition.GetModifiers();
        }
        public PropertyModifier[] GetModifiers(IPropertyKey propName)
        {
            return Definition.GetModifiers(propName);
        }
        public ModelInsertion[] GetModelInsertions()
        {
            return Definition.GetModelInsertions();
        }
        public IModelInterface GetInsertedModel(NamespaceID key)
        {
            return Target?.GetInsertedModel(key);
        }
        public Entity GetEntity()
        {
            return Target?.GetEntity();
        }
        public SeedPack GetSeedPack()
        {
            return Target as SeedPack;
        }
        internal void AddToTarget(IBuffTarget target)
        {
            if (Target != null)
                return;
            Target = target;
            foreach (var modifier in GetModifiers())
            {
                modifier.PostAdd(this, target);
            }
            auras.PostAdd();
            Definition.PostAdd(this);
        }
        internal void RemoveFromTarget()
        {
            if (Target == null)
                return;
            foreach (var modifier in GetModifiers())
            {
                modifier.PostRemove(this, Target);
            }
            auras.PostRemove();
            Definition.PostRemove(this);
            Target = null;
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
                id = ID,
                definitionID = Definition.GetID(),
                propertyDict = propertyDict.ToSerializable(),
                auras = auras.GetAll().Select(a => a.ToSerializable()).ToArray()
            };
        }
        public static Buff Deserialize(SerializableBuff seri, LevelEngine level, IBuffTarget target)
        {
            var definition = level.Content.GetBuffDefinition(seri.definitionID);
            var buff = new Buff(level, definition, seri.id);
            buff.Target = target;
            buff.propertyDict = PropertyDictionary.FromSerializable(seri.propertyDict);
            buff.auras.LoadFromSerializable(level, seri.auras);
            return buff;
        }
        LevelEngine IAuraSource.GetLevel() { return Level; }
        T IModifierContainer.GetProperty<T>(PropertyKey<T> name) => GetProperty<T>(name);
        private void CallPropertyChanged(IPropertyKey name)
        {
            OnPropertyChanged?.Invoke(name);
        }
        public event Action<IPropertyKey> OnPropertyChanged;
        public long ID { get; }
        public LevelEngine Level { get; }
        public BuffDefinition Definition { get; }
        public IBuffTarget Target { get; private set; }
        private PropertyDictionary propertyDict = new PropertyDictionary();
        private AuraEffectList auras = new AuraEffectList();
    }
}
