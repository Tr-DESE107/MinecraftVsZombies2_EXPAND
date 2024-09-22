using MVZ2.GameContent.Enemies;
using PVZEngine;
using PVZEngine.Level;

namespace MVZ2.Vanilla
{
    public static class BuiltinEnemy
    {
        public static float GetSpeed(this Entity enemy)
        {
            return enemy.GetProperty<float>(BuiltinEnemyProps.SPEED);
        }
        public static float GetMaxAttackHeight(this Entity enemy)
        {
            return enemy.GetProperty<float>(BuiltinEnemyProps.MAX_ATTACK_HEIGHT);
        }
        public static NamespaceID GetCrySound(this Entity enemy)
        {
            return enemy.GetProperty<NamespaceID>(BuiltinEnemyProps.CRY_SOUND);
        }
        public static bool IsPreviewEnemy(this Entity enemy)
        {
            return enemy.GetProperty<bool>(BuiltinEnemyProps.PREVIEW_ENEMY);
        }
        public static void SetPreviewEnemy(this Entity enemy, bool value)
        {
            enemy.SetProperty(BuiltinEnemyProps.PREVIEW_ENEMY, value);
        }
    }
}
