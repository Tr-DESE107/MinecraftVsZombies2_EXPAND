using PVZEngine;
using PVZEngine.Entities;

namespace MVZ2.Vanilla.Entities
{
    public static class VanillaEnemyProps
    {
        public const string SPEED = "speed";
        public const string CAN_ARMOR = "canArmor";
        public const string MAX_ATTACK_HEIGHT = "maxAttackHeight";
        public const string PREVIEW_ENEMY = "previewEnemy";
        public const string CRY_SOUND = "crySound";
        /// <summary>
        /// 是否无害。如果为true，则无法进屋，也不会被关卡快进计时器计数。
        /// </summary>
        public const string HARMLESS = "harmless";
        public const string DEATH_MESSAGE = "deathMessage";
        public static float GetSpeed(this Entity enemy)
        {
            return enemy.GetProperty<float>(SPEED);
        }
        public static float GetMaxAttackHeight(this Entity enemy)
        {
            return enemy.GetProperty<float>(MAX_ATTACK_HEIGHT);
        }
        public static NamespaceID GetCrySound(this Entity enemy)
        {
            return enemy.GetProperty<NamespaceID>(CRY_SOUND);
        }
        public static void SetPreviewEnemy(this Entity enemy, bool value)
        {
            enemy.SetProperty(PREVIEW_ENEMY, value);
        }
        public static bool IsPreviewEnemy(this Entity enemy)
        {
            return enemy.GetProperty<bool>(PREVIEW_ENEMY);
        }
    }
}
