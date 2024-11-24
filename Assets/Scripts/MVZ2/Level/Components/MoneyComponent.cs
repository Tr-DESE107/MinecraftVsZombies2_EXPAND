using System.Collections.Generic;
using System.Linq;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Saves;
using MVZ2Logic;
using MVZ2Logic.Level;
using MVZ2Logic.Level.Components;
using PVZEngine;
using PVZEngine.Entities;
using PVZEngine.Level;

namespace MVZ2.Level.Components
{
    public partial class MoneyComponent : MVZ2Component, IMoneyComponent
    {
        public MoneyComponent(LevelEngine level, LevelController controller) : base(level, componentID, controller)
        {
        }
        public void SetMoney(int value)
        {
            Global.Game.SetMoney(value);
        }
        public void AddMoney(int value)
        {
            SetMoney(GetMoney() + value);
        }
        public int GetMoney()
        {
            return Global.Game.GetMoney();
        }
        public int GetDelayedMoney()
        {
            return delayedMoneyEntities.Values.Sum();
        }
        public void AddDelayedMoney(Entity entity, int value)
        {
            var before = GetMoney();
            AddMoney(value);
            var added = GetMoney() - before;
            delayedMoneyEntities.Add(entity, added);
        }
        public bool RemoveDelayedMoney(Entity entity)
        {
            return delayedMoneyEntities.Remove(entity);
        }
        public void ClearDelayedMoney()
        {
            delayedMoneyEntities.Clear();
        }
        public override void Update()
        {
            base.Update();
            UpdateDelayedEnergyEntities();
        }
        private void UpdateDelayedEnergyEntities()
        {
            var entities = delayedMoneyEntities.Keys.Where(e => !e.Exists()).ToArray();
            foreach (var entity in entities)
            {
                delayedMoneyEntities.Remove(entity);
            }
        }
        // 不保存这个
        private Dictionary<Entity, int> delayedMoneyEntities = new Dictionary<Entity, int>();
        public static readonly NamespaceID componentID = new NamespaceID(VanillaMod.spaceName, "money");
    }
}