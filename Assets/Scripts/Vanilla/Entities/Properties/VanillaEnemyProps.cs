using PVZEngine;
using PVZEngine.Entities;
using UnityEngine;

namespace MVZ2.Vanilla.Entities
{
    [PropertyRegistryRegion(PropertyRegions.entity)]
    public static class VanillaEnemyProps
    {
        public static readonly PropertyMeta<float> SPEED = new PropertyMeta<float>("speed");
        public static readonly PropertyMeta<bool> CAN_ARMOR = new PropertyMeta<bool>("canArmor");
        public static readonly PropertyMeta<float> MAX_ATTACK_HEIGHT = new PropertyMeta<float>("maxAttackHeight");
        public static readonly PropertyMeta<bool> PREVIEW_ENEMY = new PropertyMeta<bool>("previewEnemy");
        public static readonly PropertyMeta<NamespaceID> CRY_SOUND = new PropertyMeta<NamespaceID>("crySound");
        public static readonly PropertyMeta<bool> NO_REWARD = new PropertyMeta<bool>("noReward");
        public static readonly PropertyMeta<string> DEATH_MESSAGE = new PropertyMeta<string>("deathMessage");
        public static readonly PropertyMeta<bool> IS_NEUTRALIZED = new PropertyMeta<bool>("isNeutralized");
        public static readonly PropertyMeta<NamespaceID[]> EXCLUDED_AREA_TAGS = new PropertyMeta<NamespaceID[]>("excludedAreaTags");
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
        #region 初始盔甲
        public static readonly PropertyMeta<NamespaceID> STARTING_ARMOR = new PropertyMeta<NamespaceID>("starting_armor");
        public static NamespaceID GetStartingArmor(this EntityDefinition definition)
        {
            return definition.GetProperty<NamespaceID>(STARTING_ARMOR);
        }
        public static NamespaceID GetStartingArmor(this Entity enemy)
        {
            return enemy.GetProperty<NamespaceID>(STARTING_ARMOR);
        }
        #endregion

        #region 初始护盾
        public static readonly PropertyMeta<NamespaceID> STARTING_SHIELD = new PropertyMeta<NamespaceID>("starting_shield");
        public static NamespaceID GetStartingShield(this EntityDefinition definition)
        {
            return definition.GetProperty<NamespaceID>(STARTING_SHIELD);
        }
        public static NamespaceID GetStartingShield(this Entity enemy)
        {
            return enemy.GetProperty<NamespaceID>(STARTING_SHIELD);
        }
        #endregion

        #region 克制
        public static readonly PropertyMeta<NamespaceID[]> ATTACKER_TAGS = new PropertyMeta<NamespaceID[]>("attackerTags");
        public static NamespaceID[] GetAttackerTags(this EntityDefinition enemy)
        {
            return enemy.GetProperty<NamespaceID[]>(ATTACKER_TAGS);
        }
        #endregion

        #region 无害
        /// <summary>
        /// 无法进屋
        /// </summary>
        public static readonly PropertyMeta<bool> HARMLESS = new PropertyMeta<bool>("harmless");
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
        public static readonly PropertyMeta<bool> NOT_ACTIVE_ENEMY = new PropertyMeta<bool>("notActiveEnemy");
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
        public static readonly PropertyMeta<Vector3> PASSENGER_OFFSET = new PropertyMeta<Vector3>("passengerOffset");
        public static Vector3 GetPassengerOffset(this Entity enemy)
        {
            return enemy.GetProperty<Vector3>(PASSENGER_OFFSET);
        }
        #endregion

        public static readonly PropertyMeta<bool> IMMUNE_VORTEX = new PropertyMeta<bool>("immuneVortex");
        public static bool ImmuneVortex(this Entity enemy)
        {
            return enemy.GetProperty<bool>(IMMUNE_VORTEX);
        }
    }
}

