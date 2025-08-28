using System.Collections.Generic;
using MVZ2.GameContent.Detections;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Properties;
using PVZEngine;
using PVZEngine.Entities;
using PVZEngine.Level;

namespace MVZ2.GameContent.Pickups
{
    [EntityBehaviourDefinition(VanillaEntityBehaviourNames.mergePickup)]
    public class MergePickup : EntityBehaviourDefinition
    {
        public MergePickup(string nsp, string name) : base(nsp, name)
        {
            mergeDetector = new GemMergeDetector();
        }
        public override void Init(Entity entity)
        {
            base.Init(entity);
            CheckMerge(entity);
        }
        public override void Update(Entity pickup)
        {
            base.Update(pickup);
            var level = pickup.Level;
            if (!pickup.IsCollected() && pickup.IsSecondsInterval(1))
            {
                CheckMerge(pickup);
            }
        }
        private void CheckMerge(Entity entity)
        {
            var mergeCount = GetMergeCount(entity);
            if (mergeCount <= 0)
                return;
            var mergeTarget = GetMergeTarget(entity);
            var mergeDetectBuffer = new List<Entity>();
            mergeDetector.DetectEntities(entity, mergeDetectBuffer);
            if (mergeDetectBuffer.Count < mergeCount)
                return;

            var mergeBuffer = new List<Entity>();
            foreach (var target in mergeDetectBuffer)
            {
                if (target.IsCollected())
                    continue;
                mergeBuffer.Add(target);
                if (mergeBuffer.Count >= mergeCount)
                {
                    var targetGem = mergeBuffer[0];
                    var merged = entity.Level.Spawn(mergeTarget, targetGem.Position, null);
                    merged.Velocity = targetGem.Velocity;
                    foreach (var mergeGem in mergeBuffer)
                    {
                        mergeGem.Remove();
                    }
                    mergeBuffer.Clear();
                }
            }
        }
        public static int GetMergeCount(Entity entity) => entity.GetProperty<int>(PROP_MERGE_COUNT);
        public static float GetMergeRange(Entity entity) => entity.GetProperty<float>(PROP_MERGE_RANGE);
        public static NamespaceID GetMergeTarget(Entity entity) => entity.GetProperty<NamespaceID>(PROP_MERGE_TARGET);

        private GemMergeDetector mergeDetector;

        public static readonly VanillaEntityPropertyMeta<int> PROP_MERGE_COUNT = new VanillaEntityPropertyMeta<int>("merge_count");
        public static readonly VanillaEntityPropertyMeta<float> PROP_MERGE_RANGE = new VanillaEntityPropertyMeta<float>("merge_range");
        public static readonly VanillaEntityPropertyMeta<NamespaceID> PROP_MERGE_TARGET = new VanillaEntityPropertyMeta<NamespaceID>("merge_target");
    }
}