using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PVZEngine
{
    public partial class Game
    {
        public void SetEnergy(float value)
        {
            Energy = value;
            Energy = Mathf.Clamp(Energy, 0, Option.MaxEnergy);
        }
        public void AddEnergy(float value)
        {
            SetEnergy(Energy + value);
        }
        public void AddEnergyDelayed(Entity source, float value)
        {
            var before = Energy;
            AddEnergy(value);
            var added = Energy - before;
            delayedEnergyEntities.Add(new EntityReference(source), added);
        }
        public bool RemoveEnergyDelayedEntity(Entity entity)
        {
            var key = delayedEnergyEntities.Keys.FirstOrDefault(k => k.ID == entity.ID);
            if (key == null)
                return false;
            return delayedEnergyEntities.Remove(key);
        }
        public float GetDelayedEnergy()
        {
            return delayedEnergyEntities.Values.Sum();
        }
        public float Energy { get; private set; }
        private Dictionary<EntityReference, float> delayedEnergyEntities = new Dictionary<EntityReference, float>();
    }
}