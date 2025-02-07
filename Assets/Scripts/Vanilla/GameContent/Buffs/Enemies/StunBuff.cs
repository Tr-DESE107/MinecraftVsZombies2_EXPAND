using MVZ2.GameContent.Effects;
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
        }
        public override void PostRemove(Buff buff)
        {
            base.PostRemove(buff);
            var stars = buff.GetProperty<Entity>(PROP_STUN_STARS);
            if (stars != null)
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
            var stars = buff.GetProperty<Entity>(PROP_STUN_STARS);
            if (stars == null)
            {
                stars = buff.Level.Spawn(VanillaEffectID.stunStars, entity.Position, entity);
                stars.SetParent(entity);
                buff.SetProperty(PROP_STUN_STARS, stars);
            }
        }
        public static readonly VanillaBuffPropertyMeta PROP_TIMER = new VanillaBuffPropertyMeta("Timer");
        public static readonly VanillaBuffPropertyMeta PROP_STUN_STARS = new VanillaBuffPropertyMeta("StunStars");
    }
}
