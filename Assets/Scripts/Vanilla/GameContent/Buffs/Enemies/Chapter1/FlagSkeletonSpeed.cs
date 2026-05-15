#nullable enable

using MVZ2.GameContent.Models;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Models;
using MVZ2.Vanilla.Properties;
using MVZ2Logic.Models;
using PVZEngine;
using PVZEngine.Buffs;
using PVZEngine.Definitions;
using PVZEngine.Modifiers;
using MVZ2.Vanilla.Enemies;

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
            var entity = buff.GetEntity();
            if (entity == null)
                return;

            // 根据Category设置速度倍率  
            var category = entity.Definition.GetCategory();
            var multiplier = GetSpeedMultiplier(category);
            buff.SetProperty(PROP_SPEED_MULTIPLIER, multiplier);

        }
        public override void PostRemove(Buff buff)
        {
            base.PostRemove(buff);
            var entity = buff.GetEntity();
            if (entity == null)
                return;
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