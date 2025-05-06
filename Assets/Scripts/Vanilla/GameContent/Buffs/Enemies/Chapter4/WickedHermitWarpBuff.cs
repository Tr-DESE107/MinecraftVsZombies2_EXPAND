using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Level;
using MVZ2.Vanilla.Properties;
using PVZEngine.Buffs;
using PVZEngine.Entities;
using PVZEngine.Level;
using PVZEngine.Modifiers;
using Tools;
using UnityEngine;
using UnityEngine.UIElements;

namespace MVZ2.GameContent.Buffs.Contraptions
{
    [BuffDefinition(VanillaBuffNames.wickedHermitWarp)]
    public class WickedHermitWarpBuff : BuffDefinition
    {
        public WickedHermitWarpBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new IntModifier(EngineEntityProps.COLLISION_DETECTION, NumberOperator.Set, EntityCollisionHelper.DETECTION_IGNORE));
        }
        public override void PostAdd(Buff buff)
        {
            base.PostAdd(buff);
            buff.SetProperty(PROP_TIMER, new FrameTimer(MAX_TIME));
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
                if (timer.PassedFrame(WARP_TIME))
                {
                    entity.Stun(GetStunDuration(entity));
                    var position = entity.Position;
                    var column = 0;
                    if (!entity.IsFacingLeft())
                    {
                        column = entity.Level.GetMaxColumnCount() - 1;
                    }
                    position.x = entity.Level.GetEntityColumnX(column);
                    entity.Position = position;
                    entity.AddBuff<WickedHermitWarppedBuff>();
                }
                var scaleT = 1 - Mathf.Abs(timer.Frame - WARP_TIME) / (float)WARP_TIME;
                entity.SetAnimationFloat("WarpBlend", scaleT);
            }
            if (timer.Expired)
            {
                buff.Remove();
            }
        }
        public override void PostRemove(Buff buff)
        {
            base.PostRemove(buff);
            var entity = buff.GetEntity();
            if (entity != null)
            {
                entity.SetAnimationFloat("WarpBlend", 0);
            }
        }
        public static int GetStunDuration(Entity entity)
        {
            var level = entity.Level;
            return STUN_TIME + level.GetEnemyAILevel() * STUN_TIME_PER_LEVEL;
        }
        public const int MAX_TIME = 20;
        public const int WARP_TIME = 10;
        public const int STUN_TIME = 150;
        public const int STUN_TIME_PER_LEVEL = 75;
        public static readonly VanillaBuffPropertyMeta PROP_TIMER = new VanillaBuffPropertyMeta("timer");
    }
}
