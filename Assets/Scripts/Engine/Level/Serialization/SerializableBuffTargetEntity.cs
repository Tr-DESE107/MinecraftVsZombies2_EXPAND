using System;
using PVZEngine.Level;

namespace PVZEngine.Serialization
{
    [Serializable]
    public class SerializableBuffTargetEntity : ISerializeBuffTarget
    {
        public SerializableBuffTargetEntity(Entity entity)
        {
            entityID = entity.ID;
            isArmor = false;
        }
        public SerializableBuffTargetEntity(Armor armor)
        {
            entityID = armor.Owner.ID;
            isArmor = true;
        }
        IBuffTarget ISerializeBuffTarget.DeserializeBuffTarget(LevelEngine level)
        {
            var entity = level.FindEntityByID(entityID);
            if (isArmor)
                return entity.EquipedArmor;
            return entity;
        }
        public int entityID;
        public bool isArmor;
    }
    [Serializable]
    public class SerializableBuffTargetSeedPack : ISerializeBuffTarget
    {
        public SerializableBuffTargetSeedPack(SeedPack seedPack)
        {
            index = seedPack.GetIndex();
        }
        IBuffTarget ISerializeBuffTarget.DeserializeBuffTarget(LevelEngine level)
        {
            return level.GetSeedPackAt(index);
        }
        public int index;
    }
    [Serializable]
    public class SerializableBuffTargetLevel : ISerializeBuffTarget
    {
        public SerializableBuffTargetLevel(LevelEngine level)
        {
        }
        IBuffTarget ISerializeBuffTarget.DeserializeBuffTarget(LevelEngine level)
        {
            return level;
        }
    }
}
