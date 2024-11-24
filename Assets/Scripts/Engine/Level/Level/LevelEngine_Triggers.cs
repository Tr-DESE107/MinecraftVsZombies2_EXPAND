using System.Collections.Generic;
using System.Linq;
using PVZEngine.Triggers;

namespace PVZEngine.Level
{
    public partial class LevelEngine
    {
        #region 公有方法
        public void AddTrigger(Trigger trigger)
        {
            Triggers.AddTrigger(trigger);
            addedTriggers.Add(trigger);
        }
        public bool RemoveTrigger(Trigger trigger)
        {
            if (Triggers.RemoveTrigger(trigger))
            {
                addedTriggers.Remove(trigger);
                return true;
            }
            return false;
        }
        public void AddTriggers(IEnumerable<Trigger> triggers)
        {
            foreach (var trigger in triggers)
            {
                AddTrigger(trigger);
            }
        }
        public int RemoveTriggers(IEnumerable<Trigger> triggers)
        {
            int value = 0;
            foreach (var trigger in triggers.ToArray())
            {
                value += RemoveTrigger(trigger) ? 1 : 0;
            }
            return value;
        }
        #endregion

        #region 属性字段
        public IGameTriggerSystem Triggers { get; }
        private List<Trigger> addedTriggers = new List<Trigger>();
        #endregion
    }
}
