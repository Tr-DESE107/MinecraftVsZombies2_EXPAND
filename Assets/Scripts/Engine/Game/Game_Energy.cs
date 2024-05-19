using System.Collections.Generic;
using System.Linq;

namespace PVZEngine
{
    public partial class Game
    {
        public void AddEnergy(float value)
        {
            Energy += value;
        }
        public void AddEnergyDelayEntity(Entity entity, float value)
        {
            delayedEnergyEntities.Add(new EntityReference(entity), value);
        }
        public bool RemoveEnergyDelayEntity(Entity entity)
        {
            var key = delayedEnergyEntities.Keys.FirstOrDefault(k => k.ID == entity.ID);
            if (key == null)
                return false;
            return delayedEnergyEntities.Remove(key);
        }
        public float Energy { get; set; }
        private Dictionary<EntityReference, float> delayedEnergyEntities = new Dictionary<EntityReference, float>();
    }
}