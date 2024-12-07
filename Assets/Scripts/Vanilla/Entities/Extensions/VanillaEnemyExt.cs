using MVZ2.Vanilla.Callbacks;
using PVZEngine.Entities;

namespace MVZ2.Vanilla.Entities
{
    public static class VanillaEnemyExt
    {
        public static void Neutralize(this Entity enemy)
        {
            if (enemy.IsNeutralized())
                return;

            foreach (var trigger in enemy.Level.Triggers.GetTriggers(VanillaLevelCallbacks.PRE_ENEMY_NEUTRALIZE))
            {
                var result = trigger.Invoke(enemy);
                if (result is bool boolValue && !boolValue)
                    return;
            }
            enemy.SetNeutralized(true);
            enemy.DropRewards();
            enemy.Level.Triggers.RunCallback(VanillaLevelCallbacks.POST_ENEMY_NEUTRALIZE, enemy);
        }
        public static void DropRewards(this Entity enemy)
        {
            if (enemy.HasNoReward())
                return;
            enemy.Level.Triggers.RunCallback(VanillaLevelCallbacks.ENEMY_DROP_REWARDS, enemy);
        }
    }
}
