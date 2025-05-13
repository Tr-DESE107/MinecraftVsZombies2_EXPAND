using System.Collections.Generic;
using MVZ2.GameContent.Detections;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Level;
using MVZ2.Vanilla.Properties;
using MVZ2Logic.Level;
using MVZ2Logic.Models;
using PVZEngine;
using PVZEngine.Entities;
using Tools;
using UnityEngine;

namespace MVZ2.GameContent.Pickups
{
    public abstract class Gem : PickupBehaviour
    {
        public Gem(string nsp, string name) : base(nsp, name)
        {
            mergeDetector = new GemMergeDetector(MergeSize, MergeSource);
        }
        public override void Init(Entity entity)
        {
            base.Init(entity);
            CheckMerge(entity);
        }

        private void CheckMerge(Entity entity)
        {
            if (!CanMerge)
                return;
            mergeDetectBuffer.Clear();
            mergeDetector.DetectEntities(entity, mergeDetectBuffer);
            var count = mergeDetectBuffer.Count;
            if (count < MergeCount)
                return;

            mergeBuffer.Clear();
            foreach (var target in mergeDetectBuffer)
            {
                if (target.IsCollected())
                    continue;
                mergeBuffer.Add(target);
                if (mergeBuffer.Count >= MergeCount)
                {
                    var targetGem = mergeBuffer[0];
                    var merged = entity.Level.Spawn(MergeTarget, targetGem.Position, null);
                    merged.Velocity = targetGem.Velocity;
                    foreach (var mergeGem in mergeBuffer)
                    {
                        mergeGem.Remove();
                    }
                    mergeBuffer.Clear();
                }
            }
        }
        public override void Update(Entity pickup)
        {
            base.Update(pickup);
            var level = pickup.Level;
            float alpha = 1;
            float shadowAlpha = 1;
            if (pickup.IsCollected())
            {
                var collectedTime = pickup.GetCollectedTime();
                var moveTime = level.GetSecondTicks(1);
                var vanishTime = level.GetSecondTicks(1.5f);
                if (collectedTime < moveTime)
                {
                    float timePercent = collectedTime / (float)moveTime;
                    var targetPos = GetMoveTargetPosition(pickup);
                    pickup.Velocity = (targetPos - pickup.Position) * 0.2f;
                    alpha = 1;
                }
                else
                {
                    if (collectedTime == moveTime)
                    {
                        level.RemoveDelayedMoney(pickup);
                    }

                    var vanishLerp = (collectedTime - moveTime) / (float)(vanishTime - moveTime);
                    pickup.SetDisplayScale(Vector3.one * Mathf.Lerp(1, 0.5f, vanishLerp));
                    alpha = Mathf.Lerp(1, 0, vanishLerp);
                    if (collectedTime == vanishTime)
                    {
                        pickup.Remove();
                    }
                }
                shadowAlpha = 0;
            }
            else
            {
                if (!pickup.IsImportantPickup() && pickup.Timeout < 15 && pickup.Timeout >= 0)
                {
                    pickup.Velocity = Vector3.zero;
                    alpha = pickup.Timeout / 15f;
                    shadowAlpha = alpha;
                }
                var mergeTimer = GetMergeTimer(pickup);
                if (mergeTimer == null)
                {
                    mergeTimer = new FrameTimer(30);
                    SetMergeTimer(pickup, mergeTimer);
                }
                mergeTimer.Run();
                if (mergeTimer.Expired)
                {
                    mergeTimer.Reset();
                    CheckMerge(pickup);
                }
            }
            pickup.SetAnimationBool("Rotating", !pickup.IsCollected() && pickup.GetRelativeY() <= 0);
            var color = pickup.GetTint(true);
            color.a = alpha;
            pickup.SetTint(color);
            pickup.SetShadowAlpha(shadowAlpha);
        }
        public override void PostContactGround(Entity entity, Vector3 velocity)
        {
            base.PostContactGround(entity, velocity);
            entity.Velocity = Vector3.zero;
            entity.Level.PlaySound(entity.GetDropSound());
        }
        public override void PostCollect(Entity pickup)
        {
            base.PostCollect(pickup);
            pickup.Velocity = Vector3.zero;
            var level = pickup.Level;
            level.AddDelayedMoney(pickup, pickup.GetMoneyValue());
            pickup.SetProperty(EngineEntityProps.GRAVITY, 0f);
            pickup.SetSortingLayer(SortingLayers.money);
            pickup.SetSortingOrder(9999);

            var pitch = GetRandomCollectPitch() ? Random.Range(0.95f, 1.5f) : 1;
            pickup.PlaySound(pickup.GetCollectSound(), pitch);
            level.ShowMoney();
        }
        protected virtual bool GetRandomCollectPitch()
        {
            return true;
        }
        private static Vector3 GetMoveTargetPosition(Entity entity)
        {
            var level = entity.Level;
            Vector3 slotPosition = level.GetMoneyPanelEntityPosition();
            return new Vector3(slotPosition.x, slotPosition.y - COLLECTED_Z - 15, COLLECTED_Z);
        }
        public static void Disappear(Entity pickup)
        {
            pickup.Timeout = 15;
        }
        public static FrameTimer GetMergeTimer(Entity entity) => entity.GetBehaviourField<FrameTimer>(PROP_MERGE_TIMER);
        public static void SetMergeTimer(Entity entity, FrameTimer value) => entity.SetBehaviourField(PROP_MERGE_TIMER, value);
        private const float COLLECTED_Z = -100;
        private List<Entity> mergeDetectBuffer = new List<Entity>();
        private List<Entity> mergeBuffer = new List<Entity>();
        private GemMergeDetector mergeDetector;
        public const string PROP_REGION = "gem";
        [EntityPropertyRegistry(PROP_REGION)]
        public static readonly VanillaEntityPropertyMeta PROP_MERGE_TIMER = new VanillaEntityPropertyMeta("mergeTimer");
        protected virtual bool CanMerge => false;
        protected virtual int MergeCount => 1;
        protected virtual float MergeSize => 80;
        protected virtual NamespaceID MergeSource => null;
        protected virtual NamespaceID MergeTarget => null;
    }
}