using System;
using System.Collections.Generic;
using PVZEngine.Base;
using PVZEngine.Callbacks;
using PVZEngine.Level;

namespace PVZEngine.Definitions
{
    public abstract class StageDefinition : Definition
    {
        public StageDefinition(string nsp, string name) : base(nsp, name)
        {
            SetProperty(StageProperties.FIRST_WAVE_TIME, 540);
            SetProperty(StageProperties.WAVES_PER_FLAG, 10);
        }
        public virtual void Start(LevelEngine level) { }
        public virtual void Update(LevelEngine level) { }
        public virtual void PrepareForBattle(LevelEngine level) { }
        public virtual void PostWave(LevelEngine level, int wave) { }
        public virtual void PostHugeWave(LevelEngine level) { }
        public virtual void PostFinalWave(LevelEngine level) { }
        public virtual void PostEnemySpawned(Entity entity) { }
        public void AddCallbacks()
        {
            foreach (var action in addCallbackActions)
            {
                action?.Invoke();
            }
        }
        public void RemoveCallbacks()
        {
            foreach (var action in removeCallbackActions)
            {
                action?.Invoke();
            }
        }
        protected void AddCallback<TEntry, TDelegate>(CallbackListBase<TEntry, TDelegate> callbackList, TDelegate action, int priority = 0, object filter = null)
            where TDelegate : Delegate
            where TEntry : CallbackActionBase<TDelegate>, new()
        {
            addCallbackActions.Add(() => callbackList.Add(action, priority, filter));
            removeCallbackActions.Add(() => callbackList.Remove(action));
        }
        public virtual IEnumerable<IEnemySpawnEntry> GetEnemyPool()
        {
            yield break;
        }
        private List<Action> addCallbackActions = new List<Action>();
        private List<Action> removeCallbackActions = new List<Action>();
    }
}
