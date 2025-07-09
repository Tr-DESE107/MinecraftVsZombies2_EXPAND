using System;
using System.Collections.Generic;
using System.Linq;
using PVZEngine.Entities;
using PVZEngine.Level;
using PVZEngine.Modifiers;
using UnityEngine;

namespace PVZEngine.Grids
{
    public class LawnGrid : IPropertyModifyTarget
    {
        #region 公有事件
        public LawnGrid(LevelEngine level, GridDefinition definition, int lane, int column)
        {
            Level = level;
            Lane = lane;
            Column = column;
            Definition = definition;
            properties = new PropertyBlock(this);
        }
        public int GetIndex()
        {
            return Level.GetGridIndex(Column, Lane);
        }
        public Vector3 GetEntityPosition()
        {
            var x = Level.GetEntityColumnX(Column);
            var z = Level.GetEntityLaneZ(Lane);
            var y = Level.GetGroundY(x, z);
            return new Vector3(x, y, z);
        }
        public float GetGroundY()
        {
            var x = Level.GetEntityColumnX(Column);
            var z = Level.GetEntityLaneZ(Lane);
            return Level.GetGroundY(x, z);
        }
        public void AddLayerEntity(NamespaceID layer, Entity entity)
        {
            layerEntities[layer] = entity;
        }
        public bool RemoveLayerEntity(NamespaceID layer)
        {
            return layerEntities.Remove(layer);
        }
        public bool RemoveLayerEntity(NamespaceID layer, Entity entity)
        {
            if (layerEntities.TryGetValue(layer, out var ent) && ent == entity)
            {
                return RemoveLayerEntity(layer);
            }
            return false;
        }
        public Entity GetLayerEntity(NamespaceID layer)
        {
            if (layerEntities.TryGetValue(layer, out var entity))
            {
                return entity;
            }
            return null;
        }
        public bool IsEmpty()
        {
            return layerEntities.Count == 0;
        }
        public Entity[] GetEntities()
        {
            return layerEntities.Values.ToArray();
        }
        public NamespaceID[] GetLayers()
        {
            return layerEntities.Keys.ToArray();
        }
        public SerializableGrid Serialize()
        {
            return new SerializableGrid()
            {
                lane = Lane,
                column = Column,
                definitionID = Definition.GetID(),
                layerEntities = layerEntities.ToDictionary(p => p.Key.ToString(), p => p.Value.ID),
                properties = properties.ToSerializable(),
            };
        }
        public static LawnGrid Deserialize(SerializableGrid seri, LevelEngine level)
        {
            var definition = level.Content.GetGridDefinition(seri.definitionID);
            var grid = new LawnGrid(level, definition, seri.lane, seri.column);
            return grid;
        }
        public void LoadFromSerializable(SerializableGrid seri, LevelEngine level)
        {
            properties = PropertyBlock.FromSerializable(seri.properties, this);
            layerEntities = seri.layerEntities.ToDictionary(p => NamespaceID.ParseStrict(p.Key), p => level.FindEntityByID(p.Value));
        }
        #endregion 方法

        #region 网格属性
        public T GetProperty<T>(PropertyKey<T> name, bool ignoreBuffs = false)
        {
            return properties.GetProperty<T>(name, ignoreBuffs);
        }
        public void SetProperty<T>(PropertyKey<T> name, T value)
        {
            properties.SetProperty(name, value);
        }
        public void SetPropertyObject(IPropertyKey name, object value)
        {
            properties.SetPropertyObject(name, value);
        }
        #endregion

        #region 接口实现
        bool IPropertyModifyTarget.GetFallbackProperty(IPropertyKey name, out object value)
        {
            if (Definition == null)
            {
                value = default;
                return false;
            }
            return Definition.TryGetPropertyObject(name, out value);
        }
        IEnumerable<IPropertyKey> IPropertyModifyTarget.GetModifiedProperties()
        {
            yield break;
        }
        PropertyModifier[] IPropertyModifyTarget.GetModifiersUsingProperty(IPropertyKey name)
        {
            return Array.Empty<PropertyModifier>();
        }
        void IPropertyModifyTarget.GetModifierItems(IPropertyKey name, List<ModifierContainerItem> results)
        {
        }
        void IPropertyModifyTarget.UpdateModifiedProperty(IPropertyKey name, object beforeValue, object afterValue, bool triggersEvaluation)
        {
        }
        #endregion

        #region 属性
        public LevelEngine Level { get; private set; }
        public int Lane { get; set; }
        public int Column { get; set; }
        public GridDefinition Definition { get; set; }
        private Dictionary<NamespaceID, Entity> layerEntities = new Dictionary<NamespaceID, Entity>();
        private PropertyBlock properties;
        #endregion 属性
    }
}