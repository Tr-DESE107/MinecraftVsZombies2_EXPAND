// OpticalIllusion.cs  
#nullable enable  
  
using MVZ2.GameContent.Enemies;  
using MVZ2.Vanilla.Grids;  
using MVZ2.Vanilla.Level;  
using PVZEngine;  
using PVZEngine.Definitions;  
using PVZEngine.Grids;  
using PVZEngine.Level;
using PVZEngine.Definitions;  
using UnityEngine;
using MVZ2Logic.Level;
using MVZ2.GameContent.Buffs.Enemies;  // BoatBuff  
using PVZEngine.Buffs;                 // AddBuff  
using PVZEngine.Callbacks;             // LevelCallbacks, EntityCallbackParams, CallbackResult  
using PVZEngine.Entities;              // EntityTypes
using PVZEngine.Entities;       // EntityTypes, Entity  
using PVZEngine.Callbacks;       // LevelCallbacks, EntityCallbackParams, CallbackResult  
using MVZ2.Vanilla.Entities;     // IsInWater ��չ����
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

        // ԭ�е�ˮ½�����߼�...  
        SwapWaterAndLand(level);

        // 1. ����ج��ģʽ���л��Ӿ�+����+NightmareLevelBuff��  
        Dream.SetToNightmare(level);

        // 2. ����ҹ��  
        level.AddBuff<SkywardNightBuff>();

        // 3. ��ʼ����  
        level.StartRain();

        // 4. ���������Դ��ף�FrankensteinStageBuff ÿ150֡=5����һ�Σ�ͬʱ����ѹ�����գ�  
        level.AddBuff<FrankensteinStageBuff>();

        // 5. Ѫ�º�ɫ��â  
        //level.AddBuff<BloodMoonBuff>();
    }
    private void PostProjectileInitCallback(EntityCallbackParams param, CallbackResult result)
        {
            var projectile = param.entity;
            var level = projectile.Level;
            if (level.StageDefinition != this)
                return;

            // ֻ�������˷�����䵯  
            var spawner = projectile.SpawnerReference?.GetEntity(level);
            if (spawner == null || spawner.Type != EntityTypes.ENEMY)
                return;

            // �����������ˮ�У�̧���䵯����λ��  
            if (spawner.IsInWater())
            {
                var pos = projectile.Position;
                pos.y += PROJECTILE_Y_BOOST;
                projectile.Position = pos;
            }
        }

        /// <summary>  
        /// ˮ�е����䵯��Y��̧�������ɵ�������  
        /// </summary>  
        private const float PROJECTILE_Y_BOOST = 20f;

        public void PostEnemyInitCallback(EntityCallbackParams param, CallbackResult result)
        {
            var entity = param.entity;
            var level = entity.Level;
            if (level.StageDefinition != this)
                return;
            // �����е������Ӵ���ʹ����ˮ�ϺͿ�·�϶���Ư��  
            entity.AddBuff<BoatBuff>();
            entity.SetModelProperty("HasBoat", true);

            // ��20��֮��С���ʻ������buff  
            if (level.CurrentWave >= WAVE_GHOST_TRIGGER)
            {
                // 20%���ʣ��ɵ���  
                if (entity.RNG.Next(0f, 1f) < GHOST_CHANCE)
                {
                    if (!entity.HasBuff<GhostBuff>())
                    {
                        entity.AddBuff<GhostBuff>();
                    }
                }
            }
        }
        // ��20��֮��ʼ�������黯����  
        private const int WAVE_GHOST_TRIGGER = 20;

        // ���˻������buff�ĸ���
        private const float GHOST_CHANCE = 0.2f;

        public override void OnPostWave(LevelEngine level, int wave)  
        {  
            base.OnPostWave(level, wave);  
  
            // ��20����ԭˮ������������½�أ���Ϊ��·  
            if (wave == WAVE_AIR_TRIGGER)  
            {  
                ConvertOriginalWaterToAir(level);  
            }  
  
            level.PlaySound(VanillaSoundID.scream);
            // ÿ�� SATELLITE_INTERVAL С������һ�η�������  
            if (wave > 0 && wave % SATELLITE_INTERVAL == 0)  
            {  
                SpawnReverseSatellite(level);  
            }  
        }  
  
        /// <summary>  
        /// ���ֽ���ˮ�غ�½�أ�  
        /// - �ξ�����ԭʼ�����У���2-5��(lane 1-4)�ĵ�4-7��(column 3-6)��ˮ��  
        /// - ������ԭˮ�ر�½�أ�ԭ½�أ�ͬ�з�Χ�ڣ���ˮ��  
        ///   
        /// ������ԣ��������еظ񣬸���ԭʼ GridDefinition �ж��Ƿ�Ϊˮ��  
        /// - ԭˮ�� �� SetProperty(IS_WATER, false) ��½�أ�����¼λ��  
        /// - ԭ½�ظ� �� SetProperty(IS_WATER, true) ��ˮ��  
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
                        // ԭˮ�� �� ��½��  
                        grid.SetProperty(VanillaGridProps.IS_WATER, false);  
                    }  
                    else  
                    {  
                        // ԭ½�� �� ��ˮ��  
                        grid.SetProperty(VanillaGridProps.IS_WATER, true);  
                    }  
                }  
            }  
        }  
  
        /// <summary>  
        /// ��15��ʱ����ԭ����ˮ�صĵظ������Ѿ���½�أ���Ϊ��·(cloud/air)��  
        /// �Ϸ�����е�͹������ݸ��Ե� airInteraction ���Ծ����Ƿ���䣺  
        /// - �󲿷�½����е����䣨FALL_OFF �� REMOVE��  
        /// - ����/������λ����Ӱ�죨FLOAT �� NONE��  
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
  
                    // ԭʼ������ˮ���λ�ã������ѱ����Ǳ��½�أ�  
                    bool originallyWater = grid.Definition.IsWater();  
                    if (originallyWater)  
                    {  
                        // ½�� �� ��·  
                        grid.SetProperty(VanillaGridProps.IS_WATER, false);  
                        grid.SetProperty(VanillaGridProps.IS_AIR, true);  
                    }  
                }  
            }  
  
            // ��ѡ��������������Ч��ʾ���  
            //level.ShakeScreen(10, 0, 30);  
        }  
  
        /// <summary>  
        /// ���ɷ������ǣ��ο������ Castle.PostHugeWaveEvent ��ʵ�֡�  
        /// �������ǻ��Զ����� ReverseSatelliteBuff������Ұ��ת180�ȡ�  
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
        /// ��15��������·ת��  
        /// </summary>  
        private const int WAVE_AIR_TRIGGER = 20;  
  
        /// <summary>  
        /// ÿ������С����һ�η������ǣ��ɵ�������  
        /// ������Ϊ5�����5��10��15��20...������һ��  
        /// </summary>  
        private const int SATELLITE_INTERVAL = 5;  
    }  
}