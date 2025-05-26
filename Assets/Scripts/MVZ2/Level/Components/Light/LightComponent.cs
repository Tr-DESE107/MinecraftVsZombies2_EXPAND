using System;
using System.Collections.Generic;
using System.Linq;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Entities;
using MVZ2Logic.Level.Components;
using PVZEngine;
using PVZEngine.Entities;
using PVZEngine.Level;
using Tools;

namespace MVZ2.Level.Components
{
    public partial class LightComponent : MVZ2Component, ILightComponent
    {
        public LightComponent(LevelEngine level, LevelController controller) : base(level, componentID, controller)
        {
            lightSources = new LightSourceUpdateList(level);
        }
        public override void Update()
        {
            base.Update();
            if (Level.IsTimeInterval(4))
            {
                UpdateLighting();
            }
        }
        #region 序列化
        public override ISerializableLevelComponent ToSerializable()
        {
            return new SerializableLightComponent()
            {
                lightSources = lightSources.Select(pair => new SerializableLightSourceInfo(pair.Key, pair.Value)).ToArray()
            };
        }
        public override void LoadSerializable(ISerializableLevelComponent seri)
        {
            base.LoadSerializable(seri);
            if (seri is not SerializableLightComponent serializable)
                return;
            foreach (var info in serializable.lightSources)
            {
                lightSources.Add(info.id, info.ToDeserialized());
            }
        }
        #endregion

        #region 获取信息
        public bool IsIlluminated(Entity entity)
        {
            foreach (var pair in lightSources)
            {
                if (pair.Value.illuminatingEntities.Contains(entity.ID))
                    return true;
            }
            return false;
        }
        public long GetIlluminationLightSourceID(Entity entity)
        {
            foreach (var pair in lightSources)
            {
                if (pair.Value.illuminatingEntities.Contains(entity.ID))
                    return pair.Key;
            }
            return -1;
        }
        public IEnumerable<long> GetAllIlluminationLightSources(Entity entity)
        {
            foreach (var pair in lightSources)
            {
                if (pair.Value.illuminatingEntities.Contains(entity.ID))
                    yield return pair.Key;
            }
        }
        public void GetIlluminatingEntities(Entity entity, HashSet<long> results)
        {
            if (lightSources.TryGetValue(entity.ID, out var info))
            {
                foreach (var ent in info.illuminatingEntities)
                {
                    results.Add(ent);
                }
            }
        }
        #endregion

        #region 更新光照
        private void UpdateLighting()
        {
            lightSourceBuffer.Clear();
            Level.FindEntitiesNonAlloc(IsValidLightSource, lightSourceBuffer);
            lightSources.Update(lightSourceBuffer);
        }
        private bool IsValidLightSource(Entity entity)
        {
            return entity.IsLightSource();
        }
        #endregion

        #region 属性字段
        private LightSourceUpdateList lightSources;

        private List<Entity> lightSourceBuffer = new List<Entity>();

        public static readonly NamespaceID componentID = new NamespaceID(VanillaMod.spaceName, "lighting");
        #endregion
    }
    [Serializable]
    public class SerializableLightComponent : ISerializableLevelComponent
    {
        public SerializableLightSourceInfo[] lightSources;
    }
}