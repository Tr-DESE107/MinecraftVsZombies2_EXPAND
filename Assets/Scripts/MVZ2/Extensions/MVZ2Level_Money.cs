using MVZ2.Level.Components;
using PVZEngine.Entities;
using PVZEngine.Level;

namespace MVZ2.Extensions
{
    public static partial class MVZ2Level
    {
        public static IMoneyComponent GetMoneyComponent(this LevelEngine level)
        {
            return level.GetComponent<IMoneyComponent>();
        }
        public static void AddMoney(this LevelEngine level, int value)
        {
            var component = level.GetMoneyComponent();
            component.AddMoney(value);
        }
        public static int GetMoney(this LevelEngine level)
        {
            var component = level.GetMoneyComponent();
            return component.GetMoney();
        }
        public static void AddDelayedMoney(this LevelEngine level, Entity entity, int value)
        {
            var component = level.GetMoneyComponent();
            component.AddDelayedMoney(entity, value);
        }
        public static bool RemoveDelayedMoney(this LevelEngine level, Entity entity)
        {
            var component = level.GetMoneyComponent();
            return component.RemoveDelayedMoney(entity);
        }
        public static int GetDelayedMoney(this LevelEngine level)
        {
            var component = level.GetMoneyComponent();
            return component.GetDelayedMoney();
        }
        public static void ClearDelayedMoney(this LevelEngine level)
        {
            var component = level.GetMoneyComponent();
            component.ClearDelayedMoney();
        }
    }
}
