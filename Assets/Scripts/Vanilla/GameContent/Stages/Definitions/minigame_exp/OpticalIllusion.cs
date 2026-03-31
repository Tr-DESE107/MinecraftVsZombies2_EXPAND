// OpticalIllusion.cs  
#nullable enable  
  
using MVZ2.GameContent.Enemies;  
using MVZ2.Vanilla.Grids;  
using MVZ2.Vanilla.Level;  
using PVZEngine;  
using PVZEngine.Definitions;  
using PVZEngine.Grids;  
using PVZEngine.Level;  
using UnityEngine;
using MVZ2Logic.Level;
using MVZ2.GameContent.Buffs.Enemies;  // BoatBuff  
using PVZEngine.Buffs;                 // AddBuff  
using PVZEngine.Callbacks;             // LevelCallbacks, EntityCallbackParams, CallbackResult  
using PVZEngine.Entities;              // EntityTypes
using PVZEngine.Entities;       // EntityTypes, Entity  
using PVZEngine.Callbacks;       // LevelCallbacks, EntityCallbackParams, CallbackResult  
using MVZ2.Vanilla.Entities;     // IsInWater 扩展方法
using MVZ2.GameContent.Areas;           // Dream  
using MVZ2.GameContent.Buffs.Level;     // SkywardNightBuff, BloodMoonBuff  
using MVZ2.Vanilla.Level;               // VanillaLevelExt (StartRain, Thunder)  
using MVZ2.Vanilla.Audios;

namespace MVZ2.GameContent.Stages  
{  
    [StageDefinition(VanillaStageNames.OpticalIllusion)]  
    public partial class OpticalIllusion : StageDefinition  
    {  
        public OpticalIllusion(string nsp, string name) : base(nsp, name)  
        {  
            AddBehaviour(new WaveStageBehaviour(this));  
            AddBehaviour(new FinalWaveClearBehaviour(this));  
            AddBehaviour(new GemStageBehaviour(this));  
            AddBehaviour(new StarshardStageBehaviour(this));
            AddTrigger(LevelCallbacks.POST_ENTITY_INIT, PostProjectileInitCallback, filter: EntityTypes.PROJECTILE);
            AddTrigger(LevelCallbacks.POST_ENTITY_INIT, PostEnemyInitCallback, filter: EntityTypes.ENEMY);
        }
        
  
    public override void OnSetup(LevelEngine level)
    {
        base.OnSetup(level);

        // 原有的水陆互换逻辑...  
        SwapWaterAndLand(level);

        // 1. 锁定噩梦模式（切换视觉+音乐+NightmareLevelBuff）  
        Dream.SetToNightmare(level);

        // 2. 锁定夜晚  
        level.AddBuff<SkywardNightBuff>();

        // 3. 开始下雨  
        level.StartRain();

        // 4. 添加周期性打雷（FrankensteinStageBuff 每150帧=5秒雷一次，同时会逐渐压暗光照）  
        level.AddBuff<FrankensteinStageBuff>();

        // 5. 血月红色光芒  
        //level.AddBuff<BloodMoonBuff>();
    }
    private void PostProjectileInitCallback(EntityCallbackParams param, CallbackResult result)
        {
            var projectile = param.entity;
            var level = projectile.Level;
            if (level.StageDefinition != this)
                return;

            // 只调整敌人发射的射弹  
            var spawner = projectile.SpawnerReference?.GetEntity(level);
            if (spawner == null || spawner.Type != EntityTypes.ENEMY)
                return;

            // 如果发射者在水中，抬高射弹生成位置  
            if (spawner.IsInWater())
            {
                var pos = projectile.Position;
                pos.y += PROJECTILE_Y_BOOST;
                projectile.Position = pos;
            }
        }

        /// <summary>  
        /// 水中敌人射弹的Y轴抬升量（可调参数）  
        /// </summary>  
        private const float PROJECTILE_Y_BOOST = 20f;

        public void PostEnemyInitCallback(EntityCallbackParams param, CallbackResult result)
        {
            var entity = param.entity;
            var level = entity.Level;
            if (level.StageDefinition != this)
                return;
            // 给所有敌人添加船，使其在水上和空路上都能漂浮  
            entity.AddBuff<BoatBuff>();
            entity.SetModelProperty("HasBoat", true);

            // 第20波之后，小概率获得幽灵buff  
            if (level.CurrentWave >= WAVE_GHOST_TRIGGER)
            {
                // 20%概率（可调）  
                if (entity.RNG.Next(0f, 1f) < GHOST_CHANCE)
                {
                    if (!entity.HasBuff<GhostBuff>())
                    {
                        entity.AddBuff<GhostBuff>();
                    }
                }
            }
        }
        // 第20波之后开始出现幽灵化敌人  
        private const int WAVE_GHOST_TRIGGER = 20;

