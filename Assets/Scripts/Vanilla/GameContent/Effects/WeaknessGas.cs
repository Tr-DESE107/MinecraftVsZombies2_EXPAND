using MVZ2.Vanilla;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Entities;
using MVZ2Logic.Level;
using PVZEngine.Entities;

namespace MVZ2.GameContent.Effects
{
    [Definition(VanillaEffectNames.weaknessGas)]
    public class WeaknessGas : EffectBehaviour
    {

        #region 公有方法
        public WeaknessGas(string nsp, string name) : base(nsp, name)
        {
        }
        public override void Init(Entity entity)
        {
            base.Init(entity);
            entity.CollisionMaskHostile |=
                EntityCollisionHelper.MASK_PLANT |
                EntityCollisionHelper.MASK_ENEMY |
                EntityCollisionHelper.MASK_BOSS |
                EntityCollisionHelper.MASK_OBSTACLE |
                EntityCollisionHelper.MASK_PROJECTILE |
                EntityCollisionHelper.MASK_EFFECT;
            entity.Level.AddLoopSoundEntity(VanillaSoundID.poisonGas, entity.ID);
        }
        public override void Update(Entity entity)
        {
            base.Update(entity);
            entity.SetModelProperty("Size", entity.GetScaledSize());
            entity.SetModelProperty("Stopped", entity.Timeout <= DISAPPEAR_TIMEOUT);
        }
        public override void PostCollision(EntityCollision collision, int state)
        {
            base.PostCollision(collision, state);
            var entity = collision.Entity;
            var other = collision.Other;
            if (state == EntityCollisionHelper.STATE_EXIT)
                return;
            if (entity.Timeout <= DISAPPEAR_TIMEOUT)
                return;
            if (other.IsDead)
                return;
            if (other.Type != EntityTypes.ENEMY)
                return;
            other.InflictWeakness(150);
        }
        #endregion

        public const int DISAPPEAR_TIMEOUT = 30;
    }
}