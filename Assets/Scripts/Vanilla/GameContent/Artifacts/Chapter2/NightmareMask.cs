#nullable enable // 自动生成

using System.Collections.Generic;
using MVZ2.GameContent.Buffs;
using MVZ2Logic.Artifacts;
using MVZ2Logic.Level;
using PVZEngine.Auras;
using PVZEngine.Buffs;
using PVZEngine.Callbacks;
using PVZEngine.Entities;
using MVZ2Logic.Definitions;

namespace MVZ2.GameContent.Artifacts
{
    // 神器类：NightmareMask，代表"噩梦面具"神器
    [AutoArtifactDefinition(VanillaArtifactNames.NightmareMask)]
    public class NightmareMask : ArtifactDefinition
    {
        // 构造函数：注册光环效果和实体初始化的回调
        public NightmareMask(string nsp, string name) : base(nsp, name)
        {
            // 添加该神器的光环效果
            AddAura(new NightmareMaskAura());

            // 在实体初始化完成后，创建回调并对 EntityTypes.ENEMY 类型的实体生效
            AddTrigger(LevelCallbacks.POST_ENTITY_INIT, PostEntityInitCallback, filter: EntityTypes.ENEMY);
        }

        // 每帧更新中，让所有友方神器发出发光视觉效果
        public override void PostUpdate(Artifact artifact)
        {
            base.PostUpdate(artifact);
            artifact.SetGlowing(true); // 设置神器的发光效果
        }

        // 回调函数：当任意 ENEMY 实体初始化完成之后，更新光环效果
        private void PostEntityInitCallback(EntityCallbackParams param, CallbackResult result)
        {
            var contraption = param.entity;              // 获取刚初始化的实体
            var level = contraption.Level;               // 获取实体所在的关卡

            // 遍历所有神器
            foreach (var artifact in level.GetArtifacts())
            {
                // 如果神器为空或不是当前神器，则跳过
                if (artifact == null || artifact.Definition != this)
                    continue;

                // 获取神器对应的光环效果
                AuraEffect aura = artifact.GetAuraEffect<NightmareMaskAura>();
                if (aura == null)
                    continue;

                // 强制更新光环，使其应用到目标上
                aura.UpdateAura();
            }
        }

        // 光环类：NightmareMaskAura
        public class NightmareMaskAura : AuraEffectDefinition
        {
            public NightmareMaskAura() : base(VanillaBuffID.Enemy.NightmareMaskEvade)
            {
            }

            // 获取光环影响的实体
            public override void GetAuraTargets(AuraEffect auraEffect, List<IBuffTarget> results)
            {
                var level = auraEffect.Source.GetLevel();  // 获取当前关卡
                results.AddRange(level.GetEntities(EntityTypes.ENEMY)); // 将所有敌人添加到光环目标列表
            }
        }
    }
}
