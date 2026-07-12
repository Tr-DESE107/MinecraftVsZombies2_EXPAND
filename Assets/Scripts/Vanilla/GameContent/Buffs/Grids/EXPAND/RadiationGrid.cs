#nullable enable  
  
using MVZ2.GameContent.Buffs.Entities;  
using MVZ2.GameContent.Damages;  
using MVZ2.GameContent.Effects;  
using MVZ2.GameContent.Models;  
using MVZ2.Vanilla.Entities;  
using MVZ2.Vanilla.Grids;  
using MVZ2.Vanilla.Models;  
using MVZ2.Vanilla.Properties;  
using MVZ2Logic.Armors;
using MVZ2Logic.Entities;  
using MVZ2Logic.Models;  
using PVZEngine;  
using PVZEngine.Armors;  
using PVZEngine.Buffs;  
using PVZEngine.Damages;  
using PVZEngine.Definitions;  
using PVZEngine.Entities;  
using PVZEngine.Grids;  
using Tools;  
using UnityEngine;

namespace MVZ2.GameContent.Buffs.Grids  
{  
    [AutoBuffDefinition(VanillaBuffNames.Grid.RadiationGrid)]  
    public class RadiationGridBuff : BuffDefinition  
    {  
        public RadiationGridBuff(string nsp, string name) : base(nsp, name)  
        {  
            // 不加 IS_WATER / DISABLED：不改地形、不改出怪、不限制器械放置。      
            AddModelInsertion(LogicModelHelper.ANCHOR_CENTER, MODEL_KEY, VanillaModelID.RadiationGrid);  
        }  
  
        public override void OnCreate(Buff buff)  
        {  
            base.OnCreate(buff);  
            buff.SetProperty(PROP_FLASH_TIMER, TimerHelper.NewSecondTimer(FLASH_SECONDS));  
            // 持续时间计时器：到期后移除该地板 buff，避免无尽。      
            buff.SetProperty(PROP_TIMEOUT_TIMER, TimerHelper.NewSecondTimer(MAX_TIMEOUT_SECONDS));  
        }  
  
        public override void PostAdd(Buff buff)  
        {  
            base.PostAdd(buff);  
            UpdateModel(buff);  
            SpawnLight(buff);   // 生成照亮幽灵的光源实体      
        }  
  
        public override void PostUpdate(Buff buff)  
        {  
            base.PostUpdate(buff);  
  
            var flashTimer = buff.GetProperty<FrameTimer>(PROP_FLASH_TIMER);  
            flashTimer?.Run();  
  
            var timeoutTimer = buff.GetProperty<FrameTimer>(PROP_TIMEOUT_TIMER);  
            timeoutTimer?.Run();  
  
            DamageEntitiesOnGrid(buff);  
            UpdateModel(buff);  
            UpdateLight(buff);  // 维持光源存活并跟随格子      
  
            // 到期移除，恢复地板，并清掉光源。      
            if (timeoutTimer == null || timeoutTimer.Expired)  
            {  
                RemoveLight(buff);  
                buff.Remove();  
            }  
        }  
  
        // buff 被任何途径移除时，确保光源实体也被清掉，避免残留。      
        public override void PostRemove(Buff buff)  
        {  
            base.PostRemove(buff);  
            RemoveLight(buff);  
        }  
  
