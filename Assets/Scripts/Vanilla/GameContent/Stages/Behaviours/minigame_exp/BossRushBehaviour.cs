#nullable enable

using MVZ2.GameContent.Bosses;
using MVZ2.GameContent.Buffs.Enemies;
using MVZ2.GameContent.Buffs.Level;
using MVZ2.GameContent.Damages;
using MVZ2.GameContent.Enemies;
using MVZ2.GameContent.ProgressBars;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Bosses;
using MVZ2.Vanilla.Level;
using MVZ2.Vanilla.Localization;
using MVZ2.Vanilla.Properties;
using MVZ2Logic.Entities;
using MVZ2Logic.Level;
using MVZ2Logic.Localization;
using PVZEngine;
using PVZEngine.Buffs;
using PVZEngine.Damages;
using PVZEngine.Entities;
using PVZEngine.Level;
using Tools;
using UnityEngine;

namespace MVZ2.GameContent.Stages
{
    public class BossRushBehaviour : StageBehaviour
    {
        public BossRushBehaviour(StageDefinition stageDef) : base(stageDef)
        {
        }

        // ============ Boss 顺序 ============
        // 科学怪人 -> 梦魇 -> 正邪 -> 凋灵 -> 巨人 -> 红龙 -> 上锁的箱子
        // （梦魇为双形态：先瘦长鬼影，过场动画后转梦魇收割者，参照无限梦魇关卡。）
        private static readonly NamespaceID[] bossOrder = new NamespaceID[]
        {
            VanillaBossID.frankenstein,
            VanillaBossID.nightmareaper,
            VanillaBossID.seija,
            VanillaBossID.wither,
            VanillaBossID.theGiant,
            VanillaBossID.redDragon,
            VanillaBossID.lockedChest,
        };

        // ============ 生命周期 ============  
        public override void Start(LevelEngine level)
        {
            base.Start(level);
            // 不再一开始就切 STATE_BOSS_FIGHT。  
            // 先让 WaveStageBehaviour 正常跑普通波次（预热 5 波），  
            // 预热结束后再进入 Boss Rush。  
            SetBossIndex(level, 0);
            SetState(level, STATE_WARMUP);
            SetBossSeen(level, false);
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
                    FightingUpdate(level);
                    break;
                case STATE_TRANSITION:
                    TransitionUpdate(level);
                    break;
                case STATE_CLEARED:
                    level.CheckClearUpdate();
                    break;
            }
        }

        // ============ 预热阶段（先出 5 波普通怪） ============  
        private void WarmupUpdate(LevelEngine level)
        {
            // 普通波次由 WaveStageBehaviour 自行推进，这里只监测波数。  
            // 达到指定波数后进入 Boss Rush。  
            if (level.CurrentWave >= WARMUP_WAVE_COUNT)
            {
                StartBossRush(level);
            }
        }

        private void StartBossRush(LevelEngine level)
        {
            // 把关卡常驻在 STATE_BOSS_FIGHT，使普通波次系统不再自行推进到最后一波，  
            // 从而彻底避免 FinalWaveClearBehaviour 触发的“打完最后一波即通关”。  
            level.WaveState = VanillaLevelStates.STATE_BOSS_FIGHT;

            SetState(level, STATE_FIGHTING);
            SetBossIndex(level, 0);
            SetBossSeen(level, false);

            // 登场第一个 Boss（科学怪人）。  
            SpawnBossIntro(level, 0);
        }

