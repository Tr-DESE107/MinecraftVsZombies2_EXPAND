using MVZ2.GameContent.Detections;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Detections;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Properties;
using PVZEngine.Damages;
using PVZEngine.Entities;
using PVZEngine.Level;
using Tools;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

namespace MVZ2.GameContent.Bosses
{
    [EntityBehaviourDefinition(VanillaBossNames.seija)]
    public partial class Seija : BossBehaviour
    {
        public Seija(string nsp, string name) : base(nsp, name)
        {
        }

        #region 回调
        public override void Init(Entity boss)
        {
            base.Init(boss);
            stateMachine.Init(boss);
            stateMachine.StartState(boss, STATE_IDLE);
            var timer = new FrameTimer(90);
            timer.Frame = 0;
            SetFabricCooldownTimer(boss, timer);
        }
        protected override void UpdateAI(Entity entity)
        {
            base.UpdateAI(entity);

            if (entity.IsDead)
                return;
            stateMachine.UpdateAI(entity);
            var timer = GetFabricCooldownTimer(entity);
            timer.Run();
        }
        protected override void UpdateLogic(Entity entity)
        {
            base.UpdateLogic(entity);
            stateMachine.UpdateLogic(entity);

            var takenDamage = GetRecentTakenDamage(entity);
            takenDamage = Mathf.Max(0, takenDamage - TAKEN_DAMAGE_FADE);
            SetRecentTakenDamage(entity, takenDamage);
        }
        public override void PostDeath(Entity boss, DeathInfo damageInfo)
        {
            base.PostDeath(boss, damageInfo);
            stateMachine.StartState(boss, STATE_FAINT);
        }
        public override void PreTakeDamage(DamageInput damageInfo)
        {
            base.PreTakeDamage(damageInfo);
            var boss = damageInfo.Entity;
            if (damageInfo.Amount > 600)
            {
                if (CanUseFabric(boss))
                {
                    UseFabric(boss);
                }
            }
            if (boss.State == STATE_FABRIC)
            {
                damageInfo.Cancel();
            }
        }
        public override void PostTakeDamage(DamageOutput result)
        {
            base.PostTakeDamage(result);
            if (result == null || result.BodyResult == null)
                return;
            var boss = result.Entity;
            var takenDamage = GetRecentTakenDamage(boss);
            takenDamage += result.BodyResult.SpendAmount;
            SetRecentTakenDamage(boss, takenDamage);
            if (takenDamage >= FABRIC_DAMAGE_THRESOLD)
            {
                if (CanUseFabric(boss))
                {
                    UseFabric(boss);
                }
            }
        }
        #endregion 事件

        #region 属性
        public static int GetFabricCount(Entity boss) => boss.GetBehaviourField<int>(PROP_FABRIC_COUNT);
        public static void SetFabricCount(Entity boss, int value) => boss.SetBehaviourField(PROP_FABRIC_COUNT, value);
        public static FrameTimer GetFabricCooldownTimer(Entity boss) => boss.GetBehaviourField<FrameTimer>(PROP_FABRIC_COOLDOWN_TIMER);
        public static void SetFabricCooldownTimer(Entity boss, FrameTimer value) => boss.SetBehaviourField(PROP_FABRIC_COOLDOWN_TIMER, value);
        public static float GetRecentTakenDamage(Entity boss) => boss.GetBehaviourField<float>(PROP_RECENT_TAKEN_DAMAGE);
        public static void SetRecentTakenDamage(Entity boss, float value) => boss.SetBehaviourField(PROP_RECENT_TAKEN_DAMAGE, value);
        public static void AddRecentTakenDamage(Entity boss, float value) => SetRecentTakenDamage(boss, GetRecentTakenDamage(boss) + value);
        #endregion 属性

