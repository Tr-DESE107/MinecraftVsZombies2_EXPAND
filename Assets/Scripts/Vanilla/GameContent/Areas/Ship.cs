using System.Collections.Generic;
using MVZ2.GameContent.Armors;
using MVZ2.GameContent.Buffs.Contraptions;
using MVZ2.GameContent.Enemies;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Level;
using MVZ2.Vanilla.Properties;
using MVZ2Logic.Level;
using PVZEngine;
using PVZEngine.Buffs;
using PVZEngine.Definitions;
using PVZEngine.Entities;
using PVZEngine.Grids;
using PVZEngine.Level;
using Tools;
using UnityEngine;

namespace MVZ2.GameContent.Areas
{
    [AreaDefinition(VanillaAreaNames.ship)]
    public class Ship : AreaDefinition
    {
        public Ship(string nsp, string name) : base(nsp, name)
        {
        }
        public override void Setup(LevelEngine level)
        {
            base.Setup(level);
            SetSkyOffsetSpeed(level, SKY_OFFSET_SPEED_NORMAL);
            SetRNG(level, level.CreateRNG());
        }
        public override void Update(LevelEngine level)
        {
            base.Update(level);
            var skyOffsetSpeed = GetSkyOffsetSpeed(level);
            var targetSpeed = SKY_OFFSET_SPEED_NORMAL;
            if (level.IsDuringHugeWave())
            {
                targetSpeed = SKY_OFFSET_SPEED_FAST;
            }
            var accel = (targetSpeed - skyOffsetSpeed) * SKY_OFFSET_ACCELERATION;
            if (skyOffsetSpeed != targetSpeed)
            {
                if (skyOffsetSpeed < targetSpeed == skyOffsetSpeed + accel > targetSpeed)
                {
                    skyOffsetSpeed = targetSpeed;
                }
                else
                {
                    skyOffsetSpeed += accel;
                }
            }
            SetSkyOffsetSpeed(level, skyOffsetSpeed);
            level.SetModelAnimatorFloat("SkyOffsetSpeed", skyOffsetSpeed);
        }
        public override void PostHugeWaveEvent(LevelEngine level)
        {
            base.PostHugeWaveEvent(level);
            SpawnParatroops(level, 3);
        }
        public static void SpawnParatroops(LevelEngine level, int count)
        {
            List<LawnGrid> valid = new List<LawnGrid>();
            List<int> weights = new List<int>();

            for (int col = SPAWNER_MIN_COLUMN; col < level.GetMaxColumnCount(); col++)
            {
                for (int lane = 0; lane < level.GetMaxLaneCount(); lane++)
                {
                    var grid = level.GetGrid(col, lane);
                    valid.Add(grid);
                    weights.Add(GetParatroopWeight(col));
                }
            }
            count = Mathf.Clamp(count, 0, valid.Count);
            if (count <= 0)
                return;

            var rng = GetRNG(level);
            var grids = valid.WeightedRandomTake(weights.ToArray(), count, rng);
            foreach (var grid in grids)
            {
                var entityToSpawn = GetParatroopToSpawn(rng);
                SpawnParatroopOnGrid(level, entityToSpawn, grid);
            }
            level.PlaySound(VanillaSoundID.wind);
        }
        public static Entity SpawnParatroopOnGrid(LevelEngine level, NamespaceID enemyID, LawnGrid grid)
        {
            var position = grid.GetEntityPosition() + Vector3.up * 600;
            var entity = level.Spawn(enemyID, position, null);
            entity.EquipArmorTo(VanillaArmorSlots.shield, VanillaArmorID.umbrellaShield);
            entity.AddBuff<ParatroopBuff>();
            return entity;
        }
        private static int GetParatroopWeight(int column)
        {
            return column - SPAWNER_MIN_COLUMN + 1;
        }
        private static NamespaceID GetParatroopToSpawn(RandomGenerator rng)
        {
            return paratroopsToSpawn.Random(rng);
        }
        public static float GetSkyOffsetSpeed(LevelEngine level) => level.GetProperty<float>(PROP_SKY_OFFSET_SPEED);
        public static void SetSkyOffsetSpeed(LevelEngine level, float value) => level.SetProperty<float>(PROP_SKY_OFFSET_SPEED, value);
        public static RandomGenerator GetRNG(LevelEngine level) => level.GetBehaviourField<RandomGenerator>(PROP_RNG);
        public static void SetRNG(LevelEngine level, RandomGenerator rng) => level.SetBehaviourField(PROP_RNG, rng);

        public static readonly NamespaceID[] paratroopsToSpawn = new NamespaceID[]
        {
            VanillaEnemyID.zombie,
            VanillaEnemyID.leatherCappedZombie,
            VanillaEnemyID.ironHelmettedZombie
        };
        public const int SPAWNER_MIN_COLUMN = 5;
        public const float SKY_OFFSET_SPEED_NORMAL = 1;
        public const float SKY_OFFSET_SPEED_FAST = 10;
        public const float SKY_OFFSET_ACCELERATION = 0.1f;
        public static readonly VanillaLevelPropertyMeta<RandomGenerator> PROP_RNG = new VanillaLevelPropertyMeta<RandomGenerator>("SpawnerRNG");
        public static readonly VanillaLevelPropertyMeta<float> PROP_SKY_OFFSET_SPEED = new VanillaLevelPropertyMeta<float>("sky_offset_speed");
    }
}