using System;
using System.Collections.Generic;
using System.Linq;
using MVZ2Logic;
using MVZ2Logic.Entities;
using MVZ2Logic.Level.Components;
using PVZEngine;
using PVZEngine.Entities;
using PVZEngine.Level;
using Tools;
using Tools.Mathematics;
using UnityEngine;

namespace MVZ2.Level.Components
{
    public partial class LightComponent : MVZ2Component, ILightComponent
    {
        public LightComponent(LevelEngine level, LevelController controller) : base(level, componentID, controller)
        {
        }
        public override void Update()
        {
            base.Update();
            checkTimer.Run();
            if (checkTimer.Expired)
            {
                checkTimer.Reset();
                UpdateLighting();
            }
        }
        public override ISerializableLevelComponent ToSerializable()
        {
            return new SerializableLightComponent()
            {
                checkTimer = checkTimer,
                lightSourceInfo = lightSourceInfo.Values.Select(v => new SerializableLightSourceInfo(v)).ToArray()
            };
        }
        public override void LoadSerializable(ISerializableLevelComponent seri)
        {
            base.LoadSerializable(seri);
            if (seri is not SerializableLightComponent serializable)
                return;
            checkTimer = serializable.checkTimer;
            foreach (var info in serializable.lightSourceInfo)
            {
                lightSourceInfo.Add(info.id, info.ToDeserialized());
            }
        }
        public bool IsIlluminated(Entity entity)
        {
            foreach (var pair in lightSourceInfo)
            {
                if (pair.Value.illuminatingEntities.Contains(entity.ID))
                    return true;
            }
            return false;
        }
        public long GetIlluminationLightSourceID(Entity entity)
        {
            foreach (var pair in lightSourceInfo)
            {
                if (pair.Value.illuminatingEntities.Contains(entity.ID))
                    return pair.Key;
            }
            return -1;
        }
        public IEnumerable<long> GetAllIlluminationLightSources(Entity entity)
        {
            foreach (var pair in lightSourceInfo)
            {
                if (pair.Value.illuminatingEntities.Contains(entity.ID))
                    yield return pair.Key;
            }
        }
        private void UpdateLighting()
        {
            var entities = Level.GetEntities();
            foreach (var entity in entities)
            {
                UpdateEntityLightSource(entity, entities);
            }
            var missingEntities = lightSourceInfo.Keys.Where(id => !Level.EntityExists(id)).ToArray();
            foreach (var missing in missingEntities)
            {
                lightSourceInfo.Remove(missing);
            }
        }
        private void UpdateEntityLightSource(Entity entity, Entity[] allEntities)
        {
            var lightingInfo = GetLightSourceInfo(entity.ID);
            if (entity.IsLightSource())
            {
                if (lightingInfo == null)
                {
                    lightingInfo = AddLightSourceInfo(entity);
                }
                UpdateLightSourceInfo(entity, lightingInfo, allEntities);
            }
            else
            {
                if (lightingInfo != null)
                {
                    RemoveLightSourceInfo(entity);
                }
            }
        }
        private LightSourceInfo GetLightSourceInfo(long id)
        {
            return lightSourceInfo.TryGetValue(id, out var info) ? info : null;
        }
        private LightSourceInfo AddLightSourceInfo(Entity entity)
        {
            var info = new LightSourceInfo()
            {
                id = entity.ID
            };
            lightSourceInfo.Add(entity.ID, info);
            return info;
        }
        private void UpdateLightSourceInfo(Entity entity, LightSourceInfo info, Entity[] allEntities)
        {
            var lightRange = entity.GetLightRange();
            var minComp = Mathf.Min(lightRange.x, lightRange.y, lightRange.z);
            lightRange.x -= minComp;
            lightRange.y -= minComp;
            lightRange.z -= minComp;

            var center = entity.GetBoundsCenter();
            var size = lightRange;
            var radius = minComp * 0.5f;
            var maxBounds = new Bounds(center, size + Vector3.one * minComp);

            foreach (var target in allEntities)
            {
                var bounds = target.GetBounds();
                bool inLight = maxBounds.Intersects(bounds) && MathTool.CollideBetweenCudeAndRoundCube(center, size, radius, bounds.center, bounds.size);
                var id = target.ID;
                if (inLight)
                {
                    info.illuminatingEntities.Add(id);
                }
                else
                {
                    info.illuminatingEntities.Remove(id);
                }
            }
        }
        private bool RemoveLightSourceInfo(Entity entity)
        {
            return lightSourceInfo.Remove(entity.ID);
        }
        private FrameTimer checkTimer = new FrameTimer(4);
        private Dictionary<long, LightSourceInfo> lightSourceInfo = new Dictionary<long, LightSourceInfo>();
        public static readonly NamespaceID componentID = new NamespaceID(Builtin.spaceName, "lighting");
    }
    public class LightSourceInfo
    {
        public long id;
        public HashSet<long> illuminatingEntities = new HashSet<long>();
    }
    [Serializable]
    public class SerializableLightComponent : ISerializableLevelComponent
    {
        public FrameTimer checkTimer;
        public SerializableLightSourceInfo[] lightSourceInfo;
    }
    [Serializable]
    public class SerializableLightSourceInfo
    {
        public SerializableLightSourceInfo(LightSourceInfo info)
        {
            id = info.id;
            illuminatingEntities = info.illuminatingEntities.ToArray();
        }
        public LightSourceInfo ToDeserialized()
        {
            return new LightSourceInfo()
            {
                id = id,
                illuminatingEntities = illuminatingEntities.ToHashSet()
            };
        }
        public long id;
        public long[] illuminatingEntities;
    }
}