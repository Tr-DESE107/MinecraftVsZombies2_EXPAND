using PVZEngine;

namespace MVZ2.Vanilla
{
    public static class MVZ2Enemy
    {
        public static float GetSpeed(this Enemy enemy)
        {
            return enemy.GetProperty<float>(EnemyProps.SPEED);
        }
        public static float GetMaxAttackHeight(this Enemy enemy)
        {
            return enemy.GetProperty<float>(EnemyProps.MAX_ATTACK_HEIGHT);
        }
    }
}
