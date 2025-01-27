using PVZEngine;
using PVZEngine.Entities;
using UnityEngine;

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

        #region 无害
        /// <summary>
        /// 无法进屋
        /// </summary>
        public const string HARMLESS = "harmless";
        public static bool IsHarmless(this Entity enemy)
        {
            return enemy.GetProperty<bool>(HARMLESS);
        }
        public static bool IsHarmless(this EntityDefinition enemy)
        {
            return enemy.GetProperty<bool>(HARMLESS);
        }
        #endregion

        #region 有效敌人
        public const string NOT_ACTIVE_ENEMY = "notActiveEnemy";
        public static bool IsNotActiveEnemy(this Entity enemy)
        {
            return enemy.GetProperty<bool>(NOT_ACTIVE_ENEMY);
        }
        public static bool IsNotActiveEnemy(this EntityDefinition enemy)
        {
            return enemy.GetProperty<bool>(NOT_ACTIVE_ENEMY);
        }
        #endregion

        #region 乘客位置
        public const string PASSENGER_OFFSET = "passengerOffset";
        public static Vector3 GetPassengerOffset(this Entity enemy)
        {
            return enemy.GetProperty<Vector3>(PASSENGER_OFFSET);
        }
        #endregion

        public const string IMMUNE_VORTEX = "immuneVortex";
        public static bool ImmuneVortex(this Entity enemy)
        {
            return enemy.GetProperty<bool>(IMMUNE_VORTEX);
        }
    }
}
