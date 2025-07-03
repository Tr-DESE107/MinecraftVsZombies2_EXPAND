﻿using MVZ2.GameContent.Buffs;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Callbacks;
using MVZ2.Vanilla.Entities;
using MVZ2Logic.Level;
using MVZ2Logic.Modding;
using PVZEngine.Callbacks;
using PVZEngine.Entities;

namespace MVZ2.GameContent.Implements
{
    public class WaterImplements : VanillaImplements
    {
        public override void Implement(Mod mod)
        {
            mod.AddTrigger(LevelCallbacks.POST_ENTITY_UPDATE, EntityUpdateCallback);
        }
        private void EntityUpdateCallback(EntityCallbackParams param, CallbackResult result)
        {
            var entity = param.entity;
            // 头目、掉落物、特效：无效果。

            // 敌人：可以游泳的，在水中漂浮；不能游泳但支持在水中的动画的，沉入水中淹死；不支持在水中动画的，直接沉没。
            // 射弹：可以在水中漂浮的，漂浮（木球）；不能漂浮但支持在水中的动画的，沉入水中淹没（大雪球、石球）；不支持在水中动画的，直接沉没。（箭矢）

            // 器械：如果重力大于0，不是水生的，并且没有睡莲，沉没。否则漂在水面上。
            // 障碍物、小推车：如果重力大于0，沉没。否则漂在水面上。
            UpdateWater(entity);
        }
        private void TriggerWaterInteraction(Entity entity, int action)
        {
            var callbackParam = new VanillaLevelCallbacks.WaterInteractionParams()
            {
                entity = entity,
                action = action
            };
            entity.Level.Triggers.RunCallbackFiltered(VanillaLevelCallbacks.POST_WATER_INTERACTION, callbackParam, action);
        }
        private void UpdateWater(Entity entity)
        {
            bool water = entity.IsOnWater();
            if (!water)
            {
                if (entity.HasBuff<InWaterBuff>())
                {
                    // 不在水上，但有水上buff，说明之前在水上，现在离开了水面。
                    entity.RemoveBuffs<InWaterBuff>();
                    TriggerWaterInteraction(entity, WaterInteraction.ACTION_EXIT);
                    entity.SetAnimationBool("InWater", false);
                    entity.SetModelProperty("InWater", false);
                }
                return;
            }

            // 在水面之上
            var interaction = entity.GetWaterInteraction();
            bool inWater = entity.IsOnGround;

            if (interaction == WaterInteraction.REMOVE)
            {
                // 遇水移除
                if (inWater)
                {
                    // 在水中
                    entity.PlaySplashEffect();
                    entity.PlaySplashSound();
                    if (entity.Type == EntityTypes.ENEMY)
                    {
                        entity.Neutralize();
                    }
                    entity.Remove();
                    TriggerWaterInteraction(entity, WaterInteraction.ACTION_REMOVE);
                }
                return;
            }
            // 不移除


            if (interaction == WaterInteraction.NONE)
            {
                inWater = false;
            }

            if (inWater != entity.HasBuff<InWaterBuff>())
            {
                entity.PlaySplashEffect();
                if (inWater)
                {
                    entity.AddBuff<InWaterBuff>();
                    entity.PlaySplashSound();
                    TriggerWaterInteraction(entity, WaterInteraction.ACTION_ENTER);
                }
                else
                {
                    entity.RemoveBuffs<InWaterBuff>();
                    entity.PlaySound(VanillaSoundID.water);
                    TriggerWaterInteraction(entity, WaterInteraction.ACTION_EXIT);
                }
            }
            entity.SetAnimationBool("InWater", inWater);
            entity.SetModelProperty("InWater", inWater);
        }
    }
}