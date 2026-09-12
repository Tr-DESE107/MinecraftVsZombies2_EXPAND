#nullable enable

using MukioI18n;
using MVZ2.GameContent.Bosses;
using MVZ2.GameContent.Enemies;
using MVZ2.GameContent.ProgressBars;
using MVZ2.Vanilla.Audios;
using PVZEngine;
using PVZEngine.Entities;
using PVZEngine.Level;

namespace MVZ2.GameContent.Stages
{
    // 无限弗兰肯斯坦（InfinityFrankenstein）：基于 InfinityBossBehaviour。  
    // 弗兰肯斯坦在 Init 内自动进入 WAKING(APPEAR) 状态，故无需重写 OnBossAppear。  
    public class InfinityFrankensteinBehaviour : InfinityBossBehaviour
    {
        public InfinityFrankensteinBehaviour(StageDefinition stageDef) : base(stageDef)
        {
        }

        protected override NamespaceID BossID => VanillaBossID.frankenstein;
        protected override NamespaceID[] EnemyPool => enemyPool;
        protected override NamespaceID ProgressBarID => VanillaProgressBarID.frankenstein;
        protected override NamespaceID BossMusic => VanillaMusicID.halloweenBoss;

        protected override int WarmupWaveCount => 10;

        protected override int BossHealthStart => 15000;
        protected override int BossHealthStep => 5000;
        protected override int BossHealthMax => 100000;

        protected override int FirstRestSeconds => 95;
        protected override int RestStepSeconds => 5;
        protected override int MinRestSeconds => 15;

        // ============ EXPAND 增强选项幅度（先用基类默认值占位，便于后续逐Boss调整） ============
        protected override int BossHealthBonusPerChoice => 5000; // 血量选项：每次增加的血量（默认= BossHealthStep）
        protected override float BossAttackBonusPerChoice => 0.25f; // 攻击选项：每次攻击倍率 +25%
        protected override float DamageLimitStart => 2400; // 限伤选项：统一限伤初始值
        protected override float DamageLimitStep => 600; // 限伤选项：每次降低量
        protected override float DamageLimitMin => 600; // 限伤选项：下限
        protected override float DamageLimitDecay => 40; // 限伤选项：窗口每帧恢复量
        protected override float RegenBonusPerChoice => 0.5f; // 再生选项：每次增加的每帧回血（30帧=1秒）

        protected override string IntroString => STRING_INTRO;
        protected override string BossIncomingString => STRING_INCOMING;
        protected override string ProgressRestString => STRING_PROGRESS_REST;

        // ============ 出怪池（占位，请按主题调整） ============  
        private static readonly NamespaceID[] enemyPool = new NamespaceID[]
        {
            VanillaEnemyID.zombie,
            VanillaEnemyID.ZombieHead,
            VanillaEnemyID.leatherCappedZombie,
            VanillaEnemyID.ironHelmettedZombie,
            VanillaEnemyID.skeleton,
            VanillaEnemyID.ghost,
            VanillaEnemyID.mummy,
            VanillaEnemyID.necromancer,
            VanillaEnemyID.skeletonHorse,
            VanillaEnemyID.MegaZombie,
            VanillaEnemyID.RedEyeZombieHead,
            VanillaEnemyID.KingSkeleton,
            VanillaEnemyID.SuperMegaZombie,
            VanillaEnemyID.EvilMage,
        };

        // ============ 提示条本地化 Key ============  
        [TranslateMsg("无限科学怪人提示")]
        public const string STRING_INTRO = "坚持发展 10 波后科学怪人将降临！击败尽可能多的科学怪人吧！";
        [TranslateMsg("无限科学怪人提示")]
        public const string STRING_INCOMING = "科学怪人降临！";
        [TranslateMsg("无限科学怪人提示，{0}为累计击杀数，{1}为休息秒数")]
        public const string STRING_PROGRESS_REST = "已击败 {0} 个科学怪人！休息 {1} 秒后更强的敌人来袭！";
    }
}
