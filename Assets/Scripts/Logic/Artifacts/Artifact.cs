using System;
using System.Linq;
using MVZ2Logic.Callbacks;
using PVZEngine;
using PVZEngine.Auras;
using PVZEngine.Entities;
using PVZEngine.Level;
using PVZEngine.Modifiers;

namespace MVZ2Logic.Artifacts
{
    public class Artifact
    {
        public Artifact(LevelEngine level, ArtifactDefinition definition)
        {
            Level = level;
            Definition = definition;

            var auraDefs = definition.GetAuras();
            for (int i = 0; i < auraDefs.Length; i++)
            {
                var auraDef = auraDefs[i];
                auras.Add(level, new AuraEffect(auraDef, i));
            }
        }
        public void Update()
        {
            auras.Update(Level);
            if (Definition != null)
            {
                Definition.PostUpdate(this);
            }
        }
        public void Highlight()
        {
            OnHighlighted?.Invoke(this);
        }
        internal void PostAdd()
        {
            auras.PostAdd(Level);
            Definition.PostAdd(this);
        }
        internal void PostRemove()
        {
            auras.PostRemove(Level);
            Definition.PostRemove(this);
        }
        public T GetProperty<T>(string name)
        {
            if (propertyDict.TryGetProperty<T>(name, out var thisProp))
                return thisProp;
            return Definition.GetProperty<T>(name);
        }
        public void SetProperty(string name, object value)
        {
            propertyDict.SetProperty(name, value);
        }
        public SerializableArtifact Serialize()
        {
            return new SerializableArtifact()
            {
                definitionID = Definition.GetID(),
                propertyDict = propertyDict.Serialize(),
                auras = auras.GetAll().Select(a => a.ToSerializable()).ToArray()
            };
        }
        public static Artifact Deserialize(SerializableArtifact seri, LevelEngine level)
        {
            var definition = level.Content.GetArtifactDefinition(seri.definitionID);
            var buff = new Artifact(level, definition);
            buff.propertyDict = PropertyDictionary.Deserialize(seri.propertyDict);
            buff.auras.LoadFromSerializable(level, seri.auras);
            return buff;
        }
        public event Action<Artifact> OnHighlighted;
        public LevelEngine Level { get; }
        public ArtifactDefinition Definition { get; }
        private PropertyDictionary propertyDict = new PropertyDictionary();
        private AuraEffectList auras = new AuraEffectList();
    }
}
