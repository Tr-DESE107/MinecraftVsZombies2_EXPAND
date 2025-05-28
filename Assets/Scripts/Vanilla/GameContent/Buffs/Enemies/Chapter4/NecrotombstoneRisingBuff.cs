﻿using MVZ2.Vanilla.Properties;
using PVZEngine.Buffs;
using PVZEngine.Entities;
using PVZEngine.Level;
using PVZEngine.Modifiers;
using Tools;
using UnityEngine;

namespace MVZ2.GameContent.Buffs.Contraptions
{
    [BuffDefinition(VanillaBuffNames.necrotombstoneRising)]
    public class NecrotombstoneRisingBuff : BuffDefinition
    {
        public NecrotombstoneRisingBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new FloatModifier(EngineEntityProps.GROUND_LIMIT_OFFSET, NumberOperator.Add, PROP_GROUND_LIMIT_OFFSET));
        }
        public override void PostAdd(Buff buff)
        {
            base.PostAdd(buff);
            buff.SetProperty(PROP_GROUND_LIMIT_OFFSET, -100f);
            buff.SetProperty(PROP_TIMER, new FrameTimer(MAX_TIME));
            var entity = buff.GetEntity();
            if (entity != null)
            {
                entity.SetAnimationBool("Rising", true);
            }
        }
        public override void PostUpdate(Buff buff)
        {
            base.PostUpdate(buff);
            var timer = buff.GetProperty<FrameTimer>(PROP_TIMER);
            if (timer == null || timer.Expired)
            {
                buff.Remove();
                return;
            }

            timer.Run();

            var entity = buff.GetEntity();
            if (entity != null)
            {
                if (entity.IsDead)
                {
                    buff.Remove();
                    return;
                }
                buff.SetProperty(PROP_GROUND_LIMIT_OFFSET, Mathf.Lerp(-100, 0, timer.GetPassedPercentage()));
                entity.SetAnimationBool("Rising", true);
            }
        }
        public override void PostRemove(Buff buff)
        {
            base.PostRemove(buff);
            var entity = buff.GetEntity();
            if (entity != null)
            {
                entity.SetAnimationBool("Rising", false);
            }
        }
        public const int MAX_TIME = 30;
        public static readonly VanillaBuffPropertyMeta<float> PROP_GROUND_LIMIT_OFFSET = new VanillaBuffPropertyMeta<float>("groundLimitOffset");
        public static readonly VanillaBuffPropertyMeta<FrameTimer> PROP_TIMER = new VanillaBuffPropertyMeta<FrameTimer>("timer");
    }
}
