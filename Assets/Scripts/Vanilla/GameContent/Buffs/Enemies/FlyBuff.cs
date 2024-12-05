using MVZ2.Vanilla;
using MVZ2.Vanilla.Entities;
using PVZEngine.Buffs;
using PVZEngine.Entities;
using PVZEngine.Modifiers;
using UnityEngine;

namespace MVZ2.GameContent.Buffs.Enemies
{
    [Definition(VanillaBuffNames.fly)]
    public class FlyBuff : BuffDefinition
    {
        public FlyBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new FloatModifier(EngineEntityProps.GRAVITY, NumberOperator.Multiply, PROP_GRAVITY_MULTIPLIER));
        }
        public override void PostAdd(Buff buff)
        {
            base.PostAdd(buff);
            buff.SetProperty(PROP_GRAVITY_MULTIPLIER, 0);
            buff.SetProperty(PROP_TARGET_HEIGHT, 0);
            buff.SetProperty(PROP_FLY_SPEED, 0.1f);
            buff.SetProperty(PROP_FLY_SPEED_FACTOR, 0.2f);
            buff.SetProperty(PROP_MAX_FLY_SPEED, 10);
        }
        public override void PostUpdate(Buff buff)
        {
            base.PostUpdate(buff);
            var entity = buff.GetEntity();
            if (entity == null)
                return;
            if (entity.IsAIFrozen())
            {
                buff.SetProperty(PROP_GRAVITY_MULTIPLIER, 1);
                return;
            }
            buff.SetProperty(PROP_GRAVITY_MULTIPLIER, 0);
            var targetHeight = entity.Level.KillerEnemy == entity ? 0 : buff.GetProperty<float>(PROP_TARGET_HEIGHT);
            var flySpeed = buff.GetProperty<float>(PROP_FLY_SPEED);
            var flySpeedFactor = buff.GetProperty<float>(PROP_FLY_SPEED_FACTOR);
            var maxFlySpeed = buff.GetProperty<float>(PROP_MAX_FLY_SPEED);
            var currentHeight = entity.GetRelativeY();
            var heightToMove = targetHeight - currentHeight;
            var targetSpeed = Mathf.Clamp(heightToMove * flySpeed, -maxFlySpeed, maxFlySpeed);
            var velocity = entity.Velocity;
            velocity.y = velocity.y * (1 - flySpeedFactor) + targetSpeed * flySpeedFactor;
            entity.Velocity = velocity;
        }
        public const string PROP_GRAVITY_MULTIPLIER = "GravityMultiplier";
        public const string PROP_TARGET_HEIGHT = "TargetHeight";
        public const string PROP_MAX_FLY_SPEED = "MaxFlySpeed";
        public const string PROP_FLY_SPEED = "FlySpeed";
        public const string PROP_FLY_SPEED_FACTOR = "FlySpeedFactor";
    }
}
