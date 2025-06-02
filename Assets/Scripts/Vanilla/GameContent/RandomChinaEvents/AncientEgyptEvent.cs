﻿using MukioI18n;
using MVZ2.GameContent.Buffs.Level;
using MVZ2.GameContent.Enemies;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Level;
using PVZEngine;
using PVZEngine.Entities;
using Tools;

namespace MVZ2.GameContent.RandomChinaEvents
{
    [RandomChinaEventDefinition(VanillaRandomChinaEventNames.ancientEgypt)]
    public class AncientEgyptEvent : RandomChinaEventDefinition
    {
        public AncientEgyptEvent(string nsp, string path) : base(nsp, path, NAME)
        {
        }
        public override void Run(Entity contraption, RandomGenerator rng)
        {
            var level = contraption.Level;
            level.AddBuff<AncientEgyptBuff>();
            if (!level.IsAllEnemiesCleared() && !level.IsCleared)
            {
                var spawn = level.Content.GetSpawnDefinition(VanillaSpawnID.mummy);
                for (int lane = 0; lane < level.GetMaxLaneCount(); lane++)
                {
                    level.SpawnEnemy(spawn, lane);
                }
            }
            contraption.PlaySound(VanillaSoundID.lowQualityEgypt);
        }
        [TranslateMsg("随机瓷器事件名称", VanillaStrings.CONTEXT_RANDOM_CHINA_EVENT_NAME)]
        public const string NAME = "神秘埃及";
    }
}
