#nullable enable  
  
using MukioI18n;  
using MVZ2.GameContent.Bosses;  
using MVZ2.GameContent.Damages;  
using MVZ2.GameContent.Difficulties;   // WITHER_REGENERATION  
using MVZ2.GameContent.Enemies;  
using MVZ2.GameContent.ProgressBars;  
using MVZ2.Vanilla.Audios;  
using MVZ2.Vanilla.Bosses;  
using MVZ2.Vanilla.Entities;            // SetMaxHealth 扩展  
using MVZ2.Vanilla.Level;  
using MVZ2.Vanilla.Properties;  
using MVZ2Logic;  
using MVZ2Logic.Entities;  
using MVZ2Logic.Level;  
using MVZ2Logic.Localization;  
using MVZ2Logic.Stats;  
using PVZEngine;  
using PVZEngine.Damages;  
using PVZEngine.Entities;  
using PVZEngine.Level;  
using Tools;  
using UnityEngine;  
  
namespace MVZ2.GameContent.Stages  
{  
    // 无限凋灵（InfinityWither）：  
    // 1) 开局先跑 5 波普通波次，到达旗帜波后切换为 Boss 战并生成第一组凋灵。  
    // 2) 战役阶段共 10 只凋灵，按「阶梯分组」一次性生成越来越多：{1,1,1,2,2,3}。  
    //    每清空一组给玩家一段休息时间（45/45/50/50/50 秒）。  
    // 3) 累计击杀 10 只即通关并记录难度，随后进入无尽阶段。  
    // 4) 无尽阶段：进入先休息 60 秒再出怪，之后每轮休息秒数逐渐减少至最低 10 秒；  
    //    每通过两轮，单波生成的凋灵 +1，最高 10 只。  
    // 本关凋灵：关闭血量再生；最大生命值随波次提升（战役 6000→18000，无尽 6000→封顶 36000）。  
    public class InfinityWitherBehaviour : StageBehaviour  
    {  
        public InfinityWitherBehaviour(StageDefinition stageDef) : base(stageDef)  
        {  
        }  
  
        // ============ 可调参数 ============  
        private const int WARMUP_WAVE_COUNT = 5;             // 进入 Boss 战前先跑的普通波数（旗帜波）  
        private const int KILLS_TO_CLEAR = 10;               // 战役通关所需击杀数  
  
        private static readonly int[] campaignSpawnCounts = new int[] { 1, 1, 1, 2, 2, 3 }; // 合计 10  
        private static readonly int[] campaignRestSeconds = new int[] { 45, 45, 50, 50, 50 };  
  
        private const int ENDLESS_FIRST_REST = 60;  
        private const int ENDLESS_MIN_REST = 10;  
        private const int ENDLESS_REST_STEP = 5;  
        private const int ENDLESS_MAX_SPAWN = 10;  
  
        // 凋灵最大生命值曲线。  
        private const int CAMPAIGN_HP_START = 6000;          // 战役第 1 组  
        private const int CAMPAIGN_HP_END = 18000;           // 战役第 6 组  
        private const int ENDLESS_HP_START = 6000;           // 无尽第 1 轮（重新从 6000 开始）  
        private const int ENDLESS_HP_STEP = 2000;            // 无尽每轮生命值增量  
        private const int ENDLESS_HP_MAX = 36000;            // 无尽生命值上限  
  
        // ============ 状态 ============  
        private const int STATE_WARMUP = 0;  
        private const int STATE_FIGHTING = 1;  
        private const int STATE_RESTING = 2;  
  
