using System.Collections.Generic;
using System.Linq;
using PVZEngine.Entities;
using UnityEngine;

namespace PVZEngine.Level
{
    public partial class LevelEngine
    {
        public void SetEnergy(float value)
        {
            Energy = Mathf.Clamp(value, 0, Option.MaxEnergy);
        }
        public void AddEnergy(float value)
        {
            SetEnergy(Energy + value);
        }
        public void AddEnergyDelayed(Entity source, float value)
        {
            if (value == 0)
                return;
            var before = Energy;
            AddEnergy(value);
            var added = Energy - before;
            delayedEnergyEntities.Add(source, added);
        }
        public bool RemoveEnergyDelayedEntity(Entity entity)
        {
            return delayedEnergyEntities.Remove(entity);
        }
        public void ClearEnergyDelayedEntities()
        {
            delayedEnergyEntities.Clear();
        }
        public float GetDelayedEnergy()
        {
            return delayedEnergyEntities.Values.Sum();
        }
        private void UpdateDelayedEnergyEntities()
        {
            var entities = delayedEnergyEntities.Keys.Where(e => !e.Exists()).ToArray();
            foreach (var entity in entities)
            {
                delayedEnergyEntities.Remove(entity);
            }
        }
        public float Energy { get; private set; }
        private Dictionary<Entity, float> delayedEnergyEntities = new Dictionary<Entity, float>();
    }
}