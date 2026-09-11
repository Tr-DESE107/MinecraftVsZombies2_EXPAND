#nullable enable  
  
using MukioI18n;  
using MVZ2.GameContent.Bosses;  
using MVZ2.GameContent.Effects;  
using MVZ2.GameContent.Enemies;  
using MVZ2.GameContent.ProgressBars;  
using MVZ2.Vanilla.Audios;  
using MVZ2.Vanilla.Level;  
using MVZ2.Vanilla.Properties;  
using MVZ2Logic;  
using MVZ2Logic.Entities;  
using MVZ2Logic.Level;  
using PVZEngine;  
using PVZEngine.Entities;  
using PVZEngine.Level;  
using Tools;  
using UnityEngine;  
  
namespace MVZ2.GameContent.Stages  
{  
    // 无限梦魇（InfinityNightmare）：一个「梦魇」在设计上是两个形态——  
    // 阶段1 瘦长鬼影(slenderman)，被击败后阶段2 梦魇收割者(nightmareaper)。  
    // 因此每一轮 Boss = 先出 slenderman，slenderman 死亡后原地生成 nightmareaper，  
    // 只有 nightmareaper 死亡才算「击杀一个 Boss」并进入休息。  
    //  
    // 本类不需要修改基类：  
    //  - BossID 设为 nightmareaper（血条 / 击杀判定以最终形态为准）。  
    //    阶段1 只有 slenderman 时，基类 IsBossAlive(nightmareaper)=false 且 BossSeen 从未置真，  
    //    FightingUpdate 会自然空转等待，不会误判为「已击杀」。  
    //  - 覆盖 SpawnBoss：每轮开始先生成 slenderman（而非 nightmareaper），并重置阶段标记。  
    //  - 覆盖 Update：base.Update 之后追加一帧的形态过渡检查（slenderman 消失 → 生成 nightmareaper）。  
    public class InfinityNightmareBehaviour : InfinityBossBehaviour  
    {  
        public InfinityNightmareBehaviour(StageDefinition stageDef) : base(stageDef)  
        {  
        }  
  
        protected override NamespaceID BossID => VanillaBossID.nightmareaper;  
        protected override NamespaceID[] EnemyPool => enemyPool;  
        protected override NamespaceID ProgressBarID => VanillaProgressBarID.nightmare;  
        protected override NamespaceID BossMusic => VanillaMusicID.nightmareBoss; // 阶段1（瘦长鬼影）音乐  
  
        protected override int WarmupWaveCount => 8;  
  
        protected override int BossHealthStart => 3200;  
        protected override int BossHealthStep => 1000;  
        protected override int BossHealthMax => 90000;  
  
        protected override int FirstRestSeconds => 90;  
        protected override int RestStepSeconds => 5;  
        protected override int MinRestSeconds => 12;  
  
        protected override string IntroString => STRING_INTRO;  
        protected override string BossIncomingString => STRING_INCOMING;  
        protected override string ProgressRestString => STRING_PROGRESS_REST;  
  
        // ============ 每帧：在基类状态机之后追加梦魇形态过渡 ============  
        public override void Update(LevelEngine level)  
        {  
            base.Update(level);  
            UpdateNightmarePhase(level);  
        }  
  
        private void UpdateNightmarePhase(LevelEngine level)  
        {  
            switch (GetPhase(level))  
            {  
                case PHASE_SLENDER_PENDING:  
                    // 等到确实见到存活的瘦长鬼影，才进入「已登场」阶段，避免生成延迟导致的误判。  
                    if (IsSlendermanAlive(level))  
                        SetPhase(level, PHASE_SLENDER_SEEN);  
                    break;  
  
                case PHASE_SLENDER_SEEN:  
                    // 瘦长鬼影消失（被击败）→ 原地生成梦魇收割者。  
                    if (!IsSlendermanAlive(level))  
                        SpawnNightmareaper(level);  
                    break;  
            }  
        }  
  
        // ============ 阶段1：每轮先生成瘦长鬼影（覆盖基类 SpawnBoss） ============  
        protected override void SpawnBoss(LevelEngine level, int bossIndex)  
        {  
            // 记录本轮序号（用于阶段2 nightmareaper 的血量），并重置阶段标记。  
            SetCurrentIndex(level, bossIndex);  
            SetPhase(level, PHASE_SLENDER_PENDING);  
  
            var pos = GetBossSpawnPosition(level);  
            var boss = level.Spawn(VanillaBossID.slenderman, pos, null);  
            if (boss != null)  
            {  
                // 直接设定精确最大生命值（不走 ApplyBuffForBossRevenge 的 ×1.5，保证血量曲线可控）。  
                int health = GetBossHealth(bossIndex);  
                boss.SetProperty(EngineEntityProps.MAX_HEALTH, (float)health);  
                boss.Health = health;  
  
                // 参照 SlendermanTransitionBuff 的登场表现。  
                boss.Velocity = Vector3.up * 5;  
                boss.PlaySound(VanillaSoundID.splashBig);  
                boss.PlaySound(VanillaSoundID.glassBreakBig);  
                boss.Spawn(VanillaEffectID.nightmareaperSplash, pos);  
            }  
            level.ShakeScreen(30, 0, 30);  
  
            level.SetProgressBarToBoss(ProgressBarID);  
            level.PlayMusic(BossMusic);        // 阶段1音乐 nightmareBoss  
            level.SetMusicVolume(1);  
        }  
  
