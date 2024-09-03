using PVZEngine.LevelManaging;

namespace PVZEngine.Serialization
{
    public class SerializableBuffTarget : ISerializeBuffTarget
    {
        public SerializableBuffTarget(Entity entity)
        {
            entityID = entity.ID;
            isArmor = false;
        }
        public SerializableBuffTarget(Armor armor)
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
}
