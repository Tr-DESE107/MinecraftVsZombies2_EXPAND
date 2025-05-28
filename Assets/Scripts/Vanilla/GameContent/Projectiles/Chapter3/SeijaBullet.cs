﻿using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Properties;
using PVZEngine.Entities;
using PVZEngine.Level;
using Tools;

namespace MVZ2.GameContent.Projectiles
{
    [EntityBehaviourDefinition(VanillaProjectileNames.seijaBullet)]
    public class SeijaBullet : ProjectileBehaviour
    {
        public SeijaBullet(string nsp, string name) : base(nsp, name)
        {
        }
        public override void Init(Entity entity)
        {
            base.Init(entity);
            SetStateChangeTimer(entity, new FrameTimer(15));
        }
        public override void Update(Entity projectile)
        {
            base.Update(projectile);
            var timer = GetStateChangeTimer(projectile);
            if (projectile.State < 2)
            {
                timer.Run();
                if (timer.Expired)
                {
                    timer.Reset();
                    projectile.State++;
                    SetDark(projectile, !IsDark(projectile));
                }
            }


            var vel = projectile.Velocity;
            var targetSpeed = IsDark(projectile) ? DARK_SPEED : LIGHT_SPEED;
            var magnitude = vel.magnitude;
            magnitude = magnitude * 0.9f + targetSpeed * 0.1f;
            vel = vel.normalized * magnitude;
            projectile.Velocity = vel;
        }
        public static void SetDark(Entity bullet, bool value)
        {
            bullet.SetProperty(PROP_DARK, value);
            bullet.SetModelProperty("Dark", value);
        }
        public static bool IsDark(Entity bullet) => bullet.GetProperty<bool>(PROP_DARK);
        public static void SetStateChangeTimer(Entity bullet, FrameTimer value) => bullet.SetProperty(PROP_STATE_CHANGE_TIMER, value);
        public static FrameTimer GetStateChangeTimer(Entity bullet) => bullet.GetProperty<FrameTimer>(PROP_STATE_CHANGE_TIMER);
        public static readonly VanillaEntityPropertyMeta<bool> PROP_DARK = new VanillaEntityPropertyMeta<bool>("Dark");
        public static readonly VanillaEntityPropertyMeta<FrameTimer> PROP_STATE_CHANGE_TIMER = new VanillaEntityPropertyMeta<FrameTimer>("StateChangeTimer");
        public const float LIGHT_SPEED = 3;
        public const float DARK_SPEED = 10;
    }
}
