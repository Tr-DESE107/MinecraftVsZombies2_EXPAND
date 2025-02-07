using PVZEngine;
using PVZEngine.Entities;

namespace MVZ2.Vanilla.Contraptions
{
    [PropertyRegistryRegion]
    public static class VanillaContraptionProps
    {
        private static PropertyMeta Get(string name)
        {
            return new PropertyMeta(name);
        }
        #region 夜用
        public static readonly PropertyMeta NOCTURNAL = Get("nocturnal");
        public static bool IsNocturnal(this EntityDefinition definition)
        {
            return definition.GetProperty<bool>(NOCTURNAL);
        }
        public static bool IsNocturnal(this Entity entity)
        {
            return entity.GetProperty<bool>(NOCTURNAL);
        }
        #endregion

        #region 防御性
        public static readonly PropertyMeta DEFENSIVE = Get("defensive");
        public static bool IsDefensive(this Entity contraption)
        {
            return contraption.GetProperty<bool>(DEFENSIVE);
        }
        #endregion

        #region 克制
        public static readonly PropertyMeta ATTACKER_TAGS_FOR = Get("attackerFor");
        public static NamespaceID[] GetAttackerTagsFor(this EntityDefinition contraption)
        {
            return contraption.GetProperty<NamespaceID[]>(ATTACKER_TAGS_FOR);
        }
        #endregion

        #region 生产者
        public static readonly PropertyMeta PRODUCER = Get("producer");
        public static bool IsProducer(this EntityDefinition contraption)
        {
            return contraption.GetProperty<bool>(PRODUCER);
        }
        #endregion

        public static readonly PropertyMeta BLOCKS_JUMP = Get("blocksJump");
        public static bool BlocksJump(this Entity contraption)
        {
            return contraption.GetProperty<bool>(BLOCKS_JUMP);
        }

        public static readonly PropertyMeta CANNOT_DIG = Get("cannotDig");
        public static bool CannotDig(this Entity contraption)
        {
            return contraption.GetProperty<bool>(CANNOT_DIG);
        }
        public static readonly PropertyMeta UPGRADE_BLUEPRINT = Get("upgradeBlueprint");
        public static bool IsUpgradeBlueprint(this EntityDefinition definition)
        {
            return definition.GetProperty<bool>(UPGRADE_BLUEPRINT);
        }
        public static bool IsUpgradeBlueprint(this Entity contraption)
        {
            return contraption.GetProperty<bool>(UPGRADE_BLUEPRINT);
        }

        public static readonly PropertyMeta IS_FLOOR = Get("isFloor");
        public static readonly PropertyMeta CAN_DEACTIVE = Get("canDeactive");
        public static readonly PropertyMeta FRAGMENT_ID = Get("fragmentId");
        public static readonly PropertyMeta TRIGGER_ACTIVE = Get("triggerActive");
        public static readonly PropertyMeta INSTANT_TRIGGER = Get("instantTrigger");
        public static bool IsFloor(this Entity contraption)
        {
            return contraption.GetProperty<bool>(IS_FLOOR);
        }
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
        public static bool CanInstantTrigger(this EntityDefinition definition)
        {
            return definition.GetProperty<bool>(INSTANT_TRIGGER);
        }
        public static bool CanDeactive(this Entity entity)
        {
            return entity.GetProperty<bool>(CAN_DEACTIVE);
        }
        public static NamespaceID GetFragmentID(this Entity entity) => entity.GetProperty<NamespaceID>(VanillaContraptionProps.FRAGMENT_ID);
    }
}
