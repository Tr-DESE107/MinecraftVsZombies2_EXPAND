#nullable enable

using MVZ2.GameContent.Models;
using MVZ2.Vanilla.Enemies;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Models;
using MVZ2.Vanilla.Properties;
using MVZ2Logic.Models;
using PVZEngine;
using PVZEngine.Buffs;
using PVZEngine.Definitions;
using PVZEngine.Modifiers;

namespace MVZ2.GameContent.Buffs.Enemies
{
    [AutoBuffDefinition(VanillaBuffNames.Enemy.FlagSkeletonSpeed)]
    public class FlagSkeletonSpeedBuff : BuffDefinition
    {
        public FlagSkeletonSpeedBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new FloatModifier(VanillaEnemyProps.SPEED, NumberOperator.Multiply, PROP_SPEED_MULTIPLIER));
            AddModelInsertion(LogicModelHelper.ANCHOR_CENTER, VanillaModelKeys.SpeedParticles, VanillaModelID.SpeedParticles);
        }

        public override void PostAdd(Buff buff)
        {
            base.PostAdd(buff);
            UpdateMultiplier(buff);
        }

        public override void PostUpdate(Buff buff)
        {
            base.PostUpdate(buff);
            // 每帧刷新：确保同一实体身上多个旗帜骷髅buff只有一个生效  
            UpdateMultiplier(buff);
        }

        public override void PostRemove(Buff buff)
        {
            base.PostRemove(buff);
            // buff移除后，让剩余的buff重新推选出一个生效者  
            var entity = buff.GetEntity();
            if (entity == null)
                return;
            foreach (var other in entity.GetBuffs<FlagSkeletonSpeedBuff>())
            {
                if (other != buff)
                    UpdateMultiplier(other);
            }
        }

        // 只有实体身上的“第一个”FlagSkeletonSpeedBuff生效，其余乘算倍率设为1（无效果）  
        private void UpdateMultiplier(Buff buff)
        {
            var entity = buff.GetEntity();
            if (entity == null)
                return;

            var first = entity.GetFirstBuff<FlagSkeletonSpeedBuff>();
            if (first == buff)
            {
                var category = entity.Definition.GetCategory();
                buff.SetProperty(PROP_SPEED_MULTIPLIER, GetSpeedMultiplier(category));
            }
            else
            {
                // 不是第一个：不叠加，倍率保持1  
                buff.SetProperty(PROP_SPEED_MULTIPLIER, 1f);
            }
        }

        private float GetSpeedMultiplier(NamespaceID category)
        {
            if (category == new NamespaceID("mvz2", "Skeleton_Exp"))
                return 2f;
            if (category == new NamespaceID("mvz2", "WitherSkeleton_Exp"))
                return 1.5f;
            if (category == new NamespaceID("mvz2", "WINtherSkeleton_Exp"))
                return 1.2f;
            return 1f;
        }

        public static readonly VanillaBuffPropertyMeta<float> PROP_SPEED_MULTIPLIER = new VanillaBuffPropertyMeta<float>("SpeedMultiplier", 1f);
    }
}
