using MVZ2.GameContent.Effects;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Level;
using PVZEngine.Definitions;
using PVZEngine.Level;
using UnityEngine;

namespace MVZ2.GameContent.Stages
{
    [StageDefinition(VanillaStageNames.castle15)]
    public partial class Castle15Stage : StageDefinition
    {
        public Castle15Stage(string nsp, string name) : base(nsp, name)
        {
            AddBehaviour(new WaveStageBehaviour(this));
            AddBehaviour(new Castle15StageBehaviour(this));

            AddBehaviour(new GemStageBehaviour(this));
            AddBehaviour(new StarshardStageBehaviour(this));
            AddBehaviour(new ConveyorStageBehaviour(this));

            this.SetClearSound(VanillaSoundID.finalItem);
        }
        public override void OnPostWave(LevelEngine level, int wave)
        {
            base.OnPostWave(level, wave);
            if (wave <= 10 || wave >= level.GetTotalWaveCount())
                return;
            if (!level.EntityExists(VanillaEffectID.castleTwilight))
            {
                var pos = new Vector3((VanillaLevelExt.LEFT_BORDER + VanillaLevelExt.RIGHT_BORDER) * 0.5f, 0, VanillaLevelExt.LAWN_HEIGHT * 0.5f);
                level.Spawn(VanillaEffectID.castleTwilight, pos, null);
            }
        }
    }
}