        // 敌人获得幽灵buff的概率
        private const float GHOST_CHANCE = 0.2f;

        public override void OnPostWave(LevelEngine level, int wave)  
        {  
            base.OnPostWave(level, wave);  
  
            // 第20波：原水池区域（现在是陆地）变为空路  
            if (wave == WAVE_AIR_TRIGGER)  
            {  
                ConvertOriginalWaterToAir(level);  
            }  
  
            level.PlaySound(VanillaSoundID.scream);
            // 每隔 SATELLITE_INTERVAL 小波生成一次反逆卫星  
            if (wave > 0 && wave % SATELLITE_INTERVAL == 0)  
            {  
                SpawnReverseSatellite(level);  
            }  
        }  
  
        /// <summary>  
        /// 开局交换水池和陆地：  
        /// - 梦境世界原始布局中，第2-5行(lane 1-4)的第4-7列(column 3-6)是水池  
        /// - 交换后：原水池变陆地，原陆地（同行范围内）变水池  
        ///   
        /// 具体策略：遍历所有地格，根据原始 GridDefinition 判断是否为水格  
        /// - 原水格 → SetProperty(IS_WATER, false) 变陆地，并记录位置  
        /// - 原陆地格 → SetProperty(IS_WATER, true) 变水池  
        /// </summary>  
        private void SwapWaterAndLand(LevelEngine level)  
        {  
            for (int lane = 0; lane < level.GetMaxLaneCount(); lane++)  
            {  
                for (int col = 0; col < level.GetMaxColumnCount(); col++)  
                {  
                    var grid = level.GetGrid(col, lane);  
                    if (grid == null)  
                        continue;  
  
                    bool originallyWater = grid.Definition.IsWater();  
  
                    if (originallyWater)  
                    {  
                        // 原水池 → 变陆地  
                        grid.SetProperty(VanillaGridProps.IS_WATER, false);  
                    }  
                    else  
                    {  
                        // 原陆地 → 变水池  
                        grid.SetProperty(VanillaGridProps.IS_WATER, true);  
                    }  
                }  
            }  
        }  
  
        /// <summary>  
        /// 第15波时，将原本是水池的地格（现在已经是陆地）变为空路(cloud/air)。  
        /// 上方的器械和怪物会根据各自的 airInteraction 属性决定是否掉落：  
        /// - 大部分陆地器械会掉落（FALL_OFF 或 REMOVE）  
        /// - 飞行/悬浮单位不受影响（FLOAT 或 NONE）  
        /// </summary>  
        private void ConvertOriginalWaterToAir(LevelEngine level)  
        {  
            for (int lane = 0; lane < level.GetMaxLaneCount(); lane++)  
            {  
                for (int col = 0; col < level.GetMaxColumnCount(); col++)  
                {  
                    var grid = level.GetGrid(col, lane);  
                    if (grid == null)  
                        continue;  
  
                    // 原始定义是水格的位置（开局已被我们变成陆地）  
                    bool originallyWater = grid.Definition.IsWater();  
                    if (originallyWater)  
                    {  
                        // 陆地 → 空路  
                        grid.SetProperty(VanillaGridProps.IS_WATER, false);  
                        grid.SetProperty(VanillaGridProps.IS_AIR, true);  
                    }  
                }  
            }  
  
            // 可选：播放震屏和音效提示玩家  
            //level.ShakeScreen(10, 0, 30);  
        }  
  
        /// <summary>  
        /// 生成反逆卫星，参考辉针城 Castle.PostHugeWaveEvent 的实现。  
        /// 反逆卫星会自动添加 ReverseSatelliteBuff，将视野旋转180度。  
        /// </summary>  
        private void SpawnReverseSatellite(LevelEngine level)  
        {  
            var x = level.GetEnemySpawnX();  
            var z = level.GetEntityLaneZ(level.GetMaxLaneCount() / 2);  
            var y = level.GetGroundY(x, z);  
            var pos = new Vector3(x, y, z);  
            level.Spawn(VanillaEnemyID.reverseSatellite, pos, null);  
        }  
  
        /// <summary>  
        /// 第15波触发空路转换  
        /// </summary>  
        private const int WAVE_AIR_TRIGGER = 20;  
  
        /// <summary>  
        /// 每隔多少小波出一次反逆卫星（可调参数）  
        /// 例如设为5，则第5、10、15、20...波各出一次  
        /// </summary>  
        private const int SATELLITE_INTERVAL = 5;  
    }  
}