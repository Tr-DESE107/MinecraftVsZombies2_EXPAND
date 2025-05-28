using System;
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
        public long currentBuffID;
        public SerializableBuffList buffs;
        public SerializablePropertyBlock properties;
    }
}
