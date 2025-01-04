using MVZ2.Vanilla;
using PVZEngine.Buffs;
using PVZEngine.Entities;
using PVZEngine.Modifiers;
using UnityEngine;

namespace MVZ2.GameContent.Buffs
{
    [Definition(VanillaBuffNames.whiteFlash)]
    public class WhiteFlashBuff : BuffDefinition
    {
        public WhiteFlashBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new ColorModifier(EngineEntityProps.COLOR_OFFSET, PROP_COLOR));
        }
        public override void PostUpdate(Buff buff)
        {
            base.PostUpdate(buff);
            var timeout = buff.GetProperty<int>(PROP_TIMEOUT);
            timeout--;
            buff.SetProperty(PROP_TIMEOUT, timeout);

            float alpha = 0;
            var maxTimeout = buff.GetProperty<int>(PROP_MAX_TIMEOUT);
            if (maxTimeout > 0)
            {
                alpha = timeout / (float)maxTimeout;
            }
            buff.SetProperty(PROP_COLOR, new Color(1, 1, 1, alpha));

            if (timeout <= 0)
            {
                buff.Remove();
            }
        }
        public const string PROP_COLOR = "Color";
        public const string PROP_TIMEOUT = "Timeout";
        public const string PROP_MAX_TIMEOUT = "MaxTimeout";
    }
}
