using System.Collections.Generic;
using MVZ2.GameContent.Areas;
using MVZ2.GameContent.Buffs.Level;
using MVZ2.GameContent.Effects;
using MVZ2.GameContent.Enemies;
using MVZ2.Vanilla.Entities;
using PVZEngine;
using PVZEngine.Definitions;
using PVZEngine.Grids;
using PVZEngine.Level;
using Tools;
using UnityEngine;

namespace MVZ2.GameContent.Spawns
{
    public class UFOSpawnInLevelBehaviour : ISpawnInLevelBehaviour
    {
        public UFOSpawnInLevelBehaviour(int spawnLevel)
        {
            SpawnLevel = spawnLevel;
        }

        public void PreSpawnAtWave(LevelEngine level, int wave, ref float totalPoints)
        {
            if (wave <= 3)
                return;
            var flagModular = wave % level.GetWavesPerFlag();
            var intervalWaves = level.GetWavesPerFlag() / 2;
            var modular = flagModular % intervalWaves;
            if (modular == (intervalWaves - 1))
            {
                int count = Mathf.Min(10, Mathf.CeilToInt(totalPoints * 0.3333333f));
                List<int> typePool = new List<int>();
                UndeadFlyingObject.FillUFOVariantRandomPool(level, typePool);

                if (typePool.Count <= 0)
                {
                    return;
                }
                var rng = new RandomGenerator(level.GetSpawnRNG().Next());
                var startIndex = rng.Next(typePool.Count);
                for (int i = 0; i < count; i++)
                {
                    var typeIndex = (startIndex + i) % typePool.Count;
                    var variant = typePool[typeIndex];

                    if (rng.Next(100) < 10)
                    {
                        variant = UndeadFlyingObject.VARIANT_RAINBOW;
                    }
                    if (level.AreaID == VanillaAreaID.ship)
                    {
                        var x = rng.Next(UFOBackground.MIN_X, UFOBackground.MAX_X);
                        var z = rng.Next(UFOBackground.MIN_Z, UFOBackground.MAX_Z);
                        var y = rng.Next(UFOBackground.MIN_Y, UFOBackground.MAX_Y);
                        var pos = new Vector3(x, y, z);
                        var background = level.Spawn(VanillaEffectID.ufoBackground, pos, null);

                        var velocity = UFOBackground.FLY_DIRECTION * rng.Next(UFOBackground.MIN_SPEED, UFOBackground.MAX_SPEED);
                        background.Velocity = velocity;
                        background.SetVariant(variant);
                    }
                    var buff = level.AddBuff<UFOSpawnBuff>();
                    UFOSpawnBuff.SetVariant(buff, variant);
                }
            }
            else if (modular == 0)
            {
                var rng = level.GetSpawnRNG();
                HashSet<LawnGrid> possibleGrids = new HashSet<LawnGrid>();
                foreach (var buff in level.GetBuffs<UFOSpawnBuff>())
                {
                    var type = UFOSpawnBuff.GetVariant(buff);
                    possibleGrids.Clear();
                    UndeadFlyingObject.FillUFOPossibleSpawnGrids(level, type, possibleGrids);

                    var filteredGrids = UndeadFlyingObject.FilterConflictSpawnGrids(level, possibleGrids);

                    var grid = filteredGrids.Random(rng);
                    var column = grid.Column;
                    var lane = grid.Lane;
                    var pos = grid.GetEntityPosition();
                    pos.y += UndeadFlyingObject.START_HEIGHT;
                    var ufo = level.Spawn(VanillaEnemyID.ufo, pos, null);
                    ufo.SetVariant(type);
                    UndeadFlyingObject.SetTargetGridX(ufo, column);
                    UndeadFlyingObject.SetTargetGridY(ufo, lane);

                    totalPoints -= SpawnLevel;
                    buff.Remove();
                }
            }
        }
        public int GetRandomSpawnLane(LevelEngine level)
        {
            return level.GetRandomEnemySpawnLane();
        }
        public bool CanSpawnInLevel(LevelEngine level) => false;
        public int GetWeight(LevelEngine level) => 0;
        public int GetSpawnLevel(LevelEngine level) => SpawnLevel;
        public NamespaceID GetSpawnEntityID() => VanillaEnemyID.ufo;
        public int SpawnLevel { get; }
    }
}
