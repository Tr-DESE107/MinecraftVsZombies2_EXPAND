using System;
using System.Linq;
using MVZ2Logic.Games;
using PVZEngine;
using PVZEngine.Auras;
using PVZEngine.Entities;
using PVZEngine.Level;

namespace MVZ2Logic.Artifacts
{
    public class Artifact : IAuraSource
    {
        public Artifact(LevelEngine level, ArtifactDefinition definition)
        {
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
        public void Highlight()
        {
            OnHighlighted?.Invoke(this);
        }
        internal void PostAdd()
        {
            auras.PostAdd();
            Definition.PostAdd(this);
        }
        internal void PostRemove()
        {
            auras.PostRemove();
            Definition.PostRemove(this);
        }
        public T GetProperty<T>(PropertyKey name)
        {
            if (propertyDict.TryGetProperty<T>(name, out var thisProp))
                return thisProp;
            return Definition.GetProperty<T>(name);
        }
        public AuraEffect GetAuraEffect<T>() where T : AuraEffectDefinition
        {
            return auras.Get<T>();
        }
        public void SetProperty(PropertyKey name, object value)
        {
            propertyDict.SetProperty(name, value);
        }
        public SerializableArtifact Serialize()
        {
            return new SerializableArtifact()
            {
                definitionID = Definition.GetID(),
                propertyDict = propertyDict.ToSerializable(),
                auras = auras.GetAll().Select(a => a.ToSerializable()).ToArray()
            };
        }
        public static Artifact Deserialize(SerializableArtifact seri, LevelEngine level)
        {
            var definition = level.Content.GetArtifactDefinition(seri.definitionID);
            var artifact = new Artifact(level, definition);
            artifact.propertyDict = PropertyDictionary.FromSerializable(seri.propertyDict);
            artifact.auras.LoadFromSerializable(level, seri.auras);
            return artifact;
        }

        Entity IAuraSource.GetEntity() => null;
        LevelEngine IAuraSource.GetLevel() => Level;
        public event Action<Artifact> OnHighlighted;
        public LevelEngine Level { get; }
        public ArtifactDefinition Definition { get; }
        private PropertyDictionary propertyDict = new PropertyDictionary();
        private AuraEffectList auras = new AuraEffectList();
    }
}