        // ============ Boss 战阶段 ============
        private void FightingUpdate(LevelEngine level)
        {
            var index = GetBossIndex(level);
            var bossID = bossOrder[index];

            // 梦魇收割者过场期间：清场、暂停出怪，等待过场Buff生成梦魇收割者。
            if (bossID == VanillaBossID.nightmareaper && UpdateNightmareReaperTransition(level))
                return;

            // 推进阶段标记：过场Buff生成瘦长鬼影后进入「已在场」阶段（必须在提前返回之前）。
            if (bossID == VanillaBossID.nightmareaper
                && GetNightmarePhase(level) == NIGHTMARE_PHASE_SLENDER_PENDING
                && IsSlendermanAlive(level))
            {
                SetNightmarePhase(level, NIGHTMARE_PHASE_SLENDER_SEEN);
            }

            // 瘦长鬼影尚未生成时暂停出怪。
            if (bossID == VanillaBossID.nightmareaper && GetNightmarePhase(level) == NIGHTMARE_PHASE_SLENDER_PENDING)
                return;

            // 用波次系统按出怪池持续出怪（出怪池已在 Boss 登场时设置）。
            RunBossWave(level);

            // 检测当前 Boss 是否存活（梦魇按当前形态判定）。
            if (IsCurrentBossAlive(level, bossID))
            {
                SetBossSeen(level, true);
                return;
            }

            // Boss 曾经存活但现在死亡/消失 -> Boss 被击败。
            if (!GetBossSeen(level))
                return;

            // 梦魇特殊流程：瘦长鬼影被击败 → 播放过场动画生成梦魇收割者（不进入下一Boss）。
            if (bossID == VanillaBossID.nightmareaper && StartNightmareReaperTransition(level))
                return;

            OnBossDefeated(level, index);
        }

        // ============ 梦魇双形态（参照 InfinityNightmareBehaviour） ============
        private bool StartNightmareReaperTransition(LevelEngine level)
        {
            if (GetNightmarePhase(level) != NIGHTMARE_PHASE_SLENDER_SEEN)
                return false;
            // 尸体不再计入血条，并移除外挂再生Buff。
            foreach (var deadSlender in level.FindEntities(e => e.IsEntityOf(VanillaBossID.slenderman)))
            {
                deadSlender.SetProperty(LogicBossProps.DONT_COUNT_BOSS_HP, true);
                deadSlender.RemoveBuffs<RegenerationBuff>();
            }
            // 参照原版：关UI关音乐，播放黑暗过场，由过渡Buff生成梦魇收割者。
            level.ResetHeldItem();
            level.SetUIAndInputDisabled(true);
            level.StopMusic();
            level.AddBuff<NightmareaperTransitionBuff>();
            SetNightmarePhase(level, NIGHTMARE_PHASE_REAPER_TRANSITION);
            return true;
        }

        private bool UpdateNightmareReaperTransition(LevelEngine level)
        {
            if (GetNightmarePhase(level) != NIGHTMARE_PHASE_REAPER_TRANSITION)
                return false;
            // 过场期间清场并暂停出怪（参照原版）。
            ClearEnemies(level);
            if (!level.HasBuff<LevelEnemiesClearedBuff>())
                level.AddBuff<LevelEnemiesClearedBuff>();
            // 过场Buff生成梦魇收割者后恢复。
            if (IsNightmareaperAlive(level))
            {
                level.SetUIAndInputDisabled(false);
                level.RemoveBuffs<LevelEnemiesClearedBuff>();
                SetNightmarePhase(level, NIGHTMARE_PHASE_REAPER);
            }
            return true;
        }

        // 梦魇按当前形态判定存活：阶段1=瘦长鬼影，阶段2=梦魇收割者；过场期间视为存活。
        private bool IsNightmareBossAlive(LevelEngine level)
        {
            switch (GetNightmarePhase(level))
            {
                case NIGHTMARE_PHASE_SLENDER_PENDING:
                case NIGHTMARE_PHASE_SLENDER_SEEN:
                    return IsSlendermanAlive(level);
                case NIGHTMARE_PHASE_REAPER_TRANSITION:
                    return true;
                default:
                    return IsNightmareaperAlive(level);
            }
        }

        private static bool IsSlendermanAlive(LevelEngine level)
        {
            return level.EntityExists(e => e.IsEntityOf(VanillaBossID.slenderman) && !e.IsDead && e.IsHostileEntity());
        }

