using System.Collections.Generic;
using MVZ2.GameContent.Areas;
using MVZ2.GameContent.Contraptions;
using MVZ2.GameContent.Enemies;
using MVZ2.GameContent.Projectiles;
using MVZ2.Vanilla.Buffs;
using PVZEngine;

namespace MVZ2.Vanilla
{
    public class Vanilla : Mod
    {
        public Vanilla() : base("mvz2")
        {
            AddDefinition(areaDefinitions,"day", new Day());
            AddDefinition(stageDefinitions, "prologue", new StageDefinition());
            AddDefinition(buffDefinitions, "randomEnemySpeed", new RandomEnemySpeedBuff());

            AddDefinition(entityDefinitions, "dispenser", new Dispenser());
            AddDefinition(entityDefinitions, "zombie", new Zombie());
            AddDefinition(entityDefinitions, "arrow", new Arrow());
        }
    }
}
