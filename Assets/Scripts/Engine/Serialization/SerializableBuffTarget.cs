using System.Collections.Generic;
using UnityEngine;

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
        IBuffTarget ISerializeBuffTarget.DeserializeBuffTarget(Game level)
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
