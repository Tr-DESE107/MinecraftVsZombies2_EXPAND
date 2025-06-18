using MVZ2.Vanilla.Properties;
using PVZEngine.Buffs;
using PVZEngine.Entities;
using PVZEngine.Level;
using PVZEngine.Modifiers;
using UnityEngine;

namespace MVZ2.GameContent.Buffs
{
    [BuffDefinition(VanillaBuffNames.whiteFlash)]
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
        public static Buff AddToEntity(Entity entity, int timeout)
        {
            var buff = entity.CreateBuff<WhiteFlashBuff>();
            buff.SetProperty(WhiteFlashBuff.PROP_TIMEOUT, timeout);
            buff.SetProperty(WhiteFlashBuff.PROP_MAX_TIMEOUT, timeout);
            entity.AddBuff(buff);
            return buff;
        }
        public static readonly VanillaBuffPropertyMeta<Color> PROP_COLOR = new VanillaBuffPropertyMeta<Color>("Color");
        public static readonly VanillaBuffPropertyMeta<int> PROP_TIMEOUT = new VanillaBuffPropertyMeta<int>("Timeout");
        public static readonly VanillaBuffPropertyMeta<int> PROP_MAX_TIMEOUT = new VanillaBuffPropertyMeta<int>("MaxTimeout");
    }
}
