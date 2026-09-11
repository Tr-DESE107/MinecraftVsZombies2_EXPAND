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
    // 无限红龙（InfinityRedDragon）：基于 InfinityBossBehaviour。  
    public class InfinityRedDragonBehaviour : InfinityBossBehaviour  
    {  
        public InfinityRedDragonBehaviour(StageDefinition stageDef) : base(stageDef)  
        {  
        }  
  
        protected override NamespaceID BossID => VanillaBossID.redDragon;  
        protected override NamespaceID[] EnemyPool => enemyPool;  
        protected override NamespaceID ProgressBarID => VanillaProgressBarID.redDragon;  
        protected override NamespaceID BossMusic => VanillaMusicID.mausoleumBoss2; // TODO: 替换为红龙专属音乐  
  
        protected override int WarmupWaveCount => 10;  
  
        protected override int BossHealthStart => 5000;  
        protected override int BossHealthStep => 1500;  
        protected override int BossHealthMax => 120000;  
  
        protected override int FirstRestSeconds => 95;  
        protected override int RestStepSeconds => 5;  
        protected override int MinRestSeconds => 15;  
  
        protected override string IntroString => STRING_INTRO;  
        protected override string BossIncomingString => STRING_INCOMING;  
        protected override string ProgressRestString => STRING_PROGRESS_REST;  
  
        protected override void OnBossAppear(Entity boss)  
        {  
            RedDragon.SetAppear(boss);  
        }  
  
        // ============ 出怪池（占位，请按主题调整） ============  
        private static readonly NamespaceID[] enemyPool = new NamespaceID[]  
        {  
            VanillaEnemyID.PirateZombie,  
            VanillaEnemyID.LeatherPirateZombie,  
            VanillaEnemyID.PirateBucketSkeleton,  
            VanillaEnemyID.undeadFlyingObject,  
            VanillaEnemyID.zombieCloud,  
            VanillaEnemyID.cannoneerZombie,  
            VanillaEnemyID.popCaptain,  
            VanillaEnemyID.HeavyGutant,  
            VanillaEnemyID.Endermite,  
            VanillaEnemyID.cannonballZombie,  
            VanillaEnemyID.PirateIMP,  
            VanillaEnemyID.ChiefCannoneerZombie,  
            VanillaEnemyID.MusketeerZombie,  
            VanillaEnemyID.SailorZombie,  
        };  
  
        // ============ 提示条本地化 Key ============  
        [TranslateMsg("无限红龙提示")]  
        public const string STRING_INTRO = "坚持发展 10 波后红龙将降临！击败尽可能多的红龙吧！";  
        [TranslateMsg("无限红龙提示")]  
        public const string STRING_INCOMING = "红龙降临！";  
        [TranslateMsg("无限红龙提示，{0}为累计击杀数，{1}为休息秒数")]  
        public const string STRING_PROGRESS_REST = "已击败 {0} 条红龙！休息 {1} 秒后更强的红龙来袭！";  
    }  
}
