﻿using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Properties;
using PVZEngine;
using PVZEngine.Buffs;
using PVZEngine.Callbacks;
using PVZEngine.Entities;
using PVZEngine.Level;
using PVZEngine.Modifiers;
using UnityEngine;

namespace MVZ2.GameContent.Buffs.Effects
{
    [BuffDefinition(VanillaBuffNames.Effect.waterStainFrozen)]
    public class WaterStainFrozenBuff : BuffDefinition
    {
        public WaterStainFrozenBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new ColorModifier(EngineEntityProps.TINT, BlendOperator.One, BlendOperator.OneMinusSrcColor, new Color(0.75f, 0.75f, 0.75f, 1)));
        }
        public override void PostUpdate(Buff buff)
        {
            base.PostUpdate(buff);
            var entity = buff.GetEntity();
            if (entity != null)
            {
                entity.Timeout = entity.GetMaxTimeout();
            }

            var timeout = GetTimeout(buff);
            timeout--;
            SetTimeout(buff, timeout);
            if (timeout <= 0)
            {
                buff.Remove();
            }
        }
        public static int GetTimeout(Buff buff) => buff.GetProperty<int>(PROP_TIMEOUT);
        public static void SetTimeout(Buff buff, int value) => buff.SetProperty<int>(PROP_TIMEOUT, value);
        public static void ResetTimeout(Buff buff)
        {
            SetTimeout(buff, Ticks.FromSeconds(MAX_TIMEOUT_SECONDS));
        }
        public const float MAX_TIMEOUT_SECONDS = 40f;
        public static readonly PropertyMeta<int> PROP_TIMEOUT = new VanillaBuffPropertyMeta<int>("timeout");
    }
}
