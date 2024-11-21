using MVZ2.GameContent.Contraptions;
using MVZ2.Vanilla;
using MVZ2Logic.Entities;
using PVZEngine.Damages;
using PVZEngine.Entities;

namespace MVZ2.GameContent.Enemies
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
        public override void PostCollision(Entity enemy, Entity other, int state)
        {
            if (state != EntityCollision.STATE_EXIT)
            {
                MeleeCollision(enemy, other);
            }
            else
            {
                CancelMeleeAttack(enemy, other);
            }
        }
        protected void MeleeCollision(Entity enemy, Entity other)
        {
            if (ValidateMeleeTarget(enemy, enemy.Target))
                return;
            if (ValidateMeleeTarget(enemy, other))
            {
                enemy.Target = other;
            }
        }
        protected virtual bool ValidateMeleeTarget(Entity enemy, Entity target)
        {
            if (target == null || !target.Exists() || target.IsDead)
                return false;
            if (!enemy.IsEnemy(target))
                return false;
            if (!Detection.IsInSameRow(enemy, target))
                return false;
            if (!Detection.CanDetect(target))
                return false;
            if (target.Type == EntityTypes.PLANT && target.IsFloor())
                return false;
            if (target.Position.y > enemy.Position.y + enemy.GetMaxAttackHeight())
                return false;
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
            target.TakeDamage(enemy.GetDamage() * enemy.GetAttackSpeed() / 30f, new DamageEffectList(VanillaDamageEffects.MUTE), new EntityReferenceChain(enemy));
        }
    }

}