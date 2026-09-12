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

        protected override int BossHealthStart => 3000;
        protected override int BossHealthStep => 1000;
        protected override int BossHealthMax => 90000;

        protected override int FirstRestSeconds => 90;
        protected override int RestStepSeconds => 5;
        protected override int MinRestSeconds => 12;

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
