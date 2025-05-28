﻿using MVZ2.GameContent.Effects;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Properties;
using PVZEngine.Buffs;
using PVZEngine.Entities;
using PVZEngine.Level;
using PVZEngine.Modifiers;
using Tools;

namespace MVZ2.GameContent.Buffs.Enemies
{
    [BuffDefinition(VanillaBuffNames.stun)]
    public class StunBuff : BuffDefinition
    {
        public StunBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new BooleanModifier(VanillaEntityProps.AI_FROZEN, true));
        }
        public override void PostAdd(Buff buff)
        {
            base.PostAdd(buff);
            UpdateStunStars(buff);
            buff.SetProperty(StunBuff.PROP_TIMER, new FrameTimer(0));
        }
        public override void PostRemove(Buff buff)
        {
            base.PostRemove(buff);
            var starsID = GetStunStars(buff);
            var stars = starsID?.GetEntity(buff.Level);
            if (stars != null && stars.Exists())
            {
                stars.Remove();
            }
        }
        public override void PostUpdate(Buff buff)
        {
            base.PostUpdate(buff);
            UpdateStun(buff);
            UpdateStunStars(buff);
        }
        private void UpdateStun(Buff buff)
        {
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
                return;
            }
            var entity = buff.GetEntity();
            if (entity == null || !entity.Exists() || entity.IsDead)
            {
                buff.Remove();
                return;
            }
        }
        private void UpdateStunStars(Buff buff)
        {
            var entity = buff.GetEntity();
            if (entity == null)
                return;
            var starsID = GetStunStars(buff);
            var stars = starsID?.GetEntity(entity.Level);
            if (stars == null || !stars.Exists())
            {
                stars = buff.Level.Spawn(VanillaEffectID.stunStars, StunStars.GetPosition(entity), entity);
                stars.SetParent(entity);
                SetStunStars(buff, new EntityID(stars));
            }
        }
        public static void SetStunTime(Buff buff, int value)
        {
            var timer = buff.GetProperty<FrameTimer>(PROP_TIMER);
            if (timer == null)
                return;
            timer.ResetTime(value);
        }
        public static EntityID GetStunStars(Buff buff) => buff.GetProperty<EntityID>(PROP_STUN_STARS);
        public static void SetStunStars(Buff buff, EntityID value) => buff.SetProperty(PROP_STUN_STARS, value);
        public static readonly VanillaBuffPropertyMeta<FrameTimer> PROP_TIMER = new VanillaBuffPropertyMeta<FrameTimer>("Timer");
        public static readonly VanillaBuffPropertyMeta<EntityID> PROP_STUN_STARS = new VanillaBuffPropertyMeta<EntityID>("StunStars");
    }
}
