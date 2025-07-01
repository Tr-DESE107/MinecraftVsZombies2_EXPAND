using System.ComponentModel;
using MVZ2.GameContent.Buffs;
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
            UpdateLiquid(entity);
        }
        private bool IsActionInside(int action)
        {
            return action == WaterInteraction.ACTION_ENTER || action == WaterInteraction.ACTION_INSIDE;
        }
        private void UpdateWaterAction(Entity entity, int action)
        {
            switch (action)
            {
                case WaterInteraction.ACTION_REMOVE:
                    {
                        entity.PlaySplashEffect();
                        entity.PlaySplashSound();
                        if (entity.Type == EntityTypes.ENEMY)
                        {
                            entity.Neutralize();
                        }
                        entity.Remove();
                        var callbackParam = new VanillaLevelCallbacks.WaterInteractionParams()
                        {
                            entity = entity,
                            action = action
                        };
                        entity.Level.Triggers.RunCallbackFiltered(VanillaLevelCallbacks.POST_WATER_INTERACTION, callbackParam, action);
                    }
                    break;

                case WaterInteraction.ACTION_ENTER:
                    {
                        entity.PlaySplashEffect();
                        entity.PlaySplashSound();
                        var callbackParam = new VanillaLevelCallbacks.WaterInteractionParams()
                        {
                            entity = entity,
                            action = action
                        };
                        entity.Level.Triggers.RunCallbackFiltered(VanillaLevelCallbacks.POST_WATER_INTERACTION, callbackParam, action);
                    }
                    break;
                case WaterInteraction.ACTION_EXIT:
                    {
                        entity.PlaySplashEffect();
                        entity.PlaySound(VanillaSoundID.water);
                        var callbackParam = new VanillaLevelCallbacks.WaterInteractionParams()
                        {
                            entity = entity,
                            action = action
                        };
                        entity.Level.Triggers.RunCallbackFiltered(VanillaLevelCallbacks.POST_WATER_INTERACTION, callbackParam, action);
                    }
                    break;
            }
        }
        private void UpdateAirAction(Entity entity, int action)
        {
            switch (action)
            {
                case WaterInteraction.ACTION_REMOVE:
                    {
                        entity.PlayAirSplashEffect();
                        entity.PlayAirSplashSound();
                        if (entity.Type == EntityTypes.ENEMY)
                        {
                            entity.Neutralize();
                        }
                        entity.Remove();
                        var callbackParam = new VanillaLevelCallbacks.WaterInteractionParams()
                        {
                            entity = entity,
                            action = action
                        };
                        entity.Level.Triggers.RunCallbackFiltered(VanillaLevelCallbacks.POST_WATER_INTERACTION, callbackParam, action);
                    }
                    break;

                case WaterInteraction.ACTION_ENTER:
                case WaterInteraction.ACTION_EXIT:
                    {
                        entity.PlayAirSplashEffect();
                        entity.PlayAirSplashSound();
                        var callbackParam = new VanillaLevelCallbacks.WaterInteractionParams()
                        {
                            entity = entity,
                            action = action
                        };
                        entity.Level.Triggers.RunCallbackFiltered(VanillaLevelCallbacks.POST_WATER_INTERACTION, callbackParam, action);
                    }
                    break;
            }
        }
        private void UpdateLiquid(Entity entity)
        {
            bool air = entity.IsOnAir();
            bool water = entity.IsOnWater();
            int action;
            if (air)
            {
                action = GetLiquidAction(entity, entity.GetAirInteraction());
            }
            else if (water)
            {
                action = GetLiquidAction(entity, entity.GetWaterInteraction());
            }
            else if (entity.HasBuff<InWaterBuff>())
            {
                entity.RemoveBuffs<InWaterBuff>();
                action = WaterInteraction.ACTION_EXIT;
            }
            else
            {
                return;
            }


            if (air)
            {
                UpdateAirAction(entity, action);
            }
            else if (water)
            {
                UpdateWaterAction(entity, action);
            }

            entity.SetAnimationBool("InWater", IsActionInside(action));
        }
        private int GetLiquidAction(Entity entity, int interaction)
        {
            bool inside = entity.IsOnGround;

            if (interaction == WaterInteraction.REMOVE && inside)
            {
                return WaterInteraction.ACTION_REMOVE;
            }

            if (interaction == WaterInteraction.NONE)
            {
                inside = false;
            }

            if (inside != entity.HasBuff<InWaterBuff>())
            {
                if (inside)
                {
                    entity.AddBuff<InWaterBuff>();
                    return WaterInteraction.ACTION_ENTER;
                }
                else
                {
                    entity.RemoveBuffs<InWaterBuff>();
                    return WaterInteraction.ACTION_EXIT;
                }
            }
            if (inside)
            {
                return WaterInteraction.ACTION_INSIDE;
            }
            return WaterInteraction.ACTION_OUTSIDE;
        }
    }
}