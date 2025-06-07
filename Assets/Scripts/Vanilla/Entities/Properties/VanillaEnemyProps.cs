﻿using MVZ2.Vanilla.Properties;
using PVZEngine;
using PVZEngine.Entities;
using UnityEngine;

namespace MVZ2.Vanilla.Entities
{
    [PropertyRegistryRegion(PropertyRegions.entity)]
    public static class VanillaEnemyProps
    {
        private static PropertyMeta<T> Get<T>(string name, T defaultValue = default, params string[] obsoleteNames)
        {
            return new VanillaEntityPropertyMeta<T>(name, defaultValue, obsoleteNames);
        }
        public static readonly PropertyMeta<float> SPEED = Get<float>("speed");
        public static readonly PropertyMeta<bool> CAN_ARMOR = Get<bool>("canArmor");
        public static readonly PropertyMeta<float> MAX_ATTACK_HEIGHT = Get<float>("maxAttackHeight");
        public static readonly PropertyMeta<bool> PREVIEW_ENEMY = Get<bool>("previewEnemy");
        public static readonly PropertyMeta<NamespaceID> CRY_SOUND = Get<NamespaceID>("crySound");
        public static readonly PropertyMeta<bool> NO_REWARD = Get<bool>("noReward");
        public static readonly PropertyMeta<string> DEATH_MESSAGE = Get<string>("deathMessage");
        public static readonly PropertyMeta<bool> IS_NEUTRALIZED = Get<bool>("isNeutralized");
        public static readonly PropertyMeta<NamespaceID[]> EXCLUDED_AREA_TAGS = Get<NamespaceID[]>("excludedAreaTags");
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
        public static readonly PropertyMeta<NamespaceID> STARTING_ARMOR = Get<NamespaceID>("starting_armor");
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
        public static readonly PropertyMeta<NamespaceID> STARTING_SHIELD = Get<NamespaceID>("starting_shield");
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
        public static readonly PropertyMeta<NamespaceID[]> COUNTER_TAGS = Get<NamespaceID[]>("counter_tags", obsoleteNames: "attackerTags");
        public static NamespaceID[] GetCounterTags(this EntityDefinition enemy)
        {
            return enemy.GetProperty<NamespaceID[]>(COUNTER_TAGS);
        }
        #endregion

        #region 低矮
        public static readonly PropertyMeta<bool> LOW_ENEMY = Get<bool>("low_enemy");
        public static bool IsLowEnemy(this EntityDefinition enemy)
        {
            return enemy.GetProperty<bool>(LOW_ENEMY);
        }
        #endregion

        #region 飞行
        public static readonly PropertyMeta<bool> FLYING_ENEMY = Get<bool>("flying_enemy");
        public static bool IsFlyingEnemy(this EntityDefinition enemy)
        {
            return enemy.GetProperty<bool>(FLYING_ENEMY);
        }
        #endregion

        #region 无害
        /// <summary>
        /// 无法进屋
        /// </summary>
        public static readonly PropertyMeta<bool> HARMLESS = Get<bool>("harmless");
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
        public static readonly PropertyMeta<bool> NOT_ACTIVE_ENEMY = Get<bool>("notActiveEnemy");
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
        public static readonly PropertyMeta<Vector3> PASSENGER_OFFSET = Get<Vector3>("passengerOffset");
        public static Vector3 GetPassengerOffset(this Entity enemy)
        {
            return enemy.GetProperty<Vector3>(PASSENGER_OFFSET);
        }
        #endregion

        #region 假定存活

        public static readonly PropertyMeta<bool> ASSUME_ALIVE = Get<bool>("assume_alive");
        public static bool AssumeAlive(this Entity enemy)
        {
            return enemy.GetProperty<bool>(ASSUME_ALIVE);
        }
        #endregion

        public static readonly PropertyMeta<bool> IMMUNE_VORTEX = Get<bool>("immuneVortex");
        public static bool ImmuneVortex(this Entity enemy)
        {
            return enemy.GetProperty<bool>(IMMUNE_VORTEX);
        }
    }
}

