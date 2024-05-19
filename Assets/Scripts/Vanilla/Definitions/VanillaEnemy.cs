using System.Collections;
using System.Collections.Generic;
using PVZEngine;
using UnityEngine;

namespace MVZ2.Vanilla
{
    public abstract class VanillaEnemy : EntityDefinition
    {
        public override void PostEntityCollisionStay(Entity entity, Entity other, bool actively)
        {
            if (!actively)
                return;
            var enemy = entity.ToEnemy();
            MeleeCollision(enemy, other);
        }

        public override void PostEntityCollisionExit(Entity entity, Entity other, bool actively)
        {
            if (!actively)
                return;
            var enemy = entity.ToEnemy();
            CancelMeleeAttack(enemy, other);
        }
        protected void MeleeCollision(Enemy enemy, Entity other)
        {
            if (ValidateAttackTarget(enemy, enemy.AttackTarget))
                return;
            if (ValidateAttackTarget(enemy, other))
            {
                enemy.AttackTarget = other;
            }
        }

        protected void CancelMeleeAttack(Enemy enemy, Entity other)
        {
            if (enemy.AttackTarget == other)
            {
                enemy.AttackTarget = null;
            }
        }
        protected abstract bool ValidateAttackTarget(Enemy enemy, Entity other);
        public override int Type => EntityTypes.ENEMY;
    }

}