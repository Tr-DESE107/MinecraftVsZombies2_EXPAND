using MVZ2.GameContent.Damages;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Enemies;
using MVZ2.Vanilla.Entities;
using MVZ2Logic;
using PVZEngine.Damages;
using PVZEngine.Entities;

namespace MVZ2.GameContent.Enemies
{
    [Definition(VanillaEnemyNames.zombie)]
    public class Zombie : MeleeEnemy
    {
        public Zombie(string nsp, string name) : base(nsp, name)
        {
        }
        protected override void UpdateLogic(Entity entity)
        {
            base.UpdateLogic(entity);
            entity.SetAnimationInt("HealthState", entity.GetHealthState(2));
        }
        public override void PostDeath(Entity entity, DamageInput info)
        {
            base.PostDeath(entity, info);
            if (info.Effects.HasEffect(VanillaDamageEffects.DROWN))
            {
                Global.Game.Unlock(VanillaUnlockID.rickrollDrown);
            }
        }
    }
}
