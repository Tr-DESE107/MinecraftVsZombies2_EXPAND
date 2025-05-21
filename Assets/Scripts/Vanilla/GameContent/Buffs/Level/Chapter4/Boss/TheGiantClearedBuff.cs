using MVZ2.GameContent.Areas;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Level;
using MVZ2.Vanilla.Properties;
using MVZ2Logic;
using MVZ2Logic.Level;
using PVZEngine.Buffs;
using PVZEngine.Level;
using PVZEngine.Modifiers;
using UnityEngine;

namespace MVZ2.GameContent.Buffs.Level
{
    [BuffDefinition(VanillaBuffNames.Level.theGiantCleared)]
    public class TheGiantClearedBuff : BuffDefinition
    {
        public TheGiantClearedBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new BooleanModifier(LogicLevelProps.PAUSE_DISABLED, true));
            AddModifier(new BooleanModifier(VanillaStageProps.AUTO_COLLECT_ALL, true));
        }
        public override void PostAdd(Buff buff)
        {
            base.PostAdd(buff);
            buff.SetProperty(PROP_TIMEOUT, MAX_TIMEOUT);
        }
        public override void PostUpdate(Buff buff)
        {
            base.PostUpdate(buff);
            var timeout = buff.GetProperty<int>(PROP_TIMEOUT);
            timeout--;
            buff.SetProperty(PROP_TIMEOUT, timeout);

            var level = buff.Level;
            if (timeout <= 0)
            {
                level.Clear();
                buff.Remove();
            }
        }
        public static readonly VanillaBuffPropertyMeta PROP_TIMEOUT = new VanillaBuffPropertyMeta("Timeout");
        public const int MAX_TIMEOUT = 150;
    }
}
