using System.Collections.Generic;
using MVZ2.Vanilla.Entities;
using PVZEngine.Base;
using PVZEngine.Entities;
using PVZEngine.Level;
using Tools.Mathematics;
using UnityEngine;

namespace MVZ2.Level.Components
{
    public class LightSourceUpdateList : UpdateListData<long, Entity, LightSourceInfo>
    {
        public LightSourceUpdateList(LevelEngine level)
        {
            this.level = level;
        }
        protected override long GetKey(Entity source)
        {
            return source.ID;
        }
        protected override LightSourceInfo CreateData(Entity source)
        {
            return new LightSourceInfo();
        }

        public override void Update(IEnumerable<Entity> validSources)
        {
            lightReceiverBuffer.Clear();
            level.FindEntitiesNonAlloc(IsValidLightReceiver, lightReceiverBuffer);
            base.Update(validSources);
        }
        protected override void UpdateElement(Entity entity, LightSourceInfo info)
        {
            base.UpdateElement(entity, info);
            var lightRange = entity.GetLightRange();
            var minComp = Mathf.Min(lightRange.x, lightRange.y, lightRange.z);
            lightRange.x -= minComp;
            lightRange.y -= minComp;
            lightRange.z -= minComp;

            var center = entity.GetCenter();
            var size = lightRange;
            var radius = minComp * 0.5f;
            var maxBounds = new Bounds(center, size + Vector3.one * minComp);

            // 获取符合条件的光照目标列表。
            lightReceiverToAddBuffer.Clear();
            foreach (var target in lightReceiverBuffer)
            {
                var bounds = target.GetBounds();
                if (maxBounds.Intersects(bounds) && MathTool.CollideBetweenCubeAndRoundCube(center, size, radius, bounds.center, bounds.size))
                {
                    // 符合条件的光照目标。
                    lightReceiverToAddBuffer.Add(target.ID);
                }
            }
            info.illuminatingEntities.Update(lightReceiverToAddBuffer);
        }
        private bool IsValidLightReceiver(Entity entity)
        {
            return entity.ReceivesLight();
        }
        private LevelEngine level;
        private List<Entity> lightReceiverBuffer = new List<Entity>();
        private List<long> lightReceiverToAddBuffer = new List<long>();
    }
}
