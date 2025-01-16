using System;
using PVZEngine.Auras;
using PVZEngine.Buffs;
using PVZEngine.Level;

namespace PVZEngine.SeedPacks
{
    public abstract class SerializableSeedPack
    {
        public long id;
        public NamespaceID seedID;
        public long currentBuffID;
        public SerializableBuffList buffs;
        public SerializablePropertyBlock properties;
        public SerializableAuraEffect[] auras;
    }
    [Serializable]
    public class SerializableClassicSeedPack : SerializableSeedPack
    {
    }
    [Serializable]
    public class SerializableConveyorSeedPack : SerializableSeedPack
    {
    }
}
