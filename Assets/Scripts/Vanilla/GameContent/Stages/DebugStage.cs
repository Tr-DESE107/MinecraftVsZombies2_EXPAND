using MVZ2.GameContent.Contraptions;
using MVZ2.GameContent.Enemies;
using MVZ2.Vanilla.Level;
using MVZ2Logic.Level;
using PVZEngine;
using PVZEngine.Definitions;
using PVZEngine.Level;

namespace MVZ2.GameContent.Stages
{
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
                VanillaContraptionID.dispenser,
                VanillaContraptionID.furnace,
                VanillaContraptionID.obsidian,
                VanillaContraptionID.mineTNT,

                VanillaContraptionID.smallDispenser,
                VanillaContraptionID.moonlightSensor,
                VanillaContraptionID.tnt,

                VanillaEnemyID.zombie,
                VanillaEnemyID.leatherCappedZombie,
                VanillaEnemyID.ironHelmettedZombie,
            });
            level.RechargeSpeed = 9999999;
            level.SetTriggerActive(true);
        }
        public override void OnUpdate(LevelEngine level)
        {
            base.OnUpdate(level);
            level.SetStarshardSlotCount(5);
            level.SetStarshardCount(5);
        }
    }
}
