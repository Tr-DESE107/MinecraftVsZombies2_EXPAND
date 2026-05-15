#nullable enable

using System.Collections.Generic;

using MVZ2.GameContent.Armors;
using MVZ2.GameContent.Buffs;
using MVZ2.GameContent.Buffs.Enemies;
using MVZ2.GameContent.Buffs.Entities;
using MVZ2.Vanilla.Entities;

using MVZ2Logic.Entities;
using PVZEngine;
using PVZEngine.Auras;
using PVZEngine.Buffs;
using PVZEngine.Damages;
using PVZEngine.Entities;
using PVZEngine.Level;
using PVZEngine.Definitions;

namespace MVZ2.GameContent.Enemies
{
    [AutoEntityBehaviourDefinition(VanillaEnemyNames.FlagSkeleton)]
    public class FlagSkeleton : MeleeSkeleton
    {
        public FlagSkeleton(string nsp, string name) : base(nsp, name)
        {
            // 添加Aura光环  
            AddAura(new SkeletonSpeedAura());
        }

        public override void Init(Entity entity)
        {
            base.Init(entity);
            entity.SetAnimationBool("HasFlag", true);
            entity.EquipMainArmor(VanillaArmorID.ironHelmet);
        }
        
        // Aura光环定义  
        public class SkeletonSpeedAura : AuraEffectDefinition
        {
            public SkeletonSpeedAura() : base(VanillaBuffID.Enemy.FlagSkeletonSpeed, 60)
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

                    var category = enemy.Definition.GetCategory();
                    if (IsSkeletonCategory(category))
                    {
                        results.Add(enemy);
                    }
                }
            }

            private bool IsSkeletonCategory(NamespaceID category)
            {
                return category == new NamespaceID("mvz2", "Skeleton_Exp")
                    || category == new NamespaceID("mvz2", "WitherSkeleton_Exp")
                    || category == new NamespaceID("mvz2", "WINtherSkeleton_Exp");
            }
            public override void UpdateTargetBuff(AuraEffect effect, IBuffTarget target, Buff buff)
            {
                base.UpdateTargetBuff(effect, target, buff);
            }
        }
    }
}