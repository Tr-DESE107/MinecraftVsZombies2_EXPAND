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
        public const string NO_REWARD = "noReward";
        /// <summary>
        /// 是否无害。如果为true，则无法进屋，也不会被关卡快进计时器计数。
        /// </summary>
        public const string HARMLESS = "harmless";
        public const string DEATH_MESSAGE = "deathMessage";
        public const string IS_NEUTRALIZED = "isNeutralized";
        public const string EXCLUDED_AREA_TAGS = "excludedAreaTags";
        public static float GetSpeed(this Entity enemy, bool ignoreBuffs = false)
        {
            return enemy.GetProperty<float>(SPEED, ignoreBuffs: ignoreBuffs);
        }
        public static float GetMaxAttackHeight(this Entity enemy)
        {
            return enemy.GetProperty<float>(MAX_ATTACK_HEIGHT);
        }
        public static NamespaceID GetCrySound(this Entity enemy)
        {
            return enemy.GetProperty<NamespaceID>(CRY_SOUND);
        }
        public static bool HasNoReward(this Entity enemy)
        {
            return enemy.GetProperty<bool>(NO_REWARD);
        }
        public static void SetPreviewEnemy(this Entity enemy, bool value)
        {
            enemy.SetProperty(PREVIEW_ENEMY, value);
        }
        public static bool IsPreviewEnemy(this Entity enemy)
        {
            return enemy.GetProperty<bool>(PREVIEW_ENEMY);
        }
        public static void SetNeutralized(this Entity enemy, bool value)
        {
            enemy.SetProperty(IS_NEUTRALIZED, value);
        }
        public static bool IsNeutralized(this Entity enemy)
        {
            return enemy.GetProperty<bool>(IS_NEUTRALIZED);
        }
        public static NamespaceID[] GetExcludedAreaTags(this EntityDefinition definition)
        {
            return definition.GetProperty<NamespaceID[]>(EXCLUDED_AREA_TAGS);
        }
        #region 克制
        public const string ATTACKER_TAGS = "attackerTags";
        public static NamespaceID[] GetAttackerTags(this EntityDefinition enemy)
        {
            return enemy.GetProperty<NamespaceID[]>(ATTACKER_TAGS);
        }
        #endregion
        public const string IMMUNE_VORTEX = "immuneVortex";
        public static bool ImmuneVortex(this Entity enemy)
        {
            return enemy.GetProperty<bool>(IMMUNE_VORTEX);
        }
    }
}
