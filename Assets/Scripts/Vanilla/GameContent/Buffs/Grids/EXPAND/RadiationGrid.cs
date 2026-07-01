#nullable enable

using MVZ2.GameContent.Damages;
using MVZ2.GameContent.Effects;
using MVZ2.GameContent.Models;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Grids;
using MVZ2.Vanilla.Models;
using MVZ2.Vanilla.Properties;
using MVZ2Logic.Entities;
using MVZ2Logic.Models;
using PVZEngine;
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
            // 用 FindEntities 按"当前所在格子"判定：  
            // 这样行走中的怪物（不占格）也能命中，而不仅是占格的器械。  
            foreach (var entity in level.FindEntities(e => e.GetGrid() == grid))
            {
                if (entity == null || !entity.ExistsAndAlive())
                    continue;
                if (!entity.IsVulnerableEntity())
                    continue;

                var effects = new DamageEffectList(
                    VanillaDamageEffects.MUTE,
                    VanillaDamageEffects.IGNORE_ARMOR,
                    VanillaDamageEffects.NO_DEATH_EFFECTS,
                    VanillaDamageEffects.RADIATION
                    );
                entity.TakeDamageSourced(RADIATION_DAMAGE, effects, new GridSourceReference(grid));
            }
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

                // 关键：写入 GridType，prefab 中 4 个子物体据此切换贴图。  
                model.SetModelProperty("GridType", grid.GetGridModelType());
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
                light.Position = grid.GetEntityPosition() + LIGHT_OFFSET;  // 这里也要加上同样的偏移  
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
        public const float MAX_TIMEOUT_SECONDS = 10; // 持续时间，可调  
        public const float RADIATION_DAMAGE = 1;     // 每帧伤害，可调  
        public static readonly NamespaceID MODEL_KEY = VanillaModelKeys.RadiationGrid;
        public static readonly VanillaBuffPropertyMeta<FrameTimer> PROP_FLASH_TIMER = new VanillaBuffPropertyMeta<FrameTimer>("flash_timer");
        public static readonly VanillaBuffPropertyMeta<FrameTimer> PROP_TIMEOUT_TIMER = new VanillaBuffPropertyMeta<FrameTimer>("timeout_timer");
        public static readonly VanillaBuffPropertyMeta<EntityID> PROP_LIGHT = new VanillaBuffPropertyMeta<EntityID>("light");
    }
}
