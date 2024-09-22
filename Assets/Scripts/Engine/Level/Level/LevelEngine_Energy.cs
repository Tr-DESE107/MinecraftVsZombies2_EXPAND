using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PVZEngine.Level
{
    public partial class LevelEngine
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
            delayedEnergyEntities.Add(source, added);
        }
        public bool RemoveEnergyDelayedEntity(Entity entity)
        {
            return delayedEnergyEntities.Remove(entity);
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