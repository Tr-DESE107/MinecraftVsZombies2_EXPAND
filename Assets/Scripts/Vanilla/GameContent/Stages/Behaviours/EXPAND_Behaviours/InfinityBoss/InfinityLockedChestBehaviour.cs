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
    // 无限上锁的宝箱（InfinityLockedChest）：基于 InfinityBossBehaviour。  
    // 宝箱在 Init 内自动进入 IDLE 状态，其 SmashAppear 需要 SpawnParams，不适合直接当 OnBossAppear，故不重写。  
    public class InfinityLockedChestBehaviour : InfinityBossBehaviour  
    {  
        public InfinityLockedChestBehaviour(StageDefinition stageDef) : base(stageDef)  
        {  
        }  
  
        protected override NamespaceID BossID => VanillaBossID.lockedChest;  
        protected override NamespaceID[] EnemyPool => enemyPool;  
        protected override NamespaceID ProgressBarID => VanillaProgressBarID.lockedChest;  
        protected override NamespaceID BossMusic => VanillaMusicID.palaceBoss; // TODO: 替换为宝箱专属音乐  
  
        protected override int WarmupWaveCount => 10;  
  
        protected override int BossHealthStart => 8000;  
        protected override int BossHealthStep => 2500;  
        protected override int BossHealthMax => 200000;  
  
        protected override int FirstRestSeconds => 100;  
        protected override int RestStepSeconds => 4;  
        protected override int MinRestSeconds => 18;  
  
        protected override string IntroString => STRING_INTRO;  
        protected override string BossIncomingString => STRING_INCOMING;  
        protected override string ProgressRestString => STRING_PROGRESS_REST;  
  
        // ============ 出怪池（占位，请按主题调整） ============  
        private static readonly NamespaceID[] enemyPool = new NamespaceID[]  
        {  
            VanillaEnemyID.ghost,  
            VanillaEnemyID.LeatherGhost,  
            VanillaEnemyID.IronGhost,  
            VanillaEnemyID.diamondHelmettedZombie,  
            VanillaEnemyID.zombie,  
            VanillaEnemyID.shadowCell,  
            VanillaEnemyID.skeletonStatue,  
            VanillaEnemyID.hacker,  
            VanillaEnemyID.zombieCat,  
            VanillaEnemyID.WraithWarrior,  
            VanillaEnemyID.WraithArcher,  
            VanillaEnemyID.WraithBerserker,  
            VanillaEnemyID.WraithDullhan,  
            VanillaEnemyID.WraithVanguard,  
        };  
  
        // ============ 提示条本地化 Key ============  
        [TranslateMsg("无限宝箱提示")]  
        public const string STRING_INTRO = "坚持发展 10 波后上锁的宝箱将降临！击败尽可能多的宝箱吧！";  
        [TranslateMsg("无限宝箱提示")]  
        public const string STRING_INCOMING = "上锁的宝箱降临！";  
        [TranslateMsg("无限宝箱提示，{0}为累计击杀数，{1}为休息秒数")]  
        public const string STRING_PROGRESS_REST = "已击败 {0} 个上锁的宝箱！休息 {1} 秒后更强的宝箱来袭！";  
    }  
}
