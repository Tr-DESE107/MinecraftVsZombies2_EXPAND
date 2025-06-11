﻿using System;
using PVZEngine;
using PVZEngine.Auras;
using Tools;

namespace MVZ2Logic.Artifacts
{
    [Serializable]
    public class SerializableArtifact
    {
        public NamespaceID definitionID;
        public SerializableRNG rng;
        public SerializablePropertyDictionary propertyDict;
        public SerializableAuraEffect[] auras;
    }
}
