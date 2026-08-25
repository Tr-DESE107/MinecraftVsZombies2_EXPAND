#nullable enable  
  
using MukioI18n;  
using MVZ2.GameContent.Bosses;  
using MVZ2.GameContent.Difficulties;   // WITHER_REGENERATION  
using MVZ2.GameContent.Enemies;  
using MVZ2.GameContent.ProgressBars;  
using MVZ2.Vanilla.Audios;  
using MVZ2.Vanilla.Bosses;  
using MVZ2Logic.Localization;  
using PVZEngine;  
using PVZEngine.Entities;  
using PVZEngine.Level;  
  
namespace MVZ2.GameContent.Stages  
{  
    // 无限凋灵（InfinityWither）：基于 InfinityBossBehaviour。  
    // 开局跑 5 波普通怪 -> 之后每轮出 1 只凋灵，凋灵越来越强、休息越来越短。  
    // 关闭凋灵血量再生；血量随序号从 6000 每只 +2000，封顶 36000。  
    public class InfinityWitherBehaviour : InfinityBossBehaviour  
    {  
        public InfinityWitherBehaviour(StageDefinition stageDef) : base(stageDef)  
        {  
        }  
  
        protected override NamespaceID BossID => VanillaBossID.wither;  
        protected override NamespaceID[] EnemyPool => witherPool;  
        protected override NamespaceID ProgressBarID => VanillaProgressBarID.wither;  
        protected override NamespaceID BossMusic => VanillaMusicID.witherBoss;  
  
        protected override int WarmupWaveCount => 5;  
  
        //protected override int BossHealthStart => 6000;  
        //protected override int BossHealthStep => 2000;  
        //protected override int BossHealthMax => 36000;  
  
        //protected override int FirstRestSeconds => 60;  
        //protected override int RestStepSeconds => 5;  
        //protected override int MinRestSeconds => 10;  
  
        protected override string IntroString => STRING_INTRO;  
        protected override string BossIncomingString => STRING_WITHER_INCOMING;  
        protected override string ProgressRestString => STRING_PROGRESS_REST;  
  
        protected override void OnStageStart(LevelEngine level)  
        {  
            // 关闭本关凋灵的血量再生（Wither.UpdateLogic 读取该属性作为每帧回血量）。  
            level.SetProperty(VanillaDifficultyLevelProps.WITHER_REGENERATION, 0f);  
        }  
  
        protected override void OnBossAppear(Entity boss)  
        {  
            // 播放凋灵生成动画（内部会播 witherSpawn 生成音效）。  
            Wither.Appear(boss);  
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
  
        // ============ 提示条本地化 Key ============  
        [TranslateMsg("无限凋灵提示")]  
        public const string STRING_INTRO = "坚持发展 5 波后凋灵将降临！击败尽可能多的凋灵吧！";  
        [TranslateMsg("无限凋灵提示")]  
        public const string STRING_WITHER_INCOMING = "凋灵降临！";  
        [TranslateMsg("无限凋灵提示，{0}为累计击杀凋灵数，{1}为休息秒数")]  
        public const string STRING_PROGRESS_REST = "已击败 {0} 只凋灵！休息 {1} 秒后更强的凋灵来袭！";  
    }  
}