        // ============ 生命周期 ============  
        public override void Start(LevelEngine level)  
        {  
            base.Start(level);  
  
            SetState(level, STATE_WARMUP);  
            SetKillCount(level, 0);  
            SetGroupIndex(level, 0);  
            SetEndlessRound(level, 0);  
            SetCleared(level, false);  
            SetBossSeen(level, false);  
  
            // 关闭本关凋灵的血量再生（Wither.UpdateLogic 读取该属性作为每帧回血量）。  
            level.SetProperty(VanillaDifficultyLevelProps.WITHER_REGENERATION, 0f);  
  
            level.SetEnemyPool(witherPool);  
            level.ShowAdvice(LogicStrings.CONTEXT_ADVICE, STRING_INTRO, 1000, 300);  
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
  
        // ============ 预热阶段（跑满 5 波普通怪，到达旗帜波） ============  
        private void WarmupUpdate(LevelEngine level)  
        {  
            if (level.CurrentWave < WARMUP_WAVE_COUNT)  
                return;  
  
            level.WaveState = VanillaLevelStates.STATE_BOSS_FIGHT;  
  
            level.ShowAdvice(LogicStrings.CONTEXT_ADVICE, STRING_WITHER_INCOMING, 1000, 200);  
            SetGroupIndex(level, 0);  
            SpawnWithers(level, campaignSpawnCounts[0], GetCurrentMaxHealth(level));  
            SetBossSeen(level, false);  
            SetState(level, STATE_FIGHTING);  
        }  
  
        // ============ 战斗阶段：检测本组凋灵是否被全部击杀 ============  
        private void FightingUpdate(LevelEngine level)  
        {  
            if (IsWitherAlive(level))  
            {  
                SetBossSeen(level, true);  
                return;  
            }  
            if (!GetBossSeen(level))  
                return;  
  
            OnGroupCleared(level);  
        }  
  
        // ============ 一组凋灵被清空 ============  
        private void OnGroupCleared(LevelEngine level)  
        {  
            // 恢复默认音乐与关卡进度条（休息期间）。  
            var musicID = level.GetMusicID();  
            if (musicID != null)  
                level.PlayMusic(musicID);  
            level.SetMusicVolume(1);  
            level.SetProgressBarToStage();  
  
            if (!GetCleared(level))  
            {  
                // -------- 战役阶段 --------  
                int groupIndex = GetGroupIndex(level);  
                int kills = GetKillCount(level) + campaignSpawnCounts[groupIndex];  
                SetKillCount(level, kills);  
  
                int nextGroup = groupIndex + 1;  
                if (nextGroup >= campaignSpawnCounts.Length)  
                {  
                    // 达成 10 杀 -> 通关、记录难度，进入无尽阶段（先休息 60 秒）。  
                    SetCleared(level, true);  
                    RecordDifficulty(level, kills);  
                    SetEndlessRound(level, 0);  
  
                    // 通关提示 + 休息时间合并为一条，避免互相顶掉。  
                    level.ShowAdvice(LogicStrings.CONTEXT_ADVICE, STRING_CLEARED_REST, 1000, 300,  
                        ENDLESS_FIRST_REST.ToString());  
                    StartRest(level, ENDLESS_FIRST_REST);  
                }  
                else  
                {  
                    int rest = campaignRestSeconds[groupIndex];  
                    SetGroupIndex(level, nextGroup);  
  
                    // 击杀进度 + 休息时间合并为一条提示。  
                    level.ShowAdvice(LogicStrings.CONTEXT_ADVICE, STRING_PROGRESS_REST, 100, 300,  
                        kills.ToString(), KILLS_TO_CLEAR.ToString(), rest.ToString());  
                    StartRest(level, rest);  
                }  
            }  
            else  
            {  
                // -------- 无尽阶段 --------  
                int round = GetEndlessRound(level);  
                int kills = GetKillCount(level) + EndlessSpawnCount(round);  
                SetKillCount(level, kills);  
                RecordDifficulty(level, kills);  
  
                int nextRound = round + 1;  
                SetEndlessRound(level, nextRound);  
                int rest = EndlessRestSeconds(nextRound);  
  
                // 无尽累计击杀 + 休息时间合并为一条提示。  
                level.ShowAdvice(LogicStrings.CONTEXT_ADVICE, STRING_ENDLESS_PROGRESS_REST, 100, 300,  
                    kills.ToString(), rest.ToString());  
                StartRest(level, rest);  
            }  
  
            SetBossSeen(level, false);  
            SetState(level, STATE_RESTING);  
        }  
  
        // ============ 休息阶段：倒计时结束后生成下一波凋灵 ============  
        private void RestingUpdate(LevelEngine level)  
        {  
            var timer = GetRestTimer(level);  
            if (timer == null)  
                return;  
            timer.Run();  
            if (!timer.Expired)  
                return;  
  
            int count = GetCleared(level)  
                ? EndlessSpawnCount(GetEndlessRound(level))  
                : campaignSpawnCounts[GetGroupIndex(level)];  
  
            SpawnWithers(level, count, GetCurrentMaxHealth(level));  
            SetBossSeen(level, false);  
            SetState(level, STATE_FIGHTING);  
        }  
  
        // ============ 生成凋灵（生成 + 设定最大生命 + 登场动画 + 血条 + 音乐） ============  
        private void SpawnWithers(LevelEngine level, int count, int maxHealth)  
        {  
            int maxLane = level.GetMaxLaneCount();  
            int centerLane = maxLane / 2;  
  
            for (int i = 0; i < count; i++)  
            {  
                int lane = centerLane;  
                if (i > 0)  
                {  
                    int offsetLength = (i + 1) / 2;  
                    int offsetDir = ((i + 1) % 2) * 2 - 1;  
                    lane = Mathf.Clamp(centerLane + offsetDir * offsetLength, 0, maxLane - 1);  
                }  
  
                var pos = new Vector3(LevelPositions.ENEMY_RIGHT_BORDER, 0, level.GetEntityLaneZ(lane));  
                var boss = level.Spawn(VanillaBossID.wither, pos, null);  
                if (boss == null)  
                    continue;

                // 设定本波凋灵的最大生命值（直接改属性基础值），并把当前血量设满。  
                boss.SetProperty(EngineEntityProps.MAX_HEALTH, (float)maxHealth);
                boss.Health = maxHealth;

                // 播放凋灵生成动画（内部会播 witherSpawn 生成音效）。  
                // 注意：本关要求精确的生命数值，故不再调用 ApplyBuffForBossRevenge（会额外 ×1.5）。  
                Wither.Appear(boss);  
            }  
  
            level.SetProgressBarToBoss(VanillaProgressBarID.wither);  
            level.PlayMusic(VanillaMusicID.witherBoss);  
            level.SetMusicVolume(1);  
        }  
  
        // ============ 数值公式 ============  
        private static int EndlessSpawnCount(int round) => Mathf.Min(1 + round / 2, ENDLESS_MAX_SPAWN);  
        private static int EndlessRestSeconds(int round) => Mathf.Max(ENDLESS_FIRST_REST - ENDLESS_REST_STEP * round, ENDLESS_MIN_REST);  
  
        // 当前应生成凋灵的最大生命值：战役按组线性 6000→18000；无尽从 6000 每轮 +2000 封顶 36000。  
        private static int GetCurrentMaxHealth(LevelEngine level)  
        {  
            if (!GetCleared(level))  
            {  
                int gi = GetGroupIndex(level);  
                int last = campaignSpawnCounts.Length - 1; // = 5  
                float t = last <= 0 ? 0f : (float)gi / last;  
                return Mathf.RoundToInt(Mathf.Lerp(CAMPAIGN_HP_START, CAMPAIGN_HP_END, t));  
            }  
            int round = GetEndlessRound(level);  
            return Mathf.Min(ENDLESS_HP_START + ENDLESS_HP_STEP * round, ENDLESS_HP_MAX);  
        }  
  
        // ============ 休息计时（只计时，不再显示提示；提示已在 OnGroupCleared 合并显示） ============  
        private void StartRest(LevelEngine level, int seconds)  
        {  
            SetRestTimer(level, new FrameTimer(Ticks.FromSeconds(seconds)));  
        }  
  
        // ============ 出怪池 ============  
        private static readonly NamespaceID[] witherPool = new NamespaceID[]  
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
            VanillaEnemyID.AngryReverser,  
            VanillaEnemyID.RaiderSkull,  
            VanillaEnemyID.Anubiskull,  
            VanillaEnemyID.WitherSkeletonHorse,  
            VanillaEnemyID.AssaultDullahan,  
            VanillaEnemyID.KingofReverser,  
            VanillaEnemyID.WintherMage,  
            VanillaEnemyID.NetherTroopCarrier,  
        };  
  
