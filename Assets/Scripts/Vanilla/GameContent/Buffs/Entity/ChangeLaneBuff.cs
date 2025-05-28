using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Properties;
using PVZEngine.Buffs;
using PVZEngine.Level;
using UnityEngine;

namespace MVZ2.GameContent.Buffs.Carts
{
    [BuffDefinition(VanillaBuffNames.changeLane)]
    public class ChangeLaneBuff : BuffDefinition
    {
        public ChangeLaneBuff(string nsp, string name) : base(nsp, name)
        {
        }
        public override void PostUpdate(Buff buff)
        {
            base.PostUpdate(buff);
            var entity = buff.GetEntity();
            if (entity == null)
                return;
            var targetLane = GetTarget(buff);
            if (targetLane < 0 || targetLane >= entity.Level.GetMaxLaneCount())
                return;
            var sourceLane = GetSource(buff);

            float targetZ = entity.Level.GetEntityLaneZ(targetLane);
            var pos = entity.Position;
            var velocity = entity.Velocity;
            bool passed;
            if (sourceLane > targetLane) // Warp upwards.
            {
                passed = pos.z >= targetZ;
            }
            else // Warp downwards.
            {
                passed = pos.z <= targetZ;
            }

            if (!passed)
            {
                float warpSpeed = entity.GetChangeLaneSpeed();

                // Warp upwards.
                if (sourceLane > targetLane)
                {
                    velocity.z = Mathf.Max(warpSpeed, entity.Velocity.z);
                }
                // Warp downwards.
                else
                {
                    velocity.z = Mathf.Min(-warpSpeed, entity.Velocity.z);
                }
            }
            else
            {
                pos.z = targetZ;
                velocity.z = 0;
                Stop(buff);
            }
            entity.Position = pos;
            entity.Velocity = velocity;
        }
        public static void Start(Buff buff, int target, int source)
        {
            var level = buff.Level;
            SetTarget(buff, target);
            SetSource(buff, source);
        }
        public static void Stop(Buff buff)
        {
            buff.Remove();
        }
        public static int GetTarget(Buff entity) => entity.GetProperty<int>(PROP_TARGET);
        public static void SetTarget(Buff entity, int value) => entity.SetProperty(PROP_TARGET, value);
        public static int GetSource(Buff entity) => entity.GetProperty<int>(PROP_SOURCE);
        public static void SetSource(Buff entity, int value) => entity.SetProperty(PROP_SOURCE, value);
        public static readonly VanillaBuffPropertyMeta<int> PROP_TARGET = new VanillaBuffPropertyMeta<int>("target");
        public static readonly VanillaBuffPropertyMeta<int> PROP_SOURCE = new VanillaBuffPropertyMeta<int>("source");
    }
}
