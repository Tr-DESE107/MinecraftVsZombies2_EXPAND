#nullable enable

using MukioI18n;
using MVZ2.Vanilla.Level;
using MVZ2.Vanilla.Properties;
using MVZ2Logic;
using MVZ2Logic.Entities;
using MVZ2Logic.Level;
using MVZ2Logic.Localization;
using MVZ2Logic.Stats;
using PVZEngine;
using PVZEngine.Entities;
using PVZEngine.Level;
using Tools;
using UnityEngine;

namespace MVZ2.GameContent.Stages
{
    // 无限 Boss 基类（InfinityBossBehaviour）：  
    // 1) 开局先跑 WarmupWaveCount 波普通波次，之后切换为 Boss 战。  
    // 2) 没有战役阶段，直接进入无尽：每轮生成【一个】Boss。  
    // 3) Boss 血量只与「这是第几只 Boss」有关（GetBossHealth(bossIndex)），随序号提升并封顶。  
    // 4) 每击杀一个 Boss，休息一段时间（随击杀数递减，最低 MinRestSeconds），然后出下一个更强的 Boss。  
    // 5) 击杀的 Boss 数量记录到 CATEGORY_MAX_ENDLESS_FLAGS（小游戏界面「最高连胜」显示这个）。  
    //  
    // 子类可定制：Boss 是什么、出怪池、血量曲线、休息时间曲线、开局跑几波、登场特效、血条/音乐、开场初始化、提示文案。  
    public abstract class InfinityBossBehaviour : StageBehaviour
    {
        protected InfinityBossBehaviour(StageDefinition stageDef) : base(stageDef)
        {
        }

        #region 子类定制点（必须实现）  
        // 本关的 Boss ID。  
        protected abstract NamespaceID BossID { get; }
        // 无尽阶段的小怪出怪池。  
        protected abstract NamespaceID[] EnemyPool { get; }
        // Boss 血条 ID。  
        protected abstract NamespaceID ProgressBarID { get; }
        // Boss 背景音乐 ID。  
        protected abstract NamespaceID BossMusic { get; }
        #endregion

        #region 子类定制点（可选覆盖）  
        // 开局先跑多少波普通怪才生成第一个 Boss（你的变量 a）。  
        protected virtual int WarmupWaveCount => 5;

        // Boss 血量曲线参数：第 1 个 Boss 的血量、每多一个 Boss 增加的血量、血量上限。  
        protected virtual int BossHealthStart => 1000;
        protected virtual int BossHealthStep => 1000;
        protected virtual int BossHealthMax => 100000;

        // 休息时间曲线参数（秒）：首次休息、每击杀一个 Boss 递减、最低值。  
        protected virtual int FirstRestSeconds => 95;
        protected virtual int RestStepSeconds => 5;
        protected virtual int MinRestSeconds => 10;

        // Boss 血量：只与「这是第几只 Boss」有关（bossIndex 从 0 开始）。  
        // 例：第 1 个 Boss = BossHealthStart，第 2 个 = BossHealthStart + BossHealthStep …… 封顶 BossHealthMax。  
        protected virtual int GetBossHealth(int bossIndex)
        {
            return Mathf.Min(BossHealthStart + BossHealthStep * bossIndex, BossHealthMax);
        }

        // 休息秒数：随已击杀 Boss 数递减，最低 MinRestSeconds。  
        protected virtual int GetRestSeconds(int bossKilled)
        {
            return Mathf.Max(FirstRestSeconds - RestStepSeconds * bossKilled, MinRestSeconds);
        }

        // Boss 登场特效（默认无）。子类重写调用各自 Boss 的 Appear。  
        protected virtual void OnBossAppear(Entity boss)
        {
        }

        // 关卡开始时的额外初始化（默认无）。例如凋灵关闭血量再生。  
        protected virtual void OnStageStart(LevelEngine level)
        {
        }

        // 提示文案 key（子类可重写为自己的本地化字符串）。  
        protected virtual string IntroString => STRING_INTRO;
        protected virtual string BossIncomingString => STRING_BOSS_INCOMING;
        protected virtual string ProgressRestString => STRING_PROGRESS_REST;
        #endregion

        #region 生命周期  
        public override void Start(LevelEngine level)
        {
            base.Start(level);

            SetState(level, STATE_WARMUP);
            SetBossKilled(level, 0);
            SetBossIndex(level, 0);
            SetBossSeen(level, false);

            OnStageStart(level);

            level.SetEnemyPool(EnemyPool);
            level.ShowAdvice(LogicStrings.CONTEXT_ADVICE, IntroString, 1000, 300);
        }

