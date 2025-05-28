﻿using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Properties;
using PVZEngine.Buffs;
using PVZEngine.Level;
using PVZEngine.Modifiers;
using Tools;
using UnityEngine;

namespace MVZ2.GameContent.Buffs.Contraptions
{
    [BuffDefinition(VanillaBuffNames.noteBlockLoud)]
    public class NoteBlockLoudBuff : BuffDefinition
    {
        public NoteBlockLoudBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new FloatModifier(VanillaEntityProps.DAMAGE, NumberOperator.Multiply, 5));
            AddModifier(new Vector3Modifier(VanillaEntityProps.SHOT_VELOCITY, NumberOperator.Multiply, Vector3.one * 2f));
            AddModifier(new FloatModifier(VanillaEntityProps.ATTACK_SPEED, NumberOperator.Multiply, 5));
        }
        public override void PostAdd(Buff buff)
        {
            base.PostAdd(buff);
            buff.SetProperty(PROP_TIMER, new FrameTimer(MAX_TIMEOUT));
        }
        public override void PostUpdate(Buff buff)
        {
            base.PostUpdate(buff);
            var timer = buff.GetProperty<FrameTimer>(PROP_TIMER);
            if (timer == null || timer.Expired)
            {
                buff.Remove();
            }
            else
            {
                timer.Run();
            }
        }
        public const int MAX_TIMEOUT = 900;
        public static readonly VanillaBuffPropertyMeta<FrameTimer> PROP_TIMER = new VanillaBuffPropertyMeta<FrameTimer>("timer");
    }
}
