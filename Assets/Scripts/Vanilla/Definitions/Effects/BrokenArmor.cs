using MVZ2.Vanilla;
using PVZEngine.Definitions;
using PVZEngine.Level;
using UnityEngine;

namespace MVZ2.GameContent.Effects
{
    [Definition(VanillaEffectNames.brokenArmor)]
    public class BrokenArmor : VanillaEffect
    {
        #region 公有方法
        public BrokenArmor(string nsp, string name) : base(nsp, name)
        {
            SetProperty(EngineEntityProps.GRAVITY, 1);
            SetProperty(EngineEntityProps.FRICTION, 0.1f);
        }
        public override void Init(Entity entity)
        {
            base.Init(entity);
            entity.Timeout = 60;
        }
        public override void Update(Entity entity)
        {
            base.Update(entity);
            entity.SetTint(new Color(1, 1, 1, Mathf.Clamp01(entity.Timeout / 15f)));
            entity.SetAnimationInt("HealthState", 0);
            entity.RenderRotation += new Vector3(0, 0, -entity.Velocity.x);
        }
        public override void PostContactGround(Entity entity, Vector3 velocity)
        {
            base.PostContactGround(entity, velocity);
            var vel = entity.Velocity;
            vel.y *= -0.4f;
            entity.Velocity = vel;
        }
        #endregion
    }
}