        public override void Update(LevelEngine level)
        {
            base.Update(level);
            switch (GetState(level))
            {
                case STATE_WARMUP:
                    WarmupUpdate(level);
                    break;
                case STATE_FIGHTING:
                    RunBossWave(level);
                    FightingUpdate(level);
                    break;
                case STATE_RESTING:
                    RunBossWave(level);
                    RestingUpdate(level);
                    break;
            }
        }
        #endregion

        // ============ 预热阶段（跑满 a 波普通怪后出第一个 Boss） ============  
        private void WarmupUpdate(LevelEngine level)
        {
            if (level.CurrentWave < WarmupWaveCount)
                return;

            // 常驻 STATE_BOSS_FIGHT，使普通波次系统不再自行推进，之后完全由 RunBossWave 驱动出怪。  
            level.WaveState = VanillaLevelStates.STATE_BOSS_FIGHT;

            level.ShowAdvice(LogicStrings.CONTEXT_ADVICE, BossIncomingString, 1000, 200);
            SetBossIndex(level, 0);
            SpawnBoss(level, 0);
            SetBossSeen(level, false);
            SetState(level, STATE_FIGHTING);
        }

        // ============ 战斗阶段：检测当前 Boss 是否被击杀 ============  
        private void FightingUpdate(LevelEngine level)
        {
            if (IsBossAlive(level))
            {
                SetBossSeen(level, true);
                return;
            }
            if (!GetBossSeen(level))
                return;

            OnBossDefeated(level);
        }

        // ============ 一个 Boss 被击杀 ============  
        private void OnBossDefeated(LevelEngine level)
        {
            // 恢复默认音乐与关卡进度条（休息期间）。  
            var musicID = level.GetMusicID();
            if (musicID != null)
                level.PlayMusic(musicID);
            level.SetMusicVolume(1);
            level.SetProgressBarToStage();
            // 把已死亡但仍留在场上的 Boss（如正邪倒地小人）从 Boss 血条统计中排除，  
            // 否则下一个 Boss 的血条会把先前尸体的最大血量也算进分母，导致开场血条不满。  
            foreach (var deadBoss in level.FindEntities(e => e.IsEntityOf(BossID) && e.IsDead))
            {
                deadBoss.SetProperty(LogicBossProps.DONT_COUNT_BOSS_HP, true);
            }

            int bossKilled = GetBossKilled(level) + 1;
            SetBossKilled(level, bossKilled);
            RecordBossKills(level, bossKilled);

            // 下一个 Boss 的序号 = 已击杀数（0-based：第 1 个是 index 0，故下一个用 bossKilled）。  
            SetBossIndex(level, bossKilled);

            int rest = GetRestSeconds(bossKilled);
            level.ShowAdvice(LogicStrings.CONTEXT_ADVICE, ProgressRestString, 100, 300,
                bossKilled.ToString(), rest.ToString());
            StartRest(level, rest);

            SetBossSeen(level, false);
            SetState(level, STATE_RESTING);
        }

        // ============ 休息阶段：倒计时结束后生成下一个 Boss ============  
        private void RestingUpdate(LevelEngine level)
        {
            var timer = GetRestTimer(level);
            if (timer == null)
                return;
            timer.Run();
            if (!timer.Expired)
                return;

            SpawnBoss(level, GetBossIndex(level));
            SetBossSeen(level, false);
            SetState(level, STATE_FIGHTING);
        }

        // ============ 生成一个 Boss（生成 + 设定精确最大生命 + 登场特效 + 血条 + 音乐） ============  
        protected virtual void SpawnBoss(LevelEngine level, int bossIndex)
        {
            int maxLane = level.GetMaxLaneCount();
            int centerLane = maxLane / 2;

            var pos = new Vector3(LevelPositions.ENEMY_RIGHT_BORDER, 0, level.GetEntityLaneZ(centerLane));
            var boss = level.Spawn(BossID, pos, null);
            if (boss != null)
            {
                // 直接设定精确最大生命值（不走 ApplyBuffForBossRevenge 的 ×1.5，保证血量曲线可控）。  
                int health = GetBossHealth(bossIndex);
                boss.SetProperty(EngineEntityProps.MAX_HEALTH, (float)health);
                boss.Health = health;
                OnBossAppear(boss);
            }

            level.SetProgressBarToBoss(ProgressBarID);
            level.PlayMusic(BossMusic);
            level.SetMusicVolume(1);
        }

