#nullable enable  
  
using MukioI18n;
using MVZ2.GameContent.Bosses;
using MVZ2.GameContent.Buffs.Enemies;
using MVZ2.GameContent.Buffs.Level;
using MVZ2.GameContent.Damages;
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
using PVZEngine.Buffs;
using PVZEngine.Damages;
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
  
        protected override int WarmupWaveCount => 10;  
  
        protected override int BossHealthStart => 3600;  
        protected override int BossHealthStep => 1200;  
        protected override int BossHealthMax => 1000000;  
  
        protected override int FirstRestSeconds => 90;  
        protected override int RestStepSeconds => 5;  
        protected override int MinRestSeconds => 12;

        // ============ EXPAND 增强选项幅度（先用基类默认值占位，便于后续逐Boss调整） ============
        protected override int BossHealthBonusPerChoice => 1200; // 血量选项：每次增加的血量（默认= BossHealthStep）
        protected override float BossAttackBonusPerChoice => 0.25f; // 攻击选项：每次攻击倍率 +25%
        protected override float DamageLimitStart => 2400; // 限伤选项：统一限伤初始值
        protected override float DamageLimitStep => 600; // 限伤选项：每次降低量
        protected override float DamageLimitMin => 600; // 限伤选项：下限
        protected override float DamageLimitDecay => 40; // 限伤选项：窗口每帧恢复量
        protected override float RegenBonusPerChoice => 0.5f; // 再生选项：每次增加的每帧回血（30帧=1秒）
  
        protected override string IntroString => STRING_INTRO;  
        protected override string BossIncomingString => STRING_INCOMING;  
        protected override string ProgressRestString => STRING_PROGRESS_REST;  
  
        // ============ 每帧：在基类状态机之后追加梦魇形态过渡 ============
        public override void Update(LevelEngine level)
        {
            base.Update(level);
            UpdateNightmarePhase(level);
        }

        // 过场期间停止刷新小怪（参照原版梦魇关）。
        protected override bool ShouldRunBossWave(LevelEngine level)
        {
            return GetPhase(level) != PHASE_REAPER_TRANSITION;
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
                    // 瘦长鬼影被击败 → 参照原版：关UI关音乐，播放黑暗过场，由过渡Buff生成梦魇收割者。
                    if (!IsSlendermanAlive(level))
                    {
                        SetPhase(level, PHASE_REAPER_TRANSITION);
                        // 尸体不再计入血条，并移除外挂再生Buff。
                        foreach (var deadSlender in level.FindEntities(e => e.IsEntityOf(VanillaBossID.slenderman)))
                        {
                            deadSlender.SetProperty(LogicBossProps.DONT_COUNT_BOSS_HP, true);
                            deadSlender.RemoveBuffs<RegenerationBuff>();
                        }
                        DisableUIAndInput(level);
                        level.StopMusic();
                        level.AddBuff<NightmareaperTransitionBuff>();
                    }
                    break;

                case PHASE_REAPER_TRANSITION:
                    // 过场期间清场并暂停出怪（参照原版）。
                    ClearHostileEnemies(level);
                    EnterEnemiesClearedState(level);
                    if (IsNightmareaperAlive(level))
                    {
                        var reaper = FindFirstHostileBoss(level, VanillaBossID.nightmareaper);
                        if (reaper != null)
                            FinalizeSpawnedBoss(level, reaper, GetCurrentIndex(level));
                        level.SetUIAndInputDisabled(false);
                        ExitEnemiesClearedState(level);
                        SetPhase(level, PHASE_REAPER);
                    }
                    break;
            }
        }

        // ============ 阶段1：每轮先由过场动画生成瘦长鬼影（覆盖基类 SpawnBoss） ============
        protected override bool UsesSpawnTransition => true;

        protected override void SpawnBoss(LevelEngine level, int bossIndex)
        {
            // 记录本轮序号（用于阶段2 nightmareaper 的血量），并重置阶段标记。
            SetCurrentIndex(level, bossIndex);
            SetPhase(level, PHASE_SLENDER_PENDING);

            // 参照原版：由 SlendermanTransitionBuff 播放黑暗降临过场并生成瘦长鬼影
            // （含入场音效/震屏/背景变暗，以及血条与音乐）。
            level.AddBuff<SlendermanTransitionBuff>();
        }

        // 阶段1等待的是瘦长鬼影而非 BossID（nightmareaper）。
        protected override bool IsBossSpawned(LevelEngine level)
        {
            var phase = GetPhase(level);
            if (phase == PHASE_SLENDER_PENDING || phase == PHASE_SLENDER_SEEN)
                return IsSlendermanAlive(level);
            return base.IsBossSpawned(level);
        }

        protected override Entity? FindSpawnedBoss(LevelEngine level)
        {
            var phase = GetPhase(level);
            if (phase == PHASE_SLENDER_PENDING || phase == PHASE_SLENDER_SEEN)
                return FindFirstHostileBoss(level, VanillaBossID.slenderman);
            return base.FindSpawnedBoss(level);
        }

        //EXPAND 梦魇是双形态 Boss：增强/统一限伤对瘦长鬼影与梦魇收割者两形态均生效。
        protected override bool IsUpgradeTarget(LevelEngine level, Entity entity)
        {
            return base.IsUpgradeTarget(level, entity)
                || (entity.IsEntityOf(VanillaBossID.slenderman) && !entity.IsDead);
        }  
  
        // ============ 辅助 ============
        private bool IsSlendermanAlive(LevelEngine level)
        {
            return level.EntityExists(e => e.IsEntityOf(VanillaBossID.slenderman) && !e.IsDead && e.IsHostileEntity());
        }

        private bool IsNightmareaperAlive(LevelEngine level)
        {
            return level.EntityExists(e => e.IsEntityOf(VanillaBossID.nightmareaper) && !e.IsDead && e.IsHostileEntity());
        }

        private Entity? FindFirstHostileBoss(LevelEngine level, NamespaceID id)
        {
            foreach (var e in level.FindEntities(e => e.IsEntityOf(id) && e.IsHostileEntity() && !e.IsDead))
                return e;
            return null;
        }

        private void DisableUIAndInput(LevelEngine level)
        {
            level.ResetHeldItem();
            level.SetUIAndInputDisabled(true);
        }

        private void ClearHostileEnemies(LevelEngine level)
        {
            foreach (var entity in level.FindEntities(e => e.Type == EntityTypes.ENEMY && !e.IsDead && e.IsHostileEntity()))
            {
                entity.Die(new DamageEffectList(VanillaDamageEffects.NO_REVIVAL));
            }
        }

        private void EnterEnemiesClearedState(LevelEngine level)
        {
            if (!level.HasBuff<LevelEnemiesClearedBuff>())
                level.AddBuff<LevelEnemiesClearedBuff>();
        }

        private void ExitEnemiesClearedState(LevelEngine level)
        {
            level.RemoveBuffs<LevelEnemiesClearedBuff>();
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
        private const int PHASE_SLENDER_PENDING = 0;  // 等待过场生成瘦长鬼影
        private const int PHASE_SLENDER_SEEN = 1;     // 瘦长鬼影已在场（存活）
        private const int PHASE_REAPER = 2;           // 已进入梦魇收割者阶段
        private const int PHASE_REAPER_TRANSITION = 3; // 瘦长鬼影被击败，正在播放收割者过场动画

        // ============ 出怪池（占位，请按主题调整） ============  
        private static readonly NamespaceID[] enemyPool = new NamespaceID[]
        {
        VanillaEnemyID.zombie,
        VanillaEnemyID.leatherCappedZombie,
        VanillaEnemyID.ironHelmettedZombie,
        VanillaEnemyID.spider,
        VanillaEnemyID.ghast,
        VanillaEnemyID.motherTerror,
        VanillaEnemyID.necromancer,
        VanillaEnemyID.skeleton,
        VanillaEnemyID.HostMutant,
        VanillaEnemyID.HostZombie,
        VanillaEnemyID.BloodlustHostZombie,
        VanillaEnemyID.EnragedHostZombie,
        VanillaEnemyID.AngryGhast,
        VanillaEnemyID.PhaseSpider,
        };

        // ============ 提示条本地化 Key ============  
        [TranslateMsg("无限梦魇提示")]  
        public const string STRING_INTRO = "坚持发展 10 波后梦魇将降临！先击败瘦长鬼影，再击败它的第二形态——梦魇收割者！";  
        [TranslateMsg("无限梦魇提示")]  
        public const string STRING_INCOMING = "梦魇降临！";  
        [TranslateMsg("无限梦魇提示，{0}为累计击杀数，{1}为休息秒数")]  
        public const string STRING_PROGRESS_REST = "已击败 {0} 个梦魇！休息 {1} 秒后更强的梦魇来袭！";  
    }  
}
