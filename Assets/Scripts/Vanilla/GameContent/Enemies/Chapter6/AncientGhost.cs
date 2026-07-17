#nullable enable

using System.Collections.Generic;
using MVZ2.GameContent.Buffs;
using MVZ2.GameContent.Buffs.Enemies;
using MVZ2.GameContent.Entities;
using PVZEngine.Auras;
using PVZEngine.Buffs;
using PVZEngine.Definitions;
using PVZEngine.Entities;

namespace MVZ2.GameContent.Enemies
{
    [AutoEntityBehaviourDefinition(VanillaEnemyNames.AncientGhost)]
    public class AncientGhost : AIEntityBehaviour
    {
        public AncientGhost(string nsp, string name) : base(nsp, name)
        {
            // 添加周期性透明光环  
            AddAura(new AncientGhostAura());
        }

        public override void Init(Entity entity)
        {
            base.Init(entity);
            entity.SetModelProperty("HasFlag", true);
            if (!entity.HasBuff<GhostBuff>())
            {
                entity.AddBuff<GhostBuff>();
            }
            var buff = entity.AddBuff<FlyBuff>();
            buff.SetProperty(FlyBuff.PROP_TARGET_HEIGHT, 1f);
        }

        // 光环定义：每 60 tick 给场上其他敌方怪物施加 AncientGhostBuff  
        public class AncientGhostAura : AuraEffectDefinition
        {
            public AncientGhostAura() : base(VanillaBuffID.Enemy.AncientGhostBuff, 60)
            {
            }

            public override void GetAuraTargets(AuraEffect auraEffect, List<IBuffTarget> results)
            {
                var source = auraEffect.Source.GetEntity();
                if (source == null)
                    return;

                var level = source.Level;

                foreach (var enemy in level.GetEntities(EntityTypes.ENEMY))
                {
                    if (!enemy.ExistsAndAlive())
                        continue;

                    // 排除古代幽灵自身，保证它不会给自己加透明 buff  
                    if (enemy == source)
                        continue;

                    results.Add(enemy);
                }
            }
        }
    }
}
