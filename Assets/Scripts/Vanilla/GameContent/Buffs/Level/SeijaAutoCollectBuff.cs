using MVZ2.GameContent.Bosses;
using MVZ2.GameContent.Effects;
using MVZ2.GameContent.ProgressBars;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Level;
using MVZ2.Vanilla.Properties;
using MVZ2Logic.Level;
using PVZEngine.Buffs;
using PVZEngine.Entities;
using PVZEngine.Level;
using PVZEngine.Modifiers;
using UnityEngine;

namespace MVZ2.GameContent.Buffs.Level
{
    [BuffDefinition(VanillaBuffNames.Level.seijaAutoCollect)]
    public class SeijaAutoCollectBuff : BuffDefinition
    {
        public SeijaAutoCollectBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new BooleanModifier(VanillaStageProps.AUTO_COLLECT, true));
            AddModifier(new BooleanModifier(VanillaStageProps.NO_ENERGY, true));
            AddModifier(new BooleanModifier(LogicLevelProps.PAUSE_DISABLED, true));
        }
    }
}
