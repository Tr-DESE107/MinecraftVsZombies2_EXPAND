using MVZ2.GameContent.Bosses;
using MVZ2.GameContent.Buffs.Level;
using MVZ2.GameContent.Effects;
using MVZ2.GameContent.Pickups;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Level;
using MVZ2Logic;
using MVZ2Logic.Level;
using PVZEngine.Definitions;
using PVZEngine.Entities;
using PVZEngine.Level;
using UnityEngine;

namespace MVZ2.GameContent.Stages
{
    public class WitherStageBehaviour : BossStageBehaviour
    {
        public WitherStageBehaviour(StageDefinition stageDef) : base(stageDef)
        {
        }
        protected override void StartAfterFinalWave(LevelEngine level)
        {
            base.StartAfterFinalWave(level);
            level.ShowAdvice(VanillaStrings.CONTEXT_ADVICE, VanillaStrings.ADVICE_DEVELOPING, 1000, 150);
        }
    }
}
