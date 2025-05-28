﻿using MukioI18n;
using MVZ2.GameContent.Contraptions;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Entities;
using PVZEngine.Entities;
using Tools;

namespace MVZ2.GameContent.RandomChinaEvents
{
    [RandomChinaEventDefinition(VanillaRandomChinaEventNames.anvilShower)]
    public class AnvilShowerEvent : RandomChinaEventDefinition
    {
        public AnvilShowerEvent(string nsp, string path) : base(nsp, path, NAME)
        {
        }
        public override void Run(Entity contraption, RandomGenerator rng)
        {
            var level = contraption.Level;
            foreach (var enemy in level.FindEntities(e => e.Type == EntityTypes.ENEMY && e.IsHostile(contraption) && e.ExistsAndAlive()))
            {
                int column = enemy.GetColumn();
                var lane = enemy.GetLane();
                var grid = level.GetGrid(column, lane);
                if (grid != null)
                {
                    var pos = grid.GetEntityPosition();
                    pos.y = 600;
                    contraption.SpawnWithParams(VanillaContraptionID.anvil, pos);
                }
            }
        }
        [TranslateMsg("随机瓷器事件名称", VanillaStrings.CONTEXT_RANDOM_CHINA_EVENT_NAME)]
        public const string NAME = "铁砧雨";
    }
}
