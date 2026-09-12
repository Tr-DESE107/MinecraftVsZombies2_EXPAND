#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using MukioI18n;
using MVZ2.GameContent.Bosses;
using MVZ2.GameContent.Buffs.Enemies;
using MVZ2.GameContent.Difficulties;
using MVZ2.Vanilla.Callbacks;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Level;
using MVZ2.Vanilla.Properties;
using MVZ2Logic;
using MVZ2Logic.Entities;
using MVZ2Logic.Level;
using MVZ2Logic.Localization;
using MVZ2Logic.Stats;
using PVZEngine;
using PVZEngine.Buffs;
using PVZEngine.Callbacks;
using PVZEngine.Entities;
using PVZEngine.Level;
using Tools;
using UnityEngine;

namespace MVZ2.GameContent.Stages
{
    // 无限 Boss 基类（InfinityBossBehaviour）：
    // 1) 开局先跑 WarmupWaveCount 波普通波次，之后切换为 Boss 战。
    // 2) 没有战役阶段，直接进入无尽：每轮生成【一个】Boss。
    // 3) 每击杀一个 Boss，弹出「Boss 增强选项」（类似梦魇命运），玩家从随机选项中为下一个 Boss 选择增强方向：
    //    血量增加 / 攻击力提升 / 限伤降低 / 再生提升，四项均为关卡级累积属性，子类均可定制幅度。
    // 4) 所有 Boss 生成时统一禁用自身限伤行为，改用本类挂载的统一限伤（初始 3600，随限伤选项逐渐降低）。
    // 5) 击杀的 Boss 数量记录到 CATEGORY_MAX_ENDLESS_FLAGS（小游戏界面「最高连胜」显示这个）。
    //
    // 子类可定制：Boss 是什么、出怪池、血量曲线、各增强选项幅度、休息时间曲线、开局跑几波、登场特效、血条/音乐、开场初始化、提示文案。
    public abstract class InfinityBossBehaviour : StageBehaviour
    {
        protected InfinityBossBehaviour(StageDefinition stageDef) : base(stageDef)
        {
            //EXPAND 统一限伤：对当前 Boss 挂载窗口累积限伤（替代各 Boss 自身的限伤行为）。
            //注意：此回调触发时以 entity.Type 作为 filter（见 VanillaEntityExt.TakeDamage），
            //Boss 的 Type 是 BOSS 而非 ENEMY，因此不能加 filter；目标筛选由 IsUpgradeTarget 完成。
            stageDef.AddTrigger(VanillaLevelCallbacks.PRE_ENTITY_TAKE_DAMAGE, PreBossTakeDamageCallback);
            stageDef.AddTrigger(VanillaLevelCallbacks.POST_ENTITY_TAKE_DAMAGE, PostBossTakeDamageCallback);
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

        // ============ EXPAND 增强选项幅度（子类均可覆盖） ============
        // 血量选项：每选一次增加的血量（默认沿用自动加强的步长）。
        protected virtual int BossHealthBonusPerChoice => BossHealthStep;
        // 攻击选项：每选一次提升的攻击倍率（0.25 = +25%；各 Boss 的攻击均从 DAMAGE 属性读取）。
        protected virtual float BossAttackBonusPerChoice => 0.25f;
        // 限伤选项：统一限伤的初始值、每次降低的幅度、下限、每帧恢复量（窗口限伤，参考原版限伤的 decay）。
        protected virtual float DamageLimitStart => 2400;
        protected virtual float DamageLimitStep => 600;
        protected virtual float DamageLimitMin => 600;
        protected virtual float DamageLimitDecay => 40;
        // 再生选项：每选一次增加的每帧回血量（30帧=1秒）。
        protected virtual float RegenBonusPerChoice => 0.5f;
        // 弹窗随机抽取的选项数量范围：上限由难度 buff 管理（INFINITY_BOSS_UPGRADE_OPTION_COUNT，
        // 默认普通 4，简单 +1=5，困难/EXP -1=3），每次弹出时在 [上限-2, 上限] 内随机。
        protected virtual (int min, int max) GetUpgradeOptionCountRange(LevelEngine level)
        {
            var max = level.GetInfinityBossUpgradeOptionCount();
            return (Mathf.Max(1, max - 2), max);
        }

        // Boss 血量：基础值 + 血量选项的累积加成，封顶 BossHealthMax。
        // （bossIndex 保留给需要的子类使用；血量不再随序号自动提升，改由选项驱动。）
        protected virtual int GetBossHealth(LevelEngine level, int bossIndex)
        {
            return Mathf.Min(BossHealthStart + GetHealthBonus(level), BossHealthMax);
        }

        // 休息时间曲线参数（秒）：首次休息、每击杀一个 Boss 递减、最低值。  
        protected virtual int FirstRestSeconds => 95;
        protected virtual int RestStepSeconds => 5;
        protected virtual int MinRestSeconds => 10;

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

            //EXPAND 初始化统一限伤与增强属性。
            SetDamageLimit(level, DamageLimitStart);
            SetUpgradeDamageCurrent(level, 0);

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
            //EXPAND 统一限伤的窗口累积随时间恢复（与原版限伤的 decay 一致）。
            var damageCurrent = GetUpgradeDamageCurrent(level);
            if (damageCurrent > 0)
                SetUpgradeDamageCurrent(level, Mathf.Max(0, damageCurrent - DamageLimitDecay));
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

            //EXPAND 弹出 Boss 增强选项（暂停游戏，玩家选择后累积增强并恢复）。
            ShowUpgradeChoice(level);

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

        // ============ 生成一个 Boss（生成 + 设定精确最大生命 + 登场特效 + 血条 + 音乐 + 应用增强） ============  
        protected virtual void SpawnBoss(LevelEngine level, int bossIndex)
        {
            int maxLane = level.GetMaxLaneCount();
            int centerLane = maxLane / 2;

            var pos = new Vector3(LevelPositions.ENEMY_RIGHT_BORDER, 0, level.GetEntityLaneZ(centerLane));
            var boss = level.Spawn(BossID, pos, null);
            if (boss != null)
            {
                // 直接设定精确最大生命值（不走 ApplyBuffForBossRevenge 的 ×1.5，保证血量曲线可控）。  
                int health = GetBossHealth(level, bossIndex);
                boss.SetProperty(EngineEntityProps.MAX_HEALTH, (float)health);
                boss.Health = health;
                //EXPAND 应用增强选项的累积增强（统一限伤、攻击倍率、外挂再生等）。
                ApplyBossUpgrades(level, boss);
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

        // ============ EXPAND Boss 增强选项系统 ============
        // 增强方向。
        private const int UPGRADE_HEALTH = 0;
        private const int UPGRADE_ATTACK = 1;
        private const int UPGRADE_DAMAGE_LIMIT = 2;
        private const int UPGRADE_REGEN = 3;

        // 击杀 Boss 后弹出增强选择：暂停游戏 → 随机抽取选项 → 玩家选择 → 累积增强并恢复。
        private void ShowUpgradeChoice(LevelEngine level)
        {
            // 构建可用选项池：血量封顶 / 限伤到底后从池中排除（攻击与再生可无限累积）。
            var pool = new List<int>();
            if (BossHealthStart + GetHealthBonus(level) < BossHealthMax)
                pool.Add(UPGRADE_HEALTH);
            pool.Add(UPGRADE_ATTACK);
            if (GetDamageLimit(level) > DamageLimitMin)
                pool.Add(UPGRADE_DAMAGE_LIMIT);
            pool.Add(UPGRADE_REGEN);

            //使用关卡自带的 RNG（带种子、可序列化），保证存档重载/回放时抽到的选项一致。
            var rng = level.CreateRNG();
            //选项数量随难度在范围内随机（NextIntRange 不含上限，故 max + 1），
            //并钳制到 [1, 可用选项数]：可用选项不足范围上限时全给，不会溢出。
            var (minCount, maxCount) = GetUpgradeOptionCountRange(level);
            var count = Mathf.Clamp(rng.Next(minCount, maxCount + 1), 1, pool.Count);
            var selected = pool.RandomTake(count, rng).ToArray();
            var texts = selected.Select(o => GetUpgradeText(level, o)).ToArray();

            level.PauseGame(100);
            var title = Global.Localization.GetText(STRING_UPGRADE_TITLE);
            var desc = Global.Localization.GetText(STRING_UPGRADE_DESC);
            level.ShowDialog(title, desc, texts, i =>
            {
                ApplyUpgrade(level, selected[i]);
                level.ResumeGameDelayed(100);
            });
        }

        // 执行选中的增强：写入关卡累积属性，下一个 Boss 生成时应用。
        private void ApplyUpgrade(LevelEngine level, int option)
        {
            switch (option)
            {
                case UPGRADE_HEALTH:
                    SetHealthBonus(level, GetHealthBonus(level) + BossHealthBonusPerChoice);
                    break;
                case UPGRADE_ATTACK:
                    SetAttackBonus(level, GetAttackBonus(level) + BossAttackBonusPerChoice);
                    break;
                case UPGRADE_DAMAGE_LIMIT:
                    SetDamageLimit(level, Mathf.Max(DamageLimitMin, GetDamageLimit(level) - DamageLimitStep));
                    break;
                case UPGRADE_REGEN:
                    SetRegenAmount(level, GetRegenAmount(level) + RegenBonusPerChoice);
                    break;
            }
        }

        // 选项文本（带当前数值变化）。
        private string GetUpgradeText(LevelEngine level, int option)
        {
            switch (option)
            {
                case UPGRADE_HEALTH:
                    return Global.Localization.GetText(STRING_UPGRADE_HEALTH, BossHealthBonusPerChoice);
                case UPGRADE_ATTACK:
                    return Global.Localization.GetText(STRING_UPGRADE_ATTACK, Mathf.RoundToInt(BossAttackBonusPerChoice * 100));
                case UPGRADE_DAMAGE_LIMIT:
                    return Global.Localization.GetText(STRING_UPGRADE_DAMAGE_LIMIT, Mathf.Max(DamageLimitMin, GetDamageLimit(level) - DamageLimitStep));
                case UPGRADE_REGEN:
                    return Global.Localization.GetText(STRING_UPGRADE_REGEN, RegenBonusPerChoice * 30);
                default:
                    return "";
            }
        }

        // 对生成的 Boss 应用累积增强。
        // 血量由 GetBossHealth 处理；本方法处理统一限伤、攻击倍率、外挂再生。
        protected virtual void ApplyBossUpgrades(LevelEngine level, Entity boss)
        {
            // 统一禁用 Boss 自身的限伤行为（含正邪的闪避布与巨人的可塑性减伤变体）。
            BossResistance.SetDisabled(boss, true);
            // 重置统一限伤的窗口累积（每个新 Boss 从零开始）。
            SetUpgradeDamageCurrent(level, 0);
            // 攻击力：按累积倍率提升（各 Boss 的弹幕/火息等伤害均从 DAMAGE 属性读取）。
            var attackBonus = GetAttackBonus(level);
            if (attackBonus > 0)
                boss.SetDamage(boss.GetDamage() * (1 + attackBonus));
            // 再生：外挂 RegenerationBuff（超长超时等效常驻；凋灵自身再生已由子类 OnStageStart 关闭）。
            var regen = GetRegenAmount(level);
            if (regen > 0)
            {
                var buff = boss.AddBuff<RegenerationBuff>();
                if (buff != null)
                {
                    buff.SetProperty(RegenerationBuff.PROP_HEAL_AMOUNT, regen);
                    buff.SetProperty(RegenerationBuff.PROP_TIMEOUT, int.MaxValue);
                }
            }
        }

        // ============ EXPAND 统一限伤（替代各 Boss 自身的限伤行为） ============
        // 是否为增强/限伤的生效目标（多形态 Boss 如梦魇可覆盖以包含全部形态）。
        protected virtual bool IsUpgradeTarget(LevelEngine level, Entity entity)
        {
            return entity.IsEntityOf(BossID) && !entity.IsDead;
        }
        // 窗口累积限伤：已受伤量 + 本次伤害不超过当前限伤值（与原版 BossResistance 同机制）。
        private void PreBossTakeDamageCallback(VanillaLevelCallbacks.PreTakeDamageParams param, CallbackResult result)
        {
            var input = param.input;
            var entity = input.Entity;
            var level = entity.Level;
            if (!level.HasBehaviour(this))
                return;
            if (!IsUpgradeTarget(level, entity))
                return;

            var max = GetDamageLimit(level);
            var current = GetUpgradeDamageCurrent(level);
            var limit = Mathf.Max(0, max - current);
            if (input.Amount > limit)
                input.SetAmount(limit);
        }
        private void PostBossTakeDamageCallback(VanillaLevelCallbacks.PostTakeDamageParams param, CallbackResult result)
        {
            var output = param.output;
            var entity = output.Entity;
            var level = entity.Level;
            if (!level.HasBehaviour(this))
                return;
            if (!IsUpgradeTarget(level, entity))
                return;

            float amount = 0;
            foreach (var r in output.GetAllResults())
                amount += r.Amount;
            SetUpgradeDamageCurrent(level, GetUpgradeDamageCurrent(level) + amount);
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

        // ============ EXPAND 增强选项的关卡属性存取 ============
        private static int GetHealthBonus(LevelEngine level) => level.GetProperty<int>(PROP_HEALTH_BONUS);
        private static void SetHealthBonus(LevelEngine level, int value) => level.SetProperty(PROP_HEALTH_BONUS, value);
        private static float GetAttackBonus(LevelEngine level) => level.GetProperty<float>(PROP_ATTACK_BONUS);
        private static void SetAttackBonus(LevelEngine level, float value) => level.SetProperty(PROP_ATTACK_BONUS, value);
        private static float GetDamageLimit(LevelEngine level) => level.GetProperty<float>(PROP_DAMAGE_LIMIT);
        private static void SetDamageLimit(LevelEngine level, float value) => level.SetProperty(PROP_DAMAGE_LIMIT, value);
        private static float GetRegenAmount(LevelEngine level) => level.GetProperty<float>(PROP_REGEN_AMOUNT);
        private static void SetRegenAmount(LevelEngine level, float value) => level.SetProperty(PROP_REGEN_AMOUNT, value);
        private static float GetUpgradeDamageCurrent(LevelEngine level) => level.GetProperty<float>(PROP_DAMAGE_CURRENT);
        private static void SetUpgradeDamageCurrent(LevelEngine level, float value) => level.SetProperty(PROP_DAMAGE_CURRENT, value);

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

        // ============ EXPAND 增强选项的关卡属性（所有子类共用；同一时刻只运行一个关卡，无冲突） ============
        private const string UPGRADE_REGION = "infinity_boss_upgrade";
        [LevelPropertyRegistry(UPGRADE_REGION)]
        public static readonly VanillaLevelPropertyMeta<int> PROP_HEALTH_BONUS = new VanillaLevelPropertyMeta<int>("health_bonus");
        [LevelPropertyRegistry(UPGRADE_REGION)]
        public static readonly VanillaLevelPropertyMeta<float> PROP_ATTACK_BONUS = new VanillaLevelPropertyMeta<float>("attack_bonus");
        [LevelPropertyRegistry(UPGRADE_REGION)]
        public static readonly VanillaLevelPropertyMeta<float> PROP_DAMAGE_LIMIT = new VanillaLevelPropertyMeta<float>("damage_limit");
        [LevelPropertyRegistry(UPGRADE_REGION)]
        public static readonly VanillaLevelPropertyMeta<float> PROP_REGEN_AMOUNT = new VanillaLevelPropertyMeta<float>("regen_amount");
        [LevelPropertyRegistry(UPGRADE_REGION)]
        public static readonly VanillaLevelPropertyMeta<float> PROP_DAMAGE_CURRENT = new VanillaLevelPropertyMeta<float>("damage_current");

        // ============ EXPAND 增强选项的本地化 Key ============
        [TranslateMsg("无限Boss增强标题")]
        public const string STRING_UPGRADE_TITLE = "<color=red>选择一个加强</color>";
        [TranslateMsg("无限Boss增强描述")]
        public const string STRING_UPGRADE_DESC = "<color=red>选吧</color>";
        [TranslateMsg("无限Boss增强选项，{0}为增加的血量")]
        public const string STRING_UPGRADE_HEALTH = "生命上限 +{0}";
        [TranslateMsg("无限Boss增强选项，{0}为百分比")]
        public const string STRING_UPGRADE_ATTACK = "攻击力 +{0}%";
        [TranslateMsg("无限Boss增强选项，{0}为新的限伤值")]
        public const string STRING_UPGRADE_DAMAGE_LIMIT = "承伤上限降至 {0}";
        [TranslateMsg("无限Boss增强选项，{0}为每秒回血量")]
        public const string STRING_UPGRADE_REGEN = "再生 +{0}/秒";

        // ============ 默认提示文案（子类可通过覆盖 IntroString 等替换） ============  
        [TranslateMsg("无限Boss提示")]
        public const string STRING_INTRO = "坚持发展数波后 Boss 将降临！击败尽可能多的 Boss 吧！";
        [TranslateMsg("无限Boss提示")]
        public const string STRING_BOSS_INCOMING = "Boss 降临！";
        [TranslateMsg("无限Boss提示，{0}为累计击杀Boss数，{1}为休息秒数")]
        public const string STRING_PROGRESS_REST = "已击败 {0} 个 Boss！休息 {1} 秒后下一个更强的 Boss 来袭！";
    }
}