        // ============ 辅助 ============  
        private void RunBossWave(LevelEngine level)  
        {  
            level.GetStageBehaviour<WaveStageBehaviour>()?.RunBossWave(level);  
        }  
        private static bool IsWitherAlive(LevelEngine level)  
        {  
            return level.EntityExists(e => e.IsEntityOf(VanillaBossID.wither) && !e.IsDead && e.IsHostileEntity());  
        }  
        private void RecordDifficulty(LevelEngine level, int difficulty)  
        {  
            if (Global.Saves.GetStat(LogicStats.CATEGORY_MAX_ENDLESS_FLAGS, level.StageID) < difficulty)  
            {  
                Global.Saves.SetStat(LogicStats.CATEGORY_MAX_ENDLESS_FLAGS, level.StageID, difficulty);  
            }  
        }  
  
        // ============ 关卡属性存取 ============  
        private static int GetState(LevelEngine level) => level.GetProperty<int>(PROP_STATE);  
        private static void SetState(LevelEngine level, int value) => level.SetProperty(PROP_STATE, value);  
        private static int GetKillCount(LevelEngine level) => level.GetProperty<int>(PROP_KILL_COUNT);  
        private static void SetKillCount(LevelEngine level, int value) => level.SetProperty(PROP_KILL_COUNT, value);  
        private static int GetGroupIndex(LevelEngine level) => level.GetProperty<int>(PROP_GROUP_INDEX);  
        private static void SetGroupIndex(LevelEngine level, int value) => level.SetProperty(PROP_GROUP_INDEX, value);  
        private static int GetEndlessRound(LevelEngine level) => level.GetProperty<int>(PROP_ENDLESS_ROUND);  
        private static void SetEndlessRound(LevelEngine level, int value) => level.SetProperty(PROP_ENDLESS_ROUND, value);  
        private static bool GetBossSeen(LevelEngine level) => level.GetProperty<bool>(PROP_BOSS_SEEN);  
        private static void SetBossSeen(LevelEngine level, bool value) => level.SetProperty(PROP_BOSS_SEEN, value);  
        private static bool GetCleared(LevelEngine level) => level.GetProperty<bool>(PROP_CLEARED);  
        private static void SetCleared(LevelEngine level, bool value) => level.SetProperty(PROP_CLEARED, value);  
        private static FrameTimer? GetRestTimer(LevelEngine level) => level.GetProperty<FrameTimer>(PROP_REST_TIMER);  
        private static void SetRestTimer(LevelEngine level, FrameTimer value) => level.SetProperty(PROP_REST_TIMER, value);  
  
