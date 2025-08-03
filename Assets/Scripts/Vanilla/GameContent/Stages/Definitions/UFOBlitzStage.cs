using MVZ2.GameContent.Buffs.Level;
using MVZ2.GameContent.Enemies;
using PVZEngine.Definitions;
using PVZEngine.Level;

namespace MVZ2.GameContent.Stages
{
    public partial class UFOBlitzStage : StageDefinition
    {
        public UFOBlitzStage(string nsp, string name) : base(nsp, name)
        {
            AddBehaviour(new WaveStageBehaviour(this));
            AddBehaviour(new FinalWaveClearBehaviour(this));
            AddBehaviour(new GemStageBehaviour(this));
            AddBehaviour(new StarshardStageBehaviour(this));
            AddBehaviour(new ConveyorStageBehaviour(this));
        }
        public override void OnStart(LevelEngine level)
        {
            base.OnStart(level);
            var buff = level.AddBuff<UFOSpawnBuff>();
            buff.SetProperty(UFOSpawnBuff.PROP_VARIANT, UndeadFlyingObject.VARIANT_RED);
        }
    }
}
