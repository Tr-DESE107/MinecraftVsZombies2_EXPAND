using System;
using PVZEngine;
using PVZEngine.Auras;
using PVZEngine.Buffs;

namespace MVZ2Logic.Artifacts
{
    [Serializable]
    public class SerializableArtifact
    {
        public NamespaceID definitionID;
        public SerializablePropertyDictionary propertyDict;
        public SerializableAuraEffect[] auras;
    }
}