        private const string PROP_REGION = "infinity_wither";  
        [LevelPropertyRegistry(PROP_REGION)]  
        public static readonly VanillaLevelPropertyMeta<int> PROP_STATE = new VanillaLevelPropertyMeta<int>("state");  
        [LevelPropertyRegistry(PROP_REGION)]  
        public static readonly VanillaLevelPropertyMeta<int> PROP_KILL_COUNT = new VanillaLevelPropertyMeta<int>("kill_count");  
        [LevelPropertyRegistry(PROP_REGION)]  
        public static readonly VanillaLevelPropertyMeta<int> PROP_GROUP_INDEX = new VanillaLevelPropertyMeta<int>("group_index");  
        [LevelPropertyRegistry(PROP_REGION)]  
        public static readonly VanillaLevelPropertyMeta<int> PROP_ENDLESS_ROUND = new VanillaLevelPropertyMeta<int>("endless_round");  
        [LevelPropertyRegistry(PROP_REGION)]  
        public static readonly VanillaLevelPropertyMeta<bool> PROP_BOSS_SEEN = new VanillaLevelPropertyMeta<bool>("boss_seen");  
        [LevelPropertyRegistry(PROP_REGION)]  
        public static readonly VanillaLevelPropertyMeta<bool> PROP_CLEARED = new VanillaLevelPropertyMeta<bool>("cleared");  
        [LevelPropertyRegistry(PROP_REGION)]  
        public static readonly VanillaLevelPropertyMeta<FrameTimer> PROP_REST_TIMER = new VanillaLevelPropertyMeta<FrameTimer>("rest_timer");  
  
        // ============ 提示条本地化 Key ============  
        [TranslateMsg("无限凋灵提示")]  
        public const string STRING_INTRO = "坚持发展 5 波后凋灵将降临！累计击杀 10 只即可通关，随后进入无尽挑战！";  
        [TranslateMsg("无限凋灵提示")]  
        public const string STRING_WITHER_INCOMING = "旗帜波来袭——凋灵降临！";  
        // 击杀进度与休息时间合并显示，避免同上下文提示互相覆盖。  
        [TranslateMsg("无限凋灵提示")]  
        public const string STRING_PROGRESS_REST = "已击杀 {0}/{1} 只凋灵！休息 {2} 秒后下一波来袭！";  
        [TranslateMsg("无限凋灵提示")]  
        public const string STRING_CLEARED_REST = "通关成功！难度已记录。休息 {0} 秒后进入无尽挑战！";  
        [TranslateMsg("无限凋灵提示")]  
        public const string STRING_ENDLESS_PROGRESS_REST = "无尽模式：已累计击杀 {0} 只凋灵！休息 {1} 秒后下一波来袭！";  
    }  
}
