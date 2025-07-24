using System.Collections.Generic;
using MVZ2.GameContent.Buffs;       // 游戏自定义 Buff 系统命名空间
using MVZ2Logic;                    // MVZ2 游戏核心逻辑
using MVZ2Logic.Artifacts;         // MVZ2 中 Artifact（神器）的逻辑基类
using MVZ2Logic.Level;             // 游戏关卡逻辑
using PVZEngine.Auras;             // 光环（Aura）系统
using PVZEngine.Buffs;             // Buff 系统
using PVZEngine.Callbacks;         // 回调函数系统
using PVZEngine.Entities;          // 实体系统（包括植物、僵尸、器械等）

namespace MVZ2.GameContent.Artifacts
{
    // 定义名为 NightmareMask 的神器（Artifact），名称为“梦魇之面”
    [ArtifactDefinition(VanillaArtifactNames.NightmareMask)]
    public class NightmareMask : ArtifactDefinition
    {
        // 构造函数：注册光环效果和初始化后的回调
        public NightmareMask(string nsp, string name) : base(nsp, name)
        {
            // 添加该神器的光环效果
            AddAura(new NightmareMaskAura());

            // 当实体初始化完成后，触发回调（仅对 EntityTypes.ENEMY 类型生效）
            AddTrigger(LevelCallbacks.POST_ENTITY_INIT, PostEntityInitCallback, filter: EntityTypes.ENEMY);
        }

        // 每帧更新中，让神器具有发光特效（仅视觉效果）
        public override void PostUpdate(Artifact artifact)
        {
            base.PostUpdate(artifact);
            artifact.SetGlowing(true); // 设置神器发光
        }

        // 回调函数：在器械（ENEMY）实体初始化之后，更新光环效果
        private void PostEntityInitCallback(EntityCallbackParams param, CallbackResult result)
        {
            var contraption = param.entity;              // 获取刚初始化的实体
            var level = contraption.Level;               // 获取实体所在的关卡

            // 遍历所有神器
            foreach (var artifact in level.GetArtifacts())
            {
                // 如果神器为空或不是“这个”，跳过
                if (artifact == null || artifact.Definition != this)
                    continue;

                // 获取神器对应的光环效果
                AuraEffect aura = artifact.GetAuraEffect<NightmareMaskAura>();
                if (aura == null)
                    continue;

                // 强制更新光环，使其重新应用到目标上
                aura.UpdateAura();
            }
        }

        // 光环定义类：NightmareMaskAura
        public class NightmareMaskAura : AuraEffectDefinition
        {
            public NightmareMaskAura()
            {
                BuffID = VanillaBuffID.NightmareMaskEvade; // 设置光环对应的 Buff ID（逻辑效果由 buff 决定）
            }

            // 定义光环影响的目标实体
            public override void GetAuraTargets(AuraEffect auraEffect, List<IBuffTarget> results)
            {
                var level = auraEffect.Source.GetLevel();  // 获取当前关卡
                results.AddRange(level.GetEntities(EntityTypes.ENEMY)); // 把所有怪物加入光环作用目标
            }
        }
    }
}
