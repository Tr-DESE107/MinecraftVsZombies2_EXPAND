using MVZ2.GameContent.Buffs.Enemies;
using MVZ2.GameContent.Effects;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Level;
using PVZEngine.Buffs;
using PVZEngine.Definitions;
using PVZEngine.Level;

namespace MVZ2.GameContent.Stages
{
    [StageDefinition(VanillaStageNames.halloween11)]
    public partial class FrankensteinStage : StageDefinition
    {
        public FrankensteinStage(string nsp, string name) : base(nsp, name)
        {
            AddBehaviour(new WaveStageBehaviour(this));
            AddBehaviour(new FrankensteinStageBehaviour(this));
            AddBehaviour(new GemStageBehaviour(this));
            AddBehaviour(new StarshardStageBehaviour(this));
            AddBehaviour(new ConveyorStageBehaviour(this));

            this.SetClearSound(VanillaSoundID.finalItem);
        }
        public override void OnPostWave(LevelEngine level, int wave)
        {
            base.OnPostWave(level, wave);
            if (wave <= 10)
                return;
            if (!level.HasBuff<FrankensteinStageBuff>())
            {
                level.AddBuff<FrankensteinStageBuff>();
            }
            if (!level.EntityExists(VanillaEffectID.rain))
            {
                level.StartRain();
            }
        }
    }
}
