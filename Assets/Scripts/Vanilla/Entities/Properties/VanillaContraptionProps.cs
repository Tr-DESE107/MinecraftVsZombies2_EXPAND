﻿using MVZ2.Vanilla.Level;
using MVZ2.Vanilla.Properties;
using PVZEngine;
using PVZEngine.Entities;
using PVZEngine.Level;

namespace MVZ2.Vanilla.Contraptions
{
    [PropertyRegistryRegion(PropertyRegions.entity)]
    public static class VanillaContraptionProps
    {
        private static PropertyMeta<T> Get<T>(string name, T defaultValue = default, params string[] obsoleteNames)
        {
            return new VanillaEntityPropertyMeta<T>(name, defaultValue, obsoleteNames);
        }

        #region 夜用
        public static readonly PropertyMeta<bool> NOCTURNAL = Get<bool>("nocturnal");
        public static bool IsNocturnal(this EntityDefinition definition)
        {
            return definition.GetProperty<bool>(NOCTURNAL);
        }
        public static bool IsNocturnal(this Entity entity)
        {
            return entity.GetProperty<bool>(NOCTURNAL);
        }
        #endregion

        #region 可攀爬
        public static readonly PropertyMeta<bool> NO_CLIMB = Get<bool>("noClimb");
        public static bool NoClimb(this Entity contraption)
        {
            return contraption.GetProperty<bool>(NO_CLIMB);
        }
        #endregion

        #region 防御性
        public static readonly PropertyMeta<bool> DEFENSIVE = Get<bool>("defensive");
        public static bool IsDefensive(this Entity contraption)
        {
            return contraption.GetProperty<bool>(DEFENSIVE);
        }
        public static bool IsDefensive(this EntityDefinition contraptionDef)
        {
            return contraptionDef.GetProperty<bool>(DEFENSIVE);
        }
        #endregion

        #region 克制
        public static readonly PropertyMeta<NamespaceID[]> COUNTER_TAGS_FOR = Get<NamespaceID[]>("counter_for", obsoleteNames: "attackerFor");
        public static NamespaceID[] GetCounterTagsFor(this EntityDefinition contraption)
        {
            return contraption.GetProperty<NamespaceID[]>(COUNTER_TAGS_FOR);
        }
        #endregion

        #region 生产者
        public static readonly PropertyMeta<bool> PRODUCER = Get<bool>("producer");
        public static bool IsProducer(this EntityDefinition contraption)
        {
            return contraption.GetProperty<bool>(PRODUCER);
        }
        #endregion

        #region 激发
        public static readonly PropertyMeta<bool> PROP_EVOKED = Get<bool>("evoked");
        public static bool IsEvoked(this Entity contraption)
        {
            return contraption.GetProperty<bool>(PROP_EVOKED);
        }
        public static void SetEvoked(this Entity contraption, bool value)
        {
            contraption.SetProperty(PROP_EVOKED, value);
        }
        #endregion

        #region 阻挡跳跃
        public static readonly PropertyMeta<bool> BLOCKS_JUMP = Get<bool>("blocksJump");
        public static bool BlocksJump(this Entity contraption)
        {
            return contraption.GetProperty<bool>(BLOCKS_JUMP);
        }
        public static bool BlocksJump(this EntityDefinition definition)
        {
            return definition.GetProperty<bool>(BLOCKS_JUMP);
        }
        #endregion

        #region 无法挖掘
        public static readonly PropertyMeta<bool> CANNOT_DIG = Get<bool>("cannotDig");
        public static bool CannotDig(this Entity contraption)
        {
            return contraption.GetProperty<bool>(CANNOT_DIG);
        }
        #endregion

        #region 升级蓝图
        public static readonly PropertyMeta<bool> UPGRADE_BLUEPRINT = Get<bool>("upgradeBlueprint");
        public static bool IsUpgradeBlueprint(this EntityDefinition definition)
        {
            return definition.GetProperty<bool>(UPGRADE_BLUEPRINT);
        }
        public static bool IsUpgradeBlueprint(this Entity contraption)
        {
            return contraption.GetProperty<bool>(UPGRADE_BLUEPRINT);
        }
        #endregion

        #region 地板器械
        public static readonly PropertyMeta<bool> IS_FLOOR = Get<bool>("isFloor");
        public static bool IsFloor(this Entity contraption)
        {
            return contraption.GetProperty<bool>(IS_FLOOR);
        }
        public static bool IsFloor(this EntityDefinition definition)
        {
            return definition.GetProperty<bool>(IS_FLOOR);
        }
        #endregion

        #region 碎片ID
        public static readonly PropertyMeta<NamespaceID> FRAGMENT_ID = Get<NamespaceID>("fragmentId");
        public static NamespaceID GetFragmentID(this Entity entity) => entity.GetProperty<NamespaceID>(FRAGMENT_ID);
        #endregion

        #region 可触发
        public static readonly PropertyMeta<bool> TRIGGER_ACTIVE = Get<bool>("triggerActive");
        public static void SetTriggerActive(this Entity entity, bool value)
        {
            entity.SetProperty(TRIGGER_ACTIVE, value);
        }
        public static bool IsTriggerActive(this EntityDefinition definition)
        {
            return definition.GetProperty<bool>(TRIGGER_ACTIVE);
        }
        public static bool IsTriggerActive(this Entity entity)
        {
            return entity.GetProperty<bool>(TRIGGER_ACTIVE);
        }
        #endregion

        #region 立即触发
        public static readonly PropertyMeta<bool> INSTANT_TRIGGER = Get<bool>("instantTrigger");
        public static bool CanInstantTrigger(this EntityDefinition definition)
        {
            return definition.GetProperty<bool>(INSTANT_TRIGGER);
        }
        #endregion

        #region 立即激发
        public static readonly PropertyMeta<bool> INSTANT_EVOKE = Get<bool>("instantEvoke");
        public static bool CanInstantEvoke(this EntityDefinition definition)
        {
            return definition.GetProperty<bool>(INSTANT_EVOKE);
        }
        public static bool WillInstantEvoke(this EntityDefinition definition, LevelEngine level)
        {
            if (definition == null)
                return false;
            if (!definition.CanInstantEvoke())
                return false;
            if (level.IsDay() && definition.IsNocturnal())
                return false;
            return true;
        }
        #endregion
    }
}

