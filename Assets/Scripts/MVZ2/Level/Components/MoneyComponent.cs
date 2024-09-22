using System.Collections.Generic;
using System.Linq;
using MVZ2;
using MVZ2.GameContent;
using MVZ2.Level;
using MVZ2.Level.Components;

namespace PVZEngine.Level
{
    public partial class MoneyComponent : MVZ2Component, IMoneyComponent
    {
        public MoneyComponent(LevelEngine level, LevelController controller) : base(level, componentID, controller)
        {
        }
        public void AddMoney(int value)
        {
            Main.SaveManager.SetMoney(GetMoney() + value);
        }
        public int GetMoney()
        {
            return Main.SaveManager.GetMoney();
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
        public static readonly NamespaceID componentID = new NamespaceID(Builtin.spaceName, "money");
    }
}