#nullable enable  
  
using MukioI18n;
using MVZ2.GameContent.Bosses;
using MVZ2.GameContent.Buffs.Level;
using MVZ2.GameContent.Enemies;
using MVZ2.GameContent.ProgressBars;
using MVZ2.Vanilla.Audios;
using MVZ2Logic.Entities;
using PVZEngine;
using PVZEngine.Buffs;
using PVZEngine.Entities;
using PVZEngine.Level;

namespace MVZ2.GameContent.Stages
{
    // 无限巨人（InfinityGiant）：基于 InfinityBossBehaviour。
    // 登场：由 TheGiantTransitionBuff 播放震屏+咆哮过场并生成巨人（SetAppear 登场表现+血条+音乐）。
    // 三阶段：巨人阶段1/2倒下后会原地复活（同实体 Revive），复活倒地期间不算被击败，
    // 只有三阶段（STATE_DEATH）的死亡才进入下一轮休息流程。
    public class InfinityGiantBehaviour : InfinityBossBehaviour
    {
        public InfinityGiantBehaviour(StageDefinition stageDef) : base(stageDef)
        {
        }

        protected override NamespaceID BossID => VanillaBossID.theGiant;
        protected override NamespaceID[] EnemyPool => enemyPool;
        protected override NamespaceID ProgressBarID => VanillaProgressBarID.theGiant;
        protected override NamespaceID BossMusic => VanillaMusicID.mausoleumBoss;

        protected override int WarmupWaveCount => 10;

        protected override int BossHealthStart => 8000;
        protected override int BossHealthStep => 2000;
        protected override int BossHealthMax => 150000;

        protected override int FirstRestSeconds => 95;
        protected override int RestStepSeconds => 4;
        protected override int MinRestSeconds => 15;

        // ============ EXPAND 增强选项幅度（先用基类默认值占位，便于后续逐Boss调整） ============
        protected override int BossHealthBonusPerChoice => 2000; // 血量选项：每次增加的血量（默认= BossHealthStep）
        protected override float BossAttackBonusPerChoice => 0.25f; // 攻击选项：每次攻击倍率 +25%
        protected override float DamageLimitStart => 2400; // 限伤选项：统一限伤初始值
        protected override float DamageLimitStep => 600; // 限伤选项：每次降低量
        protected override float DamageLimitMin => 600; // 限伤选项：下限
        protected override float DamageLimitDecay => 40; // 限伤选项：窗口每帧恢复量
        protected override float RegenBonusPerChoice => 0.5f; // 再生选项：每次增加的每帧回血（30帧=1秒）

        protected override string IntroString => STRING_INTRO;
        protected override string BossIncomingString => STRING_INCOMING;
        protected override string ProgressRestString => STRING_PROGRESS_REST;

        // ============ EXPAND 登场过场与三阶段复活 ============
        protected override bool UsesSpawnTransition => true;

        protected override void SpawnBoss(LevelEngine level, int bossIndex)
        {
            // 参照原版：由 TheGiantTransitionBuff 播放震屏+咆哮过场并生成巨人。
            level.AddBuff<TheGiantTransitionBuff>();
        }

        //EXPAND 巨人三阶段：阶段1/2倒下后进入复活序列（同实体 Revive）。死亡瞬间到 CheckDeath
        //把状态切到 STATE_FAINT 之间有一帧窗口，因此只要尸体状态不是 STATE_DEATH（三阶段真死亡）
        //就视为“即将复活”，不算被击败，血条继续追踪该巨人。
        protected override bool IsBossAlive(LevelEngine level)
        {
            return base.IsBossAlive(level)
                || level.EntityExists(e => e.IsEntityOf(BossID) && e.IsDead && e.IsHostileEntity() && e.State != TheGiant.STATE_DEATH);
        }
  
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
