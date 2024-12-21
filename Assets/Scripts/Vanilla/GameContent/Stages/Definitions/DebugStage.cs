using MVZ2.GameContent.Bosses;
using MVZ2.GameContent.Contraptions;
using MVZ2.GameContent.Enemies;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Level;
using MVZ2Logic.Level;
using PVZEngine;
using PVZEngine.Definitions;
using PVZEngine.Level;

namespace MVZ2.GameContent.Stages
{
    [Definition(VanillaStageNames.debug)]
    public partial class DebugStage : StageDefinition
    {
        public DebugStage(string nsp, string name) : base(nsp, name)
        {
        }
        public override void OnStart(LevelEngine level)
        {
            base.OnStart(level);
            level.SetEnergy(9990);
            level.SetSeedSlotCount(10);
            level.ReplaceSeedPacks(new NamespaceID[]
            {
                VanillaContraptionID.soulFurnace,
                VanillaContraptionID.furnace,
                VanillaContraptionID.mineTNT,
                VanillaContraptionID.punchton,

                VanillaContraptionID.magichest,
                VanillaContraptionID.tnt,

                VanillaEnemyID.ghost,
                VanillaEnemyID.ghast,
                VanillaEnemyID.leatherCappedZombie,
                VanillaBossID.frankenstein,
            });
            level.RechargeSpeed = 9999999;
            level.SetTriggerActive(true);
        }
        public override void OnUpdate(LevelEngine level)
        {
            base.OnUpdate(level);
            level.SetStarshardSlotCount(5);
            level.SetStarshardCount(5);
            level.CheckGameOver();
        }
    }
}
