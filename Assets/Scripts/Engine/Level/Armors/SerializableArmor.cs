﻿using System;
using PVZEngine.Auras;
using PVZEngine.Buffs;
using PVZEngine.Level;

namespace PVZEngine.Armors
{
    [Serializable]
    public class SerializableArmor
    {
        public NamespaceID definitionID;
        public float health;
        public NamespaceID slot;
        [Obsolete]
        public long currentBuffID;
        public SerializableBuffList buffs;
        public SerializablePropertyBlock properties;
        public SerializableAuraEffect[] auras;
    }
}