        private static float GetChangeAdjacentLaneZSpeed(Entity boss)
        {
            var dir = 0;
            var lane = boss.GetLane();
            var maxLane = boss.Level.GetMaxLaneCount();
            if (lane <= 0)
            {
                dir = 1;
            }
            else if (lane >= maxLane - 1)
            {
                dir = -1;
            }
            else
            {
                dir = boss.RNG.Next(2) * 2 - 1;
            }
            var targetLane = Mathf.Clamp(lane + dir, 0, maxLane - 1);
            return GetChangeLaneZSpeed(boss, targetLane);
        }
        private static float GetChangeLaneZSpeed(Entity boss, int targetLane)
        {
            var targetZ = boss.Level.GetEntityLaneZ(targetLane);
            return GetChangeZSpeed(boss, targetZ);
        }
        private static float GetChangeZSpeed(Entity boss, float targetZ)
        {
            var currentZ = boss.Position.z;
            return (targetZ - currentZ) / 16.6f;
        }
        public static bool CanUseFabric(Entity boss)
        {
            var count = GetFabricCount(boss);
            if (count >= MAX_FABRIC_COUNT)
                return false;
            var timer = GetFabricCooldownTimer(boss);
            if (timer == null || !timer.Expired)
                return false;
            return true;
        }
        public static void UseFabric(Entity boss)
        {
            stateMachine.StartState(boss, STATE_FABRIC);
            SetFabricCount(boss, GetFabricCount(boss) + 1);
            var timer = GetFabricCooldownTimer(boss);
            timer.Reset();
            SetRecentTakenDamage(boss, 0);
            boss.PlaySound(VanillaSoundID.nimbleFabric);
        }
        public static bool ShouldCamera(Entity boss)
        {
            return cameraDetector.DetectEntityCount(boss) >= CAMERA_ENEMY_COUNT;
        }
        public static bool ShouldGapBomb(Entity boss)
        {
            if (boss.IsFacingLeft())
            {
                if (boss.GetColumn() < boss.Level.GetMaxColumnCount() - 1)
                    return false;
            }
            else
            {
                if (boss.GetColumn() > 0)
                    return false;
            }
            return gapBombDetector.DetectEntityCount(boss) >= GAP_BOMB_ENEMY_COUNT;
        }
        public static bool ShouldFrontflip(Entity boss)
        {
            var level = boss.Level;
            return boss.IsFacingLeft() ? boss.GetColumn() > 1 : boss.GetColumn() < level.GetMaxColumnCount() - 2;
        }
        public static bool ShouldBackflip(Entity boss)
        {
            var level = boss.Level;
            return boss.IsFacingLeft() ? boss.GetColumn() < level.GetMaxColumnCount() - 2 : boss.GetColumn() > 1;
        }
        public static Entity FindHammerTarget(Entity boss)
        {
            return hammerCheckDetector.DetectEntityWithTheLeast(boss, e => Mathf.Abs(e.Position.x - boss.Position.x));
        }

        #region 常量
        private static readonly VanillaEntityPropertyMeta PROP_FABRIC_COUNT = new VanillaEntityPropertyMeta("FabricCount");
        private static readonly VanillaEntityPropertyMeta PROP_FABRIC_COOLDOWN_TIMER = new VanillaEntityPropertyMeta("FabricCooldownTimer");
        private static readonly VanillaEntityPropertyMeta PROP_RECENT_TAKEN_DAMAGE = new VanillaEntityPropertyMeta("RecentTakenDamage");

        private const int MAX_FABRIC_COUNT = 3;
        private const float FABRIC_DAMAGE_THRESOLD = 300;
        private const float TAKEN_DAMAGE_FADE = FABRIC_DAMAGE_THRESOLD / 75f;

        private const int CAMERA_ENEMY_COUNT = 3;
        private const int GAP_BOMB_ENEMY_COUNT = 5;
        private const int BACKFLIP_ENEMY_COUNT = 3;
        private const float ADJUST_Z_THRESOLD = 5;

        private const int STATE_IDLE = VanillaEntityStates.SEIJA_IDLE;
        private const int STATE_APPEAR = VanillaEntityStates.SEIJA_APPEAR;
        private const int STATE_DANMAKU = VanillaEntityStates.SEIJA_DANMAKU;
        private const int STATE_HAMMER = VanillaEntityStates.SEIJA_HAMMER;
        private const int STATE_BACKFLIP = VanillaEntityStates.SEIJA_BACKFLIP;
        private const int STATE_FRONTFLIP = VanillaEntityStates.SEIJA_FRONTFLIP;
        private const int STATE_GAP_BOMB = VanillaEntityStates.SEIJA_GAP_BOMB;
        private const int STATE_CAMERA = VanillaEntityStates.SEIJA_CAMERA;
        private const int STATE_FABRIC = VanillaEntityStates.SEIJA_FABRIC;
        private const int STATE_FAINT = VanillaEntityStates.SEIJA_FAINT;
        #endregion 常量

        private static Detector hammerCheckDetector = new SeijaDetector(SeijaDetector.MODE_DETECT);
        private static Detector hammerSmashDetector = new SeijaDetector(SeijaDetector.MODE_SMASH);
        private static Detector hammerPlaceBombDetector = new SeijaDetector(SeijaDetector.MODE_PLACE_BOMB);
        private static Detector gapBombDetector = new SeijaDetector(SeijaDetector.MODE_GAP_BOMB);
        private static Detector cameraDetector = new SeijaDetector(SeijaDetector.MODE_CAMERA);
        private static SeijaStateMachine stateMachine = new SeijaStateMachine();
    }
}
