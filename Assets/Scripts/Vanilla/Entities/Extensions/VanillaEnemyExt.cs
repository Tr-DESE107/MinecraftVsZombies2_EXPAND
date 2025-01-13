using MVZ2.GameContent.Buffs.Enemies;
using MVZ2.Vanilla.Callbacks;
using PVZEngine.Buffs;
using PVZEngine.Entities;
using PVZEngine.Triggers;

namespace MVZ2.Vanilla.Entities
{
    public static class VanillaEnemyExt
    {
        public static void Neutralize(this Entity enemy)
        {
            if (enemy.IsNeutralized())
                return;

            var result = new TriggerResultBoolean();
            result.Result = true;
            foreach (var trigger in enemy.Level.Triggers.GetTriggers(VanillaLevelCallbacks.PRE_ENEMY_NEUTRALIZE))
            {
                trigger.Run(c => c(enemy, result));
                if (result.Result == false)
                    return;
            }
            enemy.SetNeutralized(true);
            enemy.DropRewards();
            enemy.Level.Triggers.RunCallback(VanillaLevelCallbacks.POST_ENEMY_NEUTRALIZE, c => c(enemy));
        }
        public static void DropRewards(this Entity enemy)
        {
            if (enemy.HasNoReward())
                return;
            enemy.Level.Triggers.RunCallback(VanillaLevelCallbacks.ENEMY_DROP_REWARDS, c => c(enemy));
        }
        public static void InflictWeakness(this Entity enemy, int time)
        {
            var buffs = enemy.GetBuffs<EnemyWeaknessBuff>();
            Buff buff;
            if (buffs.Length <= 0)
            {
                buff = enemy.AddBuff<EnemyWeaknessBuff>();
            }
            else
            {
                buff = buffs[0];
            }
            buff.SetProperty(EnemyWeaknessBuff.PROP_TIMEOUT, time);
        }
    }
}