        // ============ 休息计时 ============  
        private void StartRest(LevelEngine level, int seconds)
        {
            SetRestTimer(level, new FrameTimer(Ticks.FromSeconds(seconds)));
        }

        // ============ 辅助 ============  
        private void RunBossWave(LevelEngine level)
        {
            level.GetStageBehaviour<WaveStageBehaviour>()?.RunBossWave(level);
        }
        protected bool IsBossAlive(LevelEngine level)
        {
            return level.EntityExists(e => e.IsEntityOf(BossID) && !e.IsDead && e.IsHostileEntity());
        }
        private void RecordBossKills(LevelEngine level, int kills)
        {
            if (Global.Saves.GetStat(LogicStats.CATEGORY_MAX_BOSS_KILLS, level.StageID) < kills)
            {
                Global.Saves.SetStat(LogicStats.CATEGORY_MAX_BOSS_KILLS, level.StageID, kills);
            }
        }

        // ============ 关卡属性存取 ============  
        private static int GetState(LevelEngine level) => level.GetProperty<int>(PROP_STATE);
        private static void SetState(LevelEngine level, int value) => level.SetProperty(PROP_STATE, value);
        private static int GetBossKilled(LevelEngine level) => level.GetProperty<int>(PROP_BOSS_KILLED);
        private static void SetBossKilled(LevelEngine level, int value) => level.SetProperty(PROP_BOSS_KILLED, value);
        private static int GetBossIndex(LevelEngine level) => level.GetProperty<int>(PROP_BOSS_INDEX);
        private static void SetBossIndex(LevelEngine level, int value) => level.SetProperty(PROP_BOSS_INDEX, value);
        private static bool GetBossSeen(LevelEngine level) => level.GetProperty<bool>(PROP_BOSS_SEEN);
        private static void SetBossSeen(LevelEngine level, bool value) => level.SetProperty(PROP_BOSS_SEEN, value);
        private static FrameTimer? GetRestTimer(LevelEngine level) => level.GetProperty<FrameTimer>(PROP_REST_TIMER);
        private static void SetRestTimer(LevelEngine level, FrameTimer value) => level.SetProperty(PROP_REST_TIMER, value);

        // ============ 状态 ============  
        protected const int STATE_WARMUP = 0;
        protected const int STATE_FIGHTING = 1;
        protected const int STATE_RESTING = 2;

        // 所有子类共用同一组关卡属性（同一时刻只运行一个关卡，无冲突）。  
        private const string PROP_REGION = "infinity_boss";
        [LevelPropertyRegistry(PROP_REGION)]
        public static readonly VanillaLevelPropertyMeta<int> PROP_STATE = new VanillaLevelPropertyMeta<int>("state");
        [LevelPropertyRegistry(PROP_REGION)]
        public static readonly VanillaLevelPropertyMeta<int> PROP_BOSS_KILLED = new VanillaLevelPropertyMeta<int>("boss_killed");
        [LevelPropertyRegistry(PROP_REGION)]
        public static readonly VanillaLevelPropertyMeta<int> PROP_BOSS_INDEX = new VanillaLevelPropertyMeta<int>("boss_index");
        [LevelPropertyRegistry(PROP_REGION)]
        public static readonly VanillaLevelPropertyMeta<bool> PROP_BOSS_SEEN = new VanillaLevelPropertyMeta<bool>("boss_seen");
        [LevelPropertyRegistry(PROP_REGION)]
        public static readonly VanillaLevelPropertyMeta<FrameTimer> PROP_REST_TIMER = new VanillaLevelPropertyMeta<FrameTimer>("rest_timer");

        // ============ 默认提示文案（子类可通过覆盖 IntroString 等替换） ============  
        [TranslateMsg("无限Boss提示")]
        public const string STRING_INTRO = "坚持发展数波后 Boss 将降临！击败尽可能多的 Boss 吧！";
        [TranslateMsg("无限Boss提示")]
        public const string STRING_BOSS_INCOMING = "Boss 降临！";
        [TranslateMsg("无限Boss提示，{0}为累计击杀Boss数，{1}为休息秒数")]
        public const string STRING_PROGRESS_REST = "已击败 {0} 个 Boss！休息 {1} 秒后下一个更强的 Boss 来袭！";
    }
}