        private static bool IsNightmareaperAlive(LevelEngine level)
        {
            return level.EntityExists(e => e.IsEntityOf(VanillaBossID.nightmareaper) && !e.IsDead && e.IsHostileEntity());
        }

        private void OnBossDefeated(LevelEngine level, int index)
        {
            //EXPAND 本关正邪的尸体不留在场上，击败后立即移除（倒地音效与晕厥特效不受影响）。
            if (bossOrder[index] == VanillaBossID.seija)
            {
                foreach (var deadSeija in level.FindEntities(e => e.IsEntityOf(VanillaBossID.seija) && e.IsDead && e.IsHostileEntity()))
                {
                    deadSeija.Remove();
                }
            }

            // 恢复默认音乐与关卡进度条。
            var musicID = level.GetMusicID();
            if (musicID != null)
            {
                level.PlayMusic(musicID);
            }
            level.SetMusicVolume(1);
            level.SetProgressBarToStage();

            int nextIndex = index + 1;
            if (nextIndex >= bossOrder.Length)
            {
                // 最后一个 Boss 被击败 -> 清理残余小怪并进入通关判定。  
                ClearEnemies(level);
                SetState(level, STATE_CLEARED);
                return;
            }

            // 弹出提示：30 秒后将登场下一个 Boss。  
            ShowNextBossTip(level, bossOrder[nextIndex]);

            // 进入 30 秒过渡阶段。  
            SetState(level, STATE_TRANSITION);
            SetBossSeen(level, false);
            SetTransitionTimer(level, new FrameTimer(Ticks.FromSeconds(30)));
        }

        // ============ 过渡阶段（30 秒后登场下一个 Boss） ============  
        private void TransitionUpdate(LevelEngine level)
        {
            // 过渡期间继续用上一个 Boss 阶段的出怪池出怪（如不需要可删除下一行）。  
            RunBossWave(level);

            var timer = GetTransitionTimer(level);
            if (timer == null)
                return;
            timer.Run();
            if (timer.Expired)
            {
                int nextIndex = GetBossIndex(level) + 1;
                SetBossIndex(level, nextIndex);
                SetState(level, STATE_FIGHTING);
                SetBossSeen(level, false);
                SpawnBossIntro(level, nextIndex);
            }
        }

        // ============ Boss 登场（生成 + 特效 + 音乐 + 血条 + 出怪池） ============
        private void SpawnBossIntro(LevelEngine level, int index)
        {
            var bossID = bossOrder[index];

            // 梦魇：参照无限梦魇关卡，先由过场Buff生成瘦长鬼影（含登场表现/音乐/血条，并自带复仇加成），
            // 瘦长鬼影被击败后再过场生成梦魇收割者。
            if (bossID == VanillaBossID.nightmareaper)
            {
                SetNightmarePhase(level, NIGHTMARE_PHASE_SLENDER_PENDING);
                level.AddBuff<SlendermanTransitionBuff>();
                level.SetEnemyPool(GetEnemyPoolForBoss(bossID));
                return;
            }

            var boss = SpawnBossEntity(level, bossID);
            if (boss == null)
                return;

            // 独特登场特效。
            if (bossID == VanillaBossID.frankenstein)
            {
                Frankenstein.DoTransformationEffects(boss);
            }
            else if (bossID == VanillaBossID.seija)
            {
                // 正邪从右侧前空翻跳入场地（与官方 castle7 / 3-11 登场一致）。
                Seija.StartState(boss, VanillaBossStates.SEIJA_FRONTFLIP);
            }
            else if (bossID == VanillaBossID.wither)
            {
                Wither.Appear(boss);
            }
            else if (bossID == VanillaBossID.theGiant)
            {
                TheGiant.SetAppear(boss);
            }
            else if (bossID == VanillaBossID.redDragon)
            {
                RedDragon.SetAppear(boss);
            }
            // 上锁的箱子在 SpawnBossEntity 内已用 SmashAppear 生成并进入登场状态。  

            boss.ApplyBuffForBossRevenge();

            // 切换该阶段出怪池。  
            level.SetEnemyPool(GetEnemyPoolForBoss(bossID));

            // 切换背景音乐与血条。  
            level.PlayMusic(GetBossMusic(bossID));
            level.SetMusicVolume(1);
            level.SetProgressBarToBoss(GetBossProgressBar(bossID));
        }

