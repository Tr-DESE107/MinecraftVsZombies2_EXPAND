using PVZEngine.LevelManagement;

namespace PVZEngine.Serialization
{
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
        IBuffTarget ISerializeBuffTarget.DeserializeBuffTarget(Level level)
        {
            var entity = level.FindEntityByID(entityID);
            if (isArmor)
                return entity.EquipedArmor;
            return entity;
        }
        public int entityID;
        public bool isArmor;
    }
    public class SerializableBuffTargetSeedPack : ISerializeBuffTarget
    {
        public SerializableBuffTargetSeedPack(SeedPack seedPack)
        {
            index = seedPack.GetIndex();
        }
        IBuffTarget ISerializeBuffTarget.DeserializeBuffTarget(Level level)
        {
            return level.GetSeedPackAt(index);
        }
        public int index;
    }
    public class SerializableBuffTargetLevel : ISerializeBuffTarget
    {
        public SerializableBuffTargetLevel(Level level)
        {
        }
        IBuffTarget ISerializeBuffTarget.DeserializeBuffTarget(Level level)
        {
            return level;
        }
    }
}