        private void DamageEntitiesOnGrid(Buff buff)  
        {  
            var grid = buff.Target as LawnGrid;  
            if (grid == null)  
                return;  
            var level = grid.Level;  
            // 用 FindEntities 按“当前所在格子”判定：      
            // 这样行走中的怪物（不占格）也能命中，而不仅是占格的器械。      
            foreach (var entity in level.FindEntities(e => e.GetGrid() == grid))  
            {  
                if (entity == null || !entity.ExistsAndAlive())  
                    continue;  
                if (!entity.IsVulnerableEntity())  
                    continue;  
  
                // 离地高度衰减：贴地=1，越高越少，达到 RADIATION_MAX_HEIGHT 及以上=0。      
                float relativeY = Mathf.Max(0f, entity.GetRelativeY());  
                float heightFactor = Mathf.Clamp01(1f - relativeY / RADIATION_MAX_HEIGHT);  
  
                // 高度过高时跳过直接伤害结算（0 伤害无意义）。      
                if (heightFactor > 0f)  
                {  
                    var armor = entity.GetMainArmor();  
                    if (Armor.Exists(armor) && !armor.IsIgnored())  
                    {  
                        // 有盔甲：50% 穿甲伤害，再乘高度衰减。      
                        var effects = new DamageEffectList(  
                            VanillaDamageEffects.MUTE,  
                            VanillaDamageEffects.IGNORE_ARMOR,  
                            VanillaDamageEffects.NO_DEATH_EFFECTS,  
                            VanillaDamageEffects.RADIATION  
                            );  
                        entity.TakeDamageSourced(  
                            RADIATION_DAMAGE * ARMORED_PENETRATION_RATIO * heightFactor,  
                            effects,  
                            new GridSourceReference(grid));  
                    }  
                    else  
                    {  
                        // 盔甲为空：100% 原本伤害（打身体），再乘高度衰减。      
                        var effects = new DamageEffectList(  
                            VanillaDamageEffects.MUTE,  
                            VanillaDamageEffects.NO_DEATH_EFFECTS,  
                            VanillaDamageEffects.RADIATION  
                            );  
                        entity.TakeDamageSourced(  
                            RADIATION_DAMAGE * heightFactor,  
                            effects,  
                            new GridSourceReference(grid));  
                    }  
                }  
  
                // 降低最大生命值同样随离地高度衰减：贴地满额，越高越少。      
                ApplyNuclearDiffusion(entity, heightFactor);  
            }  
        }  
  
        // 每个实体只保留一个 NuclearDiffusionBuff：没有则新增，有则只改内部数值。      
        // heightFactor：离地高度衰减系数（1=贴地满额，0=高到不再累减）。      
        private void ApplyNuclearDiffusion(Entity entity, float heightFactor)  
        {  
            if (heightFactor <= 0f)  
                return; // 高度过高，本帧不降低最大生命值。    
  
            // 辐射保护同样减免“降低最大生命值”的效果，取最高保护等级。    
            float protection = RadiationProtection.GetProtectionLevel(entity);  
            float reductionPerFrame = HEALTH_REDUCTION_PER_FRAME * (1f - protection) * heightFactor;  
            if (reductionPerFrame <= 0f)  
                return; // 保护满级（>=1）时完全免疫，不再累减。    
  
            var buff = entity.GetFirstBuff<NuclearDiffusionBuff>();  
            if (buff == null)  
            {  
                buff = entity.AddBuff<NuclearDiffusionBuff>();  
            }  
            // PROP_HEALTH_REDUCTION 存负数，Add 即减少最大生命值；每帧累减固定值。    
            var current = buff.GetProperty<float>(NuclearDiffusionBuff.PROP_HEALTH_REDUCTION);  
            buff.SetProperty(NuclearDiffusionBuff.PROP_HEALTH_REDUCTION, current - reductionPerFrame);  
        }  
  
        public void UpdateModel(Buff buff)  
        {  
            var grid = buff.Target as LawnGrid;  
            if (grid == null)  
                return;  
            var model = buff.GetInsertedModel(MODEL_KEY);  
            if (model != null)  
            {  
                var flashTimer = buff.GetProperty<FrameTimer>(PROP_FLASH_TIMER);  
                var c = flashTimer?.GetTimeoutPercentage() ?? 0;  
  
                // 剩余时间百分比：1（满）-> 0（到期）      
                var timeoutTimer = buff.GetProperty<FrameTimer>(PROP_TIMEOUT_TIMER);  
                float remaining = timeoutTimer?.GetTimeoutPercentage() ?? 1f;  
                // alpha 从 1.0 线性降到最小值     
                float alpha = Mathf.Lerp(MIN_ALPHA, 1f, remaining);  
  
                model.SetModelProperty("GridType", grid.GetGridModelType());  
                model.SetShaderColor(ShaderProperties.TINT, new Color(1, 1, 1, alpha));  
                model.ApplyShaderProperties();  
            }  
        }  
        // 新增一个常量，统一管理光源相对格子中心的偏移      
        public static readonly Vector3 LIGHT_OFFSET = new Vector3(0, 20f, 0); // 按需调整      
  