        // ============ 阶段2：瘦长鬼影死亡后生成梦魇收割者 ============  
        private void SpawnNightmareaper(LevelEngine level)  
        {  
            // 把已死亡但仍留在场上的瘦长鬼影从 Boss 血条统计中排除，  
            // 否则梦魇收割者的血条会把先前尸体的最大血量算进分母，导致开场血条不满。  
            foreach (var deadSlender in level.FindEntities(e => e.IsEntityOf(VanillaBossID.slenderman)))  
            {  
                deadSlender.SetProperty(LogicBossProps.DONT_COUNT_BOSS_HP, true);  
            }  
  
            int bossIndex = GetCurrentIndex(level);  
            var pos = GetBossSpawnPosition(level);  
            var boss = level.Spawn(VanillaBossID.nightmareaper, pos, null);  
            if (boss != null)  
            {  
                int health = GetBossHealth(bossIndex);  
                boss.SetProperty(EngineEntityProps.MAX_HEALTH, (float)health);  
                boss.Health = health;  
                Nightmareaper.Appear(boss);   // 播放梦魇收割者登场动画  
            }  
  
            level.SetProgressBarToBoss(ProgressBarID);  
            level.PlayMusic(VanillaMusicID.nightmareBoss2);  // 阶段2音乐  
            level.SetMusicVolume(1);  
  
            SetPhase(level, PHASE_REAPER);  
        }  
  
        // ============ 辅助 ============  
        private Vector3 GetBossSpawnPosition(LevelEngine level)  
        {  
            int centerLane = level.GetMaxLaneCount() / 2;  
            return new Vector3(LevelPositions.ENEMY_RIGHT_BORDER, 0, level.GetEntityLaneZ(centerLane));  
        }  
  
        private bool IsSlendermanAlive(LevelEngine level)  
        {  
            return level.EntityExists(e => e.IsEntityOf(VanillaBossID.slenderman) && !e.IsDead && e.IsHostileEntity());  
        }  
  
        // ============ 关卡属性（本关专用，独立 region） ============  
        private const string PROP_REGION_NM = "infinity_nightmare";  
        [LevelPropertyRegistry(PROP_REGION_NM)]  
        public static readonly VanillaLevelPropertyMeta<int> PROP_NM_PHASE = new VanillaLevelPropertyMeta<int>("nm_phase");  
        [LevelPropertyRegistry(PROP_REGION_NM)]  
        public static readonly VanillaLevelPropertyMeta<int> PROP_NM_INDEX = new VanillaLevelPropertyMeta<int>("nm_index");  
  
        private static int GetPhase(LevelEngine level) => level.GetProperty<int>(PROP_NM_PHASE);  
        private static void SetPhase(LevelEngine level, int value) => level.SetProperty(PROP_NM_PHASE, value);  
        private static int GetCurrentIndex(LevelEngine level) => level.GetProperty<int>(PROP_NM_INDEX);  
        private static void SetCurrentIndex(LevelEngine level, int value) => level.SetProperty(PROP_NM_INDEX, value);  
  
        // 阶段常量  
        private const int PHASE_SLENDER_PENDING = 0;  // 已生成瘦长鬼影，等待其登场  
        private const int PHASE_SLENDER_SEEN = 1;     // 瘦长鬼影已在场（存活）  
        private const int PHASE_REAPER = 2;           // 已进入梦魇收割者阶段  

        // ============ 出怪池（占位，请按主题调整） ============  
        private static readonly NamespaceID[] enemyPool = new NamespaceID[]
        {
        VanillaEnemyID.WitherSkeleton,
        VanillaEnemyID.LeatherWitherSkeleton,
        VanillaEnemyID.IronWitherSkeleton,
        VanillaEnemyID.mesmerizer,
        VanillaEnemyID.berserker,
        VanillaEnemyID.dullahan,
        VanillaEnemyID.NetherWarrior,
        VanillaEnemyID.NetherArcher,
        VanillaEnemyID.NetherVanguard,
        VanillaEnemyID.RaiderSkull,
        VanillaEnemyID.Anubiskull,
        };

        // ============ 提示条本地化 Key ============  
        [TranslateMsg("无限梦魇提示")]  
        public const string STRING_INTRO = "坚持发展 8 波后梦魇将降临！先击败瘦长鬼影，再击败它的第二形态——梦魇收割者！";  
        [TranslateMsg("无限梦魇提示")]  
        public const string STRING_INCOMING = "梦魇降临！";  
        [TranslateMsg("无限梦魇提示，{0}为累计击杀数，{1}为休息秒数")]  
        public const string STRING_PROGRESS_REST = "已击败 {0} 个梦魇！休息 {1} 秒后更强的梦魇来袭！";  
    }  
}
