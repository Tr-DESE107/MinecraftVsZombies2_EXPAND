using MVZ2.GameContent.Buffs.Enemies;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Level;
using MVZ2Logic.Level;
using PVZEngine;
using PVZEngine.Buffs;
using PVZEngine.Entities;
using PVZEngine.Level;

namespace MVZ2.GameContent.Enemies
{
    [EntityBehaviourDefinition(VanillaEnemyNames.flagZombie)]
    public class FlagZombie : Zombie
    {
        public FlagZombie(string nsp, string name) : base(nsp, name)
        {
        }

        public override void Init(Entity entity)
        {
            base.Init(entity);
            entity.SetModelProperty("HasFlag", true);
            var speedBuff = entity.GetFirstBuff<RandomEnemySpeedBuff>();
            if (speedBuff != null)
            {
                RandomEnemySpeedBuff.SetSpeed(speedBuff, 2);
            }
            var level = entity.Level;
            if (level.IsIZombie())
            {
                level.PlaySound(VanillaSoundID.siren);
                var regular = level.Content.GetSpawnDefinition(VanillaSpawnID.zombie);
                var leather = level.Content.GetSpawnDefinition(VanillaSpawnID.leatherCappedZombie);
                var iron = level.Content.GetSpawnDefinition(VanillaSpawnID.ironHelmettedZombie);
                for (var lane = 0; lane < entity.Level.GetMaxLaneCount(); lane++)
                {
                    entity.Level.SpawnEnemy(regular, lane);
                    entity.Level.SpawnEnemy(leather, lane);
                    entity.Level.SpawnEnemy(iron, lane);
                }
            }
        }
    }
}
