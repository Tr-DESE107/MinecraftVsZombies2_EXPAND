using MVZ2.GameContent.Buffs.Enemies;
using MVZ2.GameContent.Effects;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Level;
using MVZ2Logic.Level;
using PVZEngine.Definitions;
using PVZEngine.Level;
using UnityEngine;

namespace MVZ2.GameContent.Stages
{
    [Definition(VanillaStageNames.dream11)]
    public partial class NightmareStage : StageDefinition
    {
        public NightmareStage(string nsp, string name) : base(nsp, name)
        {
            AddBehaviour(new NightmareStageBehaviour(this));
            AddBehaviour(new GemStageBehaviour(this));
            AddBehaviour(new StarshardStageBehaviour(this));
            AddBehaviour(new ConveyorStageBehaviour(this));
            this.SetClearSound(VanillaSoundID.finalItem);
        }
        public override void OnStart(LevelEngine level)
        {
            base.OnStart(level);
            level.SetConveyorMode(true);
        }
        public override void OnPostWave(LevelEngine level, int wave)
        {
            base.OnPostWave(level, wave);
            if (wave <= 10)
                return;
            if (!level.EntityExists(VanillaEffectID.nightmareWatchingEye))
            {
                var pos = new Vector3((VanillaLevelExt.LEFT_BORDER + VanillaLevelExt.RIGHT_BORDER) * 0.5f, 0, VanillaLevelExt.LAWN_HEIGHT * 0.5f);
                level.Spawn(VanillaEffectID.nightmareWatchingEye, pos, null);
            }
        }
    }
}
