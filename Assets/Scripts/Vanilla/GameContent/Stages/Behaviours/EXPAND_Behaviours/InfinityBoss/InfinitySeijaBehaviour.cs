#nullable enable

using MukioI18n;
using MVZ2.GameContent.Bosses;
using MVZ2.GameContent.Enemies;
using MVZ2.GameContent.ProgressBars;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Bosses;
using PVZEngine;
using PVZEngine.Entities;
using PVZEngine.Level;

namespace MVZ2.GameContent.Stages
{
    // 无限正邪（InfinitySeija）：基于 InfinityBossBehaviour。  
    // 开局跑数波普通怪 -> 之后每轮出 1 只正邪，正邪越来越强、休息越来越短。  
    // 正邪从右侧前空翻登场（SEIJA_FRONTFLIP），自带诅咒人偶随从（其 Init 内生成）。  
    public class InfinitySeijaBehaviour : InfinityBossBehaviour
    {
        public InfinitySeijaBehaviour(StageDefinition stageDef) : base(stageDef)
        {
        }

        protected override NamespaceID BossID => VanillaBossID.seija;
        protected override NamespaceID[] EnemyPool => seijaPool;
        protected override NamespaceID ProgressBarID => VanillaProgressBarID.seija;
        protected override NamespaceID BossMusic => VanillaMusicID.seija;

        protected override int WarmupWaveCount => 5;

        // 正邪基础血量约 2552，起点贴近该量级；数值可自行调整。  
        protected override int BossHealthStart => 2552;
        protected override int BossHealthStep => 696;
        protected override int BossHealthMax => 69696;

        protected override int FirstRestSeconds => 85;
        protected override int RestStepSeconds => 5;
        protected override int MinRestSeconds => 10;

        protected override string IntroString => STRING_INTRO;
        protected override string BossIncomingString => STRING_SEIJA_INCOMING;
        protected override string ProgressRestString => STRING_PROGRESS_REST;

        protected override void OnBossAppear(Entity boss)
        {
            // 正邪从右侧前空翻跳入场地（与官方 castle7 / 3-11 登场一致）。  
            Seija.StartState(boss, VanillaBossStates.SEIJA_FRONTFLIP);
        }

        // ============ 出怪池 ============  
        // 注意：以下敌人 ID 为占位，请按你想要的第三章/正邪主题小怪调整。  
        private static readonly NamespaceID[] seijaPool = new NamespaceID[]
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
            VanillaEnemyID.KingofReverser,
            VanillaEnemyID.NetherTroopCarrier,
            VanillaEnemyID.EvilMage,
        };

        // ============ 提示条本地化 Key ============  
        [TranslateMsg("无限正邪提示")]
        public const string STRING_INTRO = "坚持发展 5 波后正邪将降临！击败尽可能多的正邪吧！";
        [TranslateMsg("无限正邪提示")]
        public const string STRING_SEIJA_INCOMING = "正邪降临！";
        [TranslateMsg("无限正邪提示，{0}为累计击杀正邪数，{1}为休息秒数")]
        public const string STRING_PROGRESS_REST = "已击败 {0} 只正邪！休息 {1} 秒后更强的正邪来袭！";
    }
}