        private void SpawnLight(Buff buff)  
        {  
            var grid = buff.Target as LawnGrid;  
            if (grid == null)  
                return;  
            var level = grid.Level;  
            var pos = grid.GetEntityPosition() + LIGHT_OFFSET;   // 用统一偏移      
            var light = level.Spawn(VanillaEffectID.RadiationLight, pos, null);  
            if (light != null)  
                buff.SetProperty(PROP_LIGHT, new EntityID(light));  
        }  
  
        private void UpdateLight(Buff buff)  
        {  
            var grid = buff.Target as LawnGrid;  
            if (grid == null)  
                return;  
            var lightID = buff.GetProperty<EntityID>(PROP_LIGHT);  
            var light = lightID?.GetEntity(buff.Level);  
            if (!light.ExistsAndAlive())  
            {  
                SpawnLight(buff);  
            }  
            else  
            {  
                light.Position = grid.GetEntityPosition() + LIGHT_OFFSET;  
  
                // 剩余时间百分比：1（满）-> 0（到期）      
                var timeoutTimer = buff.GetProperty<FrameTimer>(PROP_TIMEOUT_TIMER);  
                float remaining = timeoutTimer?.GetTimeoutPercentage() ?? 1f;  
                // 光照范围从 LIGHT_RANGE_FULL 线性降到 0      
                light.SetLightRange(LIGHT_RANGE_FULL * remaining);  
            }  
        }  
  
        private void RemoveLight(Buff buff)  
        {  
            var lightID = buff.GetProperty<EntityID>(PROP_LIGHT);  
            var light = lightID?.GetEntity(buff.Level);  
            if (light.ExistsAndAlive())  
                light.Remove();  
            buff.SetProperty<EntityID>(PROP_LIGHT, null);  
        }  
  
        // 给指定格子添加该地板（仿 PagodaLaser.DisableGrid）。      
        public static void Radiate(LawnGrid grid)  
        {  
            grid.AddBuff(VanillaBuffID.Grid.RadiationGrid);  
        }  
  
        public const float FLASH_SECONDS = 1;  
        public const float MAX_TIMEOUT_SECONDS = 10;  
        public const float RADIATION_DAMAGE = 2;  
  
        // 有盔甲时的穿甲伤害倍率（50%）。      
        public const float ARMORED_PENETRATION_RATIO = 0.5f;  
        // 辐射伤害与最大生命值减少随离地高度衰减到 0 的高度阈值，可按需调参。      
        public const float RADIATION_MAX_HEIGHT = 100f;  
  
        public const float HEALTH_REDUCTION_PER_FRAME = 0.2f;  
  
        public static readonly NamespaceID MODEL_KEY = VanillaModelKeys.RadiationGrid;  
        public static readonly VanillaBuffPropertyMeta<FrameTimer> PROP_FLASH_TIMER = new VanillaBuffPropertyMeta<FrameTimer>("flash_timer");  
        public static readonly VanillaBuffPropertyMeta<FrameTimer> PROP_TIMEOUT_TIMER = new VanillaBuffPropertyMeta<FrameTimer>("timeout_timer");  
        public static readonly VanillaBuffPropertyMeta<EntityID> PROP_LIGHT = new VanillaBuffPropertyMeta<EntityID>("light");  
        public const float MIN_ALPHA = 0.01f;  
        public static readonly Vector3 LIGHT_RANGE_FULL = new Vector3(120, 120, 120);  
    }  
}
