using MVZ2.GameContent.Damages;
using MVZ2.Vanilla.Callbacks;
using MVZ2.Vanilla.Contraptions;
using MVZ2.Vanilla.Detections;
using MVZ2.Vanilla.Entities;
using PVZEngine.Damages;
using PVZEngine.Entities;
using PVZEngine.Triggers;

namespace MVZ2.Vanilla.Enemies
{
    public abstract class MeleeEnemy : StateEnemy
    {
        protected MeleeEnemy(string nsp, string name) : base(nsp, name)
        {
        }

        protected override void UpdateAI(Entity enemy)
        {
            if (!ValidateMeleeTarget(enemy, enemy.Target))
                enemy.Target = null;
            base.UpdateAI(enemy);
        }
        public override void PostCollision(EntityCollision collision, int state)
        {
            if (!collision.Collider.IsForMain())
                return;
            if (!collision.OtherCollider.IsForMain())
                return;
            if (state != EntityCollisionHelper.STATE_EXIT)
            {
                MeleeCollision(collision.Entity, collision.Other);
            }
            else
            {
                CancelMeleeAttack(collision.Entity, collision.Other);
            }
        }
        protected void MeleeCollision(Entity enemy, Entity other)
        {
            if (ValidateMeleeTarget(enemy, enemy.Target))
                return;
            var target = other;
            var protector = target.GetProtector();
            if (protector != null && protector.Exists() && !protector.IsFriendly(enemy))
            {
                target = protector;
            }
            if (ValidateMeleeTarget(enemy, target))
            {
                enemy.Target = other;
            }
        }
        protected virtual bool ValidateMeleeTarget(Entity enemy, Entity target)
        {
            if (target == null || !target.Exists() || target.IsDead)
                return false;
            if (!enemy.IsHostile(target))
                return false;
            if (!Detection.IsInSameRow(enemy, target))
                return false;
            if (!Detection.CanDetect(target))
                return false;
            if (target.Position.y > enemy.Position.y + enemy.GetMaxAttackHeight())
                return false;
            if (target.Type == EntityTypes.PLANT || target.Type == EntityTypes.OBSTACLE)
            {
                if (target.IsFloor())
                    return false;
                var protector = target.GetProtector();
                if (protector != null && protector.Exists() && !protector.IsFriendly(enemy))
                    return false;
            }
            return true;
        }
        protected void CancelMeleeAttack(Entity enemy, Entity other)
        {
            if (enemy.Target == other)
            {
                enemy.Target = null;
            }
        }
        protected override void UpdateStateAttack(Entity enemy)
        {
            MeleeAttack(enemy, enemy.Target);
        }
        protected void MeleeAttack(Entity enemy, Entity target)
        {
            if (target == null)
                return;
            var damage = enemy.GetDamage() * enemy.GetAttackSpeed() / 30f;
            target.TakeDamage(damage, new DamageEffectList(VanillaDamageEffects.MUTE, VanillaDamageEffects.ENEMY_MELEE), enemy);
            enemy.Level.Triggers.RunCallback(VanillaLevelCallbacks.POST_ENEMY_MELEE_ATTACK, c => c(enemy, target, damage));
        }
    }

}