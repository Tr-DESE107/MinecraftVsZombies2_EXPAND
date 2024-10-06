using System;
using System.Collections.Generic;
using System.Linq;
using MVZ2.GameContent;
using PVZEngine;
using PVZEngine.Level;
using Tools.Mathematics;
using UnityEngine;

namespace MVZ2.Level.Components
{
    public partial class LightComponent : MVZ2Component, ILightComponent
    {
        public LightComponent(LevelEngine level, LevelController controller) : base(level, componentID, controller)
        {
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

            var hitbox = new RoundCube(entity.GetBoundsCenter(), lightRange, minComp * 0.5f);

            info.illuminatingEntities.Clear();
            info.illuminatingEntities.AddRange(allEntities.Where(e => MathTool.CollideBetweenCudeAndRoundCube(hitbox, e.GetBounds())).Select(e => e.ID));
        }
        private bool RemoveLightSourceInfo(Entity entity)
        {
            return lightSourceInfo.Remove(entity.ID);
        }
        private Dictionary<long, LightSourceInfo> lightSourceInfo = new Dictionary<long, LightSourceInfo>();
        public static readonly NamespaceID componentID = new NamespaceID(Builtin.spaceName, "lighting");
    }
    [Serializable]
    public class LightSourceInfo
    {
        public long id;
        public List<long> illuminatingEntities = new List<long>();
    }
}