        private Entity? SpawnBossEntity(LevelEngine level, NamespaceID bossID)
        {
            int centerLane = Mathf.FloorToInt(level.GetMaxLaneCount() * 0.5f);

            if (bossID == VanillaBossID.redDragon)
            {
                var pos = new Vector3(RedDragon.APPEAR_START_X, RedDragon.APPEAR_START_Y, level.GetLawnCenterZ());
                return level.Spawn(bossID, pos, null);
            }
            if (bossID == VanillaBossID.lockedChest)
            {
                // 上锁的箱子使用专门的砸地登场方法。  
                var pos = new Vector3(level.GetLawnCenterX(), 0, level.GetEntityLaneZ(centerLane));
                return LockedChest.SmashAppear(level, pos, null, new SpawnParams());
            }

            var spawnPos = new Vector3(LevelPositions.ENEMY_RIGHT_BORDER, 0, level.GetEntityLaneZ(centerLane));
            return level.Spawn(bossID, spawnPos, null);
        }

        // ============ 出怪表：按 Boss 阶段返回该阶段的出怪池 ============
        private static NamespaceID[] GetEnemyPoolForBoss(NamespaceID bossID)
        {
            if (bossID == VanillaBossID.frankenstein) return frankensteinPool;
            if (bossID == VanillaBossID.nightmareaper) return nightmarePool;
            if (bossID == VanillaBossID.seija) return seijaPool;
            if (bossID == VanillaBossID.wither) return witherPool;
            if (bossID == VanillaBossID.theGiant) return giantPool;
            if (bossID == VanillaBossID.redDragon) return redDragonPool;
            if (bossID == VanillaBossID.lockedChest) return lockedChestPool;
            return frankensteinPool;
        }

