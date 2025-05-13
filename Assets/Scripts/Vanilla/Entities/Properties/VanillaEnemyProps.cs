using PVZEngine;
using PVZEngine.Entities;
using UnityEngine;

namespace MVZ2.Vanilla.Entities
{
    [PropertyRegistryRegion(PropertyRegions.entity)]
    public static class VanillaEnemyProps
    {
        public static readonly PropertyMeta SPEED = new PropertyMeta("speed");
        public static readonly PropertyMeta CAN_ARMOR = new PropertyMeta("canArmor");
        public static readonly PropertyMeta MAX_ATTACK_HEIGHT = new PropertyMeta("maxAttackHeight");
        public static readonly PropertyMeta PREVIEW_ENEMY = new PropertyMeta("previewEnemy");
        public static readonly PropertyMeta CRY_SOUND = new PropertyMeta("crySound");
        public static readonly PropertyMeta NO_REWARD = new PropertyMeta("noReward");
        public static readonly PropertyMeta DEATH_MESSAGE = new PropertyMeta("deathMessage");
        public static readonly PropertyMeta IS_NEUTRALIZED = new PropertyMeta("isNeutralized");
        public static readonly PropertyMeta EXCLUDED_AREA_TAGS = new PropertyMeta("excludedAreaTags");
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
        public static readonly PropertyMeta ATTACKER_TAGS = new PropertyMeta("attackerTags");
        public static NamespaceID[] GetAttackerTags(this EntityDefinition enemy)
        {
            return enemy.GetProperty<NamespaceID[]>(ATTACKER_TAGS);
        }
        #endregion

        #region 无害
        /// <summary>
        /// 无法进屋
        /// </summary>
        public static readonly PropertyMeta HARMLESS = new PropertyMeta("harmless");
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
        public static readonly PropertyMeta NOT_ACTIVE_ENEMY = new PropertyMeta("notActiveEnemy");
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
        public static readonly PropertyMeta PASSENGER_OFFSET = new PropertyMeta("passengerOffset");
        public static Vector3 GetPassengerOffset(this Entity enemy)
        {
            return enemy.GetProperty<Vector3>(PASSENGER_OFFSET);
        }
        #endregion

        public static readonly PropertyMeta IMMUNE_VORTEX = new PropertyMeta("immuneVortex");
        public static bool ImmuneVortex(this Entity enemy)
        {
            return enemy.GetProperty<bool>(IMMUNE_VORTEX);
        }
    }
}
