using System;
using MVZ2.GameContent.Buffs;
using MVZ2.GameContent.Effects;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Callbacks;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Level;
using MVZ2Logic.Level;
using MVZ2Logic.Modding;
using PVZEngine.Callbacks;
using PVZEngine.Entities;
using PVZEngine.Triggers;
using UnityEngine;

namespace MVZ2.GameContent.Implements
{
    public class WaterImplements : VanillaImplements
    {
        public override void Implement(Mod mod)
        {
            mod.AddTrigger(LevelCallbacks.POST_ENTITY_UPDATE, EntityUpdateCallback);
        }
        private void EntityUpdateCallback(Entity entity)
        {
            // 头目、掉落物、特效：无效果。

            // 敌人：可以游泳的，在水中漂浮；不能游泳但支持在水中的动画的，沉入水中淹死；不支持在水中动画的，直接沉没。
            // 射弹：可以在水中漂浮的，漂浮（木球）；不能漂浮但支持在水中的动画的，沉入水中淹没（大雪球、石球）；不支持在水中动画的，直接沉没。（箭矢）

            // 器械：如果重力大于0，不是水生的，并且没有睡莲，沉没。否则漂在水面上。
            // 障碍物、小推车：如果重力大于0，沉没。否则漂在水面上。
            int interaction = entity.GetWaterInteraction();
            bool isInWater = entity.IsInWater();
            if (isInWater && interaction == WaterInteraction.REMOVE)
            {
                entity.PlaySplashEffect();
                entity.PlaySplashSound();
                entity.Remove();
                entity.Level.Triggers.RunCallbackFiltered(VanillaLevelCallbacks.POST_WATER_INTERACTION, WaterInteraction.ACTION_REMOVE, c => c(entity, WaterInteraction.ACTION_REMOVE));

                return;
            }

            bool floatInWater = interaction == WaterInteraction.DROWN || interaction == WaterInteraction.FLOAT;
            if (isInWater && floatInWater)
            {
                if (!entity.HasBuff<InWaterBuff>())
                {
                    entity.AddBuff<InWaterBuff>();
                    entity.PlaySplashEffect();
                    entity.PlaySplashSound();
                    entity.Level.Triggers.RunCallbackFiltered(VanillaLevelCallbacks.POST_WATER_INTERACTION, WaterInteraction.ACTION_ENTER, c => c(entity, WaterInteraction.ACTION_ENTER));
                }
            }
            else
            {
                if (entity.HasBuff<InWaterBuff>())
                {
                    entity.RemoveBuffs(entity.GetBuffs<InWaterBuff>());
                    entity.PlaySplashEffect();
                    entity.PlaySound(VanillaSoundID.water);
                    entity.Level.Triggers.RunCallbackFiltered(VanillaLevelCallbacks.POST_WATER_INTERACTION, WaterInteraction.ACTION_EXIT, c => c(entity, WaterInteraction.ACTION_EXIT));
                }
            }
            entity.SetAnimationBool("InWater", isInWater && floatInWater);
        }
    }
}