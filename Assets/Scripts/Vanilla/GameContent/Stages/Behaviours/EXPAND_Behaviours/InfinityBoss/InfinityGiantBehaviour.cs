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
    // 无限巨人（InfinityGiant）：基于 InfinityBossBehaviour。  
    // 巨人在 Init 内自动进入 IDLE 状态，无公开 Appear，故不重写 OnBossAppear。  
    public class InfinityGiantBehaviour : InfinityBossBehaviour  
    {  
        public InfinityGiantBehaviour(StageDefinition stageDef) : base(stageDef)  
        {  
        }  
  
        protected override NamespaceID BossID => VanillaBossID.theGiant;  
        protected override NamespaceID[] EnemyPool => enemyPool;  
        protected override NamespaceID ProgressBarID => VanillaProgressBarID.theGiant;  
        protected override NamespaceID BossMusic => VanillaMusicID.palaceBoss; // TODO: 替换为巨人专属音乐  
  
        protected override int WarmupWaveCount => 10;  
  
        protected override int BossHealthStart => 6000;  
        protected override int BossHealthStep => 2000;  
        protected override int BossHealthMax => 150000;  
  
        protected override int FirstRestSeconds => 95;  
        protected override int RestStepSeconds => 4;  
        protected override int MinRestSeconds => 15;  
  
        protected override string IntroString => STRING_INTRO;  
        protected override string BossIncomingString => STRING_INCOMING;  
        protected override string ProgressRestString => STRING_PROGRESS_REST;  
  
        // ============ 出怪池（占位，请按主题调整） ============  
        private static readonly NamespaceID[] enemyPool = new NamespaceID[]  
        {  
            VanillaEnemyID.MonkZombie,  
            VanillaEnemyID.LeatherMonkZombie,  
            VanillaEnemyID.IronMonkZombie,  
            VanillaEnemyID.reflectiveBarrierZombie,  
            VanillaEnemyID.wickedHermitZombie,  
            VanillaEnemyID.shikaisenZombie,  
            VanillaEnemyID.emperorZombie,  
            VanillaEnemyID.TorchKongfuZombie,  
            VanillaEnemyID.Hemperor,  
            VanillaEnemyID.FlyingPot,  
        };  
  
        // ============ 提示条本地化 Key ============  
        [TranslateMsg("无限巨人提示")]  
        public const string STRING_INTRO = "坚持发展 10 波后巨人将降临！击败尽可能多的巨人吧！";  
        [TranslateMsg("无限巨人提示")]  
        public const string STRING_INCOMING = "巨人降临！";  
        [TranslateMsg("无限巨人提示，{0}为累计击杀数，{1}为休息秒数")]  
        public const string STRING_PROGRESS_REST = "已击败 {0} 个巨人！休息 {1} 秒后更强的巨人来袭！";  
    }  
}
