﻿using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Properties;
using PVZEngine.Buffs;
using PVZEngine.Entities;
using PVZEngine.Level;
using PVZEngine.Modifiers;
using Tools;
using UnityEngine;

namespace MVZ2.GameContent.Buffs.Enemies
{
    [BuffDefinition(VanillaBuffNames.slow)]
    public class SlowBuff : BuffDefinition
    {
        public SlowBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new ColorModifier(EngineEntityProps.TINT, new Color(0, 0, 1, 1f)));
            AddModifier(new FloatModifier(VanillaEnemyProps.SPEED, NumberOperator.Multiply, 0.5f));
            AddModifier(new FloatModifier(VanillaEntityProps.ATTACK_SPEED, NumberOperator.Multiply, 0.5f));
        }
        public override void PostAdd(Buff buff)
        {
            base.PostAdd(buff);
            buff.SetProperty(PROP_TIMER, new FrameTimer(300));
        }
        public override void PostUpdate(Buff buff)
        {
            base.PostUpdate(buff);
            var timer = buff.GetProperty<FrameTimer>(PROP_TIMER);
            if (timer == null)
            {
                buff.Remove();
                return;
            }
            timer.Run();
            if (timer.Expired)
            {
                buff.Remove();
            }
        }
        public static void SetTimeout(Buff buff, int value)
        {
            var timer = buff.GetProperty<FrameTimer>(PROP_TIMER);
            if (timer == null)
            {
                timer = new FrameTimer(value);
                buff.SetProperty(PROP_TIMER, timer);
            }
            timer.ResetTime(value);
        }

        public static readonly VanillaBuffPropertyMeta<FrameTimer> PROP_TIMER = new VanillaBuffPropertyMeta<FrameTimer>("Timer");
    }
}
