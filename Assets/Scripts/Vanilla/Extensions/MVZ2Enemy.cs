using MVZ2.GameContent.Enemies;
using PVZEngine;

namespace MVZ2.Vanilla
{
    public static class MVZ2Enemy
    {
        public static float GetSpeed(this Entity enemy)
        {
            return enemy.GetProperty<float>(EnemyProps.SPEED);
        }
        public static float GetMaxAttackHeight(this Entity enemy)
        {
            return enemy.GetProperty<float>(EnemyProps.MAX_ATTACK_HEIGHT);
        }
        public static bool IsPreviewEnemy(this Entity enemy)
        {
            return enemy.GetProperty<bool>(EnemyProps.PREVIEW_ENEMY);
        }
        public static void SetPreviewEnemy(this Entity enemy, bool value)
        {
            enemy.SetProperty(EnemyProps.PREVIEW_ENEMY, value);
        }
    }
}