        private static readonly NamespaceID[] frankensteinPool = new NamespaceID[]
        {  
            // TODO: 科学怪人波次出怪表  
            VanillaEnemyID.zombie,
            VanillaEnemyID.ZombieHead,
            VanillaEnemyID.leatherCappedZombie,
            VanillaEnemyID.ironHelmettedZombie,
            VanillaEnemyID.skeleton,
            VanillaEnemyID.SkeletonHead,
            VanillaEnemyID.ghost,
            VanillaEnemyID.mummy,
            VanillaEnemyID.necromancer,
            VanillaEnemyID.skeletonHorse,
            VanillaEnemyID.MegaZombie,
            VanillaEnemyID.RedEyeZombieHead,
            VanillaEnemyID.KingSkeleton,
            VanillaEnemyID.SuperMegaZombie,
        };
        private static readonly NamespaceID[] nightmarePool = new NamespaceID[]
        {  
            // TODO: 梦魇波次出怪表  
            VanillaEnemyID.zombie,
            VanillaEnemyID.leatherCappedZombie,
            VanillaEnemyID.ironHelmettedZombie,
            VanillaEnemyID.spider,
            VanillaEnemyID.caveSpider,
            VanillaEnemyID.ghast,
            VanillaEnemyID.motherTerror,
            VanillaEnemyID.necromancer,
            VanillaEnemyID.skeleton,
            VanillaEnemyID.HostMutant,
            VanillaEnemyID.HostZombie,
            VanillaEnemyID.BloodlustHostZombie,
        };
        private static readonly NamespaceID[] seijaPool = new NamespaceID[]
        {
            // 正邪波次出怪表（与无限正邪一致，可按第三章主题调整）。
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
        private static readonly NamespaceID[] giantPool = new NamespaceID[]
        {  
            // TODO: 巨人波次出怪表  
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
        private static readonly NamespaceID[] redDragonPool = new NamespaceID[]
        {  
            // TODO: 红龙波次出怪表  
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
        private static readonly NamespaceID[] lockedChestPool = new NamespaceID[]
        {  
            // TODO: 上锁的箱子波次出怪表  
            VanillaEnemyID.zombie,
            VanillaEnemyID.leatherCappedZombie,
            VanillaEnemyID.ironHelmettedZombie,
            VanillaEnemyID.diamondHelmettedZombie,
            VanillaEnemyID.shadowCell,
            VanillaEnemyID.skeletonStatue,
            VanillaEnemyID.hacker,
            VanillaEnemyID.zombieCat,
            VanillaEnemyID.LeatherGhost,
            VanillaEnemyID.IronGhost,
            VanillaEnemyID.ghost,
            VanillaEnemyID.WraithBerserker,
        };

        // ============ 音乐 / 血条映射 ============
        private static NamespaceID GetBossMusic(NamespaceID bossID)
        {
            if (bossID == VanillaBossID.frankenstein) return VanillaMusicID.halloweenBoss;
            if (bossID == VanillaBossID.nightmareaper) return VanillaMusicID.nightmareBoss;
            if (bossID == VanillaBossID.seija) return VanillaMusicID.seija;
            if (bossID == VanillaBossID.wither) return VanillaMusicID.witherBoss;
            if (bossID == VanillaBossID.theGiant) return VanillaMusicID.mausoleumBoss;
            if (bossID == VanillaBossID.redDragon) return VanillaMusicID.shipBoss;
            if (bossID == VanillaBossID.lockedChest) return VanillaMusicID.palaceBoss;
            return VanillaMusicID.halloweenBoss;
        }
        private static NamespaceID GetBossProgressBar(NamespaceID bossID)
        {
            if (bossID == VanillaBossID.frankenstein) return VanillaProgressBarID.frankenstein;
            if (bossID == VanillaBossID.nightmareaper) return VanillaProgressBarID.nightmare;
            if (bossID == VanillaBossID.seija) return VanillaProgressBarID.seija;
            if (bossID == VanillaBossID.wither) return VanillaProgressBarID.wither;
            if (bossID == VanillaBossID.theGiant) return VanillaProgressBarID.theGiant;
            if (bossID == VanillaBossID.redDragon) return VanillaProgressBarID.redDragon;
            if (bossID == VanillaBossID.lockedChest) return VanillaProgressBarID.lockedChest;
            return VanillaProgressBarID.frankenstein;
        }

        // ============ 提示 ============  
        private void ShowNextBossTip(LevelEngine level, NamespaceID nextBossID)
        {
            var bossName = GetBossDisplayName(nextBossID);
            level.ShowAdvicePlural(LogicStrings.CONTEXT_ADVICE, VanillaStrings.ADVICE_NEXT_BOSS_INCOMING, 1, 100, 300, bossName);
        }
        private static string GetBossDisplayName(NamespaceID bossID)
        {
            if (bossID == VanillaBossID.frankenstein) return "科学怪人";
            if (bossID == VanillaBossID.nightmareaper) return "梦魇";
            if (bossID == VanillaBossID.seija) return "正邪";
            if (bossID == VanillaBossID.wither) return "凋灵";
            if (bossID == VanillaBossID.theGiant) return "巨人";
            if (bossID == VanillaBossID.redDragon) return "红龙";
            if (bossID == VanillaBossID.lockedChest) return "上锁的箱子";
            return "";
        }

        // ============ 辅助 ============  
        private void RunBossWave(LevelEngine level)
        {
            level.GetStageBehaviour<WaveStageBehaviour>()?.RunBossWave(level);
        }
        private static bool IsBossAlive(LevelEngine level, NamespaceID bossID)
        {
            return level.EntityExists(e => e.IsEntityOf(bossID) && !e.IsDead && e.IsHostileEntity());
        }
        private bool IsCurrentBossAlive(LevelEngine level, NamespaceID bossID)
        {
            if (bossID == VanillaBossID.nightmareaper)
                return IsNightmareBossAlive(level);
            return IsBossAlive(level, bossID);
        }
        private static void ClearEnemies(LevelEngine level)
        {
            foreach (var entity in level.FindEntities(e => e.Type == EntityTypes.ENEMY && !e.IsDead && e.IsHostileEntity()))
            {
                entity.Die(new DamageEffectList(VanillaDamageEffects.NO_REVIVAL));
            }
        }

        // ============ 关卡属性 ============  
        private static int GetBossIndex(LevelEngine level) => level.GetProperty<int>(PROP_BOSS_INDEX);
        private static void SetBossIndex(LevelEngine level, int value) => level.SetProperty(PROP_BOSS_INDEX, value);
        private static int GetState(LevelEngine level) => level.GetProperty<int>(PROP_STATE);
        private static void SetState(LevelEngine level, int value) => level.SetProperty(PROP_STATE, value);
        private static bool GetBossSeen(LevelEngine level) => level.GetProperty<bool>(PROP_BOSS_SEEN);
        private static void SetBossSeen(LevelEngine level, bool value) => level.SetProperty(PROP_BOSS_SEEN, value);
        private static FrameTimer? GetTransitionTimer(LevelEngine level) => level.GetProperty<FrameTimer>(PROP_TRANSITION_TIMER);
        private static void SetTransitionTimer(LevelEngine level, FrameTimer value) => level.SetProperty(PROP_TRANSITION_TIMER, value);
        private static int GetNightmarePhase(LevelEngine level) => level.GetProperty<int>(PROP_NIGHTMARE_PHASE);
        private static void SetNightmarePhase(LevelEngine level, int value) => level.SetProperty(PROP_NIGHTMARE_PHASE, value);

        private const int STATE_WARMUP = 0;
        private const int STATE_FIGHTING = 1;
        private const int STATE_TRANSITION = 2;
        private const int STATE_CLEARED = 3;
        private const int WARMUP_WAVE_COUNT = 10; // 进入 Boss Rush 前先出的普通波数

        // 梦魇双形态阶段（仅当当前 Boss 为 nightmareaper 时有意义）。
        private const int NIGHTMARE_PHASE_SLENDER_PENDING = 1;  // 等待过场Buff生成瘦长鬼影
        private const int NIGHTMARE_PHASE_SLENDER_SEEN = 2;     // 瘦长鬼影已在场
        private const int NIGHTMARE_PHASE_REAPER_TRANSITION = 3; // 播放过场动画，等待梦魇收割者
        private const int NIGHTMARE_PHASE_REAPER = 4;            // 梦魇收割者已在场

        private const string PROP_REGION = "boss_rush";
        [LevelPropertyRegistry(PROP_REGION)]
        public static readonly VanillaLevelPropertyMeta<int> PROP_BOSS_INDEX = new VanillaLevelPropertyMeta<int>("boss_index");
        [LevelPropertyRegistry(PROP_REGION)]
        public static readonly VanillaLevelPropertyMeta<int> PROP_STATE = new VanillaLevelPropertyMeta<int>("state");
        [LevelPropertyRegistry(PROP_REGION)]
        public static readonly VanillaLevelPropertyMeta<bool> PROP_BOSS_SEEN = new VanillaLevelPropertyMeta<bool>("boss_seen");
        [LevelPropertyRegistry(PROP_REGION)]
        public static readonly VanillaLevelPropertyMeta<FrameTimer> PROP_TRANSITION_TIMER = new VanillaLevelPropertyMeta<FrameTimer>("transition_timer");
        [LevelPropertyRegistry(PROP_REGION)]
        public static readonly VanillaLevelPropertyMeta<int> PROP_NIGHTMARE_PHASE = new VanillaLevelPropertyMeta<int>("nightmare_phase");
    }
}
