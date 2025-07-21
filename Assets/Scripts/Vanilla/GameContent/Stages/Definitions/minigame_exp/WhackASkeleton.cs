using MVZ2.GameContent.Contraptions;
using MVZ2.GameContent.Seeds;
using MVZ2.Vanilla.Level;
using PVZEngine;
using PVZEngine.Definitions;
using PVZEngine.Level;

namespace MVZ2.GameContent.Stages
{
    [StageDefinition(VanillaStageNames.WhackASkeleton)]
    public partial class WhackASkeleton : StageDefinition
    {
        public WhackASkeleton(string nsp, string name) : base(nsp, name)
        {
            var waveStageBehaviour = new WaveStageBehaviour(this);
            waveStageBehaviour.SpawnFlagZombie = false;
            AddBehaviour(waveStageBehaviour);
            AddBehaviour(new WhackASkeletonBehaviour(this));
            AddBehaviour(new FinalWaveClearBehaviour(this));
            AddBehaviour(new GemStageBehaviour(this));
            AddBehaviour(new StarshardStageBehaviour(this));
            AddBehaviour(new RedstoneDropStageBehaviour(this));
        }
        public override void OnSetup(LevelEngine level)
        {
            base.OnSetup(level);
            //level.StartRain();
        }
        public override void OnStart(LevelEngine level)
        {
            base.OnStart(level);

            //level.SetSeedSlotCount(3);
            //level.FillSeedPacks(new NamespaceID[]
            //{
            //    VanillaBlueprintID.FromEntity(VanillaContraptionID.glowstone),
            //    VanillaBlueprintID.FromEntity(VanillaContraptionID.obsidian),
            //    VanillaBlueprintID.FromEntity(VanillaContraptionID.gravityPad),
            //});
        }

    }
}
