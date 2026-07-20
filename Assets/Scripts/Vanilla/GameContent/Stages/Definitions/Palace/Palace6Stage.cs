#nullable enable

using MVZ2.GameContent.Enemies;
using MVZ2.GameContent.Seeds;
using MVZ2.Vanilla.Entities;
using PVZEngine;
using PVZEngine.Callbacks;
using PVZEngine.Definitions;
using PVZEngine.Entities;

namespace MVZ2.GameContent.Stages
{
    [AutoStageDefinition(VanillaStageNames.palace6)]
    public class Palace6Stage : HeavyWeaponStageBase
    {
        public Palace6Stage(string nsp, string name) : base(nsp, name)
        {
            // 实体初始化时触发，只处理敌方怪物  
            AddTrigger(LevelCallbacks.POST_ENTITY_INIT, PostEnemyInitCallback, filter: EntityTypes.ENEMY);
        }

        protected override NamespaceID[] GetBlueprintsID()
        {
            return new NamespaceID[3]
            {
                VanillaBlueprintID.heavyWeaponNuke,
                VanillaBlueprintID.heavyWeaponRapid,
                VanillaBlueprintID.heavyWeaponSpread,
            };
        }

        private void PostEnemyInitCallback(EntityCallbackParams param, CallbackResult result)
        {
            var entity = param.entity;
            var level = entity.Level;

            // 关键：只在本关卡（palace6）实例生效，其他关卡的 AncientGhost 不受影响  
            if (level.StageDefinition != this)
                return;

            // 只让 AncientGhost 立刻死亡/移除  
            if (entity.GetDefinitionID() == VanillaEnemyID.AncientGhost)
            {
                entity.RemoveDie();
            }
        }
    }
}
