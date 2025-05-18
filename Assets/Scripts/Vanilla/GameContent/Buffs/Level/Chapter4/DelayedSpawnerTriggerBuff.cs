using MVZ2.GameContent.Obstacles;
using MVZ2.Vanilla.Properties;
using PVZEngine.Buffs;
using PVZEngine.Level;
using Tools;

namespace MVZ2.GameContent.Buffs.Level
{
    [BuffDefinition(VanillaBuffNames.Level.delayedSpawnerTrigger)]
    public class DelayedSpawnerTriggerBuff : BuffDefinition
    {
        public DelayedSpawnerTriggerBuff(string nsp, string name) : base(nsp, name)
        {
        }
        public override void PostAdd(Buff buff)
        {
            base.PostAdd(buff);
            buff.SetProperty(PROP_TIMER, new FrameTimer(MAX_TIMEOUT));
        }
        public override void PostUpdate(Buff buff)
        {
            base.PostUpdate(buff);
            var timer = buff.GetProperty<FrameTimer>(PROP_TIMER);
            if (timer == null || timer.Expired)
            {
                foreach (var spawner in buff.Level.FindEntities(VanillaObstacleID.monsterSpawner))
                {
                    MonsterSpawner.Trigger(spawner);
                }
                buff.Remove();
            }
            else
            {
                timer.Run();
            }
        }
        public const int MAX_TIMEOUT = 60;
        public static readonly VanillaBuffPropertyMeta PROP_TIMER = new VanillaBuffPropertyMeta("Timer");
    }
}
