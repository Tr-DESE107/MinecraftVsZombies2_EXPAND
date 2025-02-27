using System.Collections.Generic;
using System.Linq;
using PVZEngine.Entities;
using PVZEngine.Level;
using UnityEngine;

namespace PVZEngine.Grids
{
    public class LawnGrid
    {
        #region 公有事件
        public LawnGrid(LevelEngine level, GridDefinition definition, int lane, int column)
        {
            Level = level;
            Lane = lane;
            Column = column;
            Definition = definition;
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
                layerEntities = layerEntities.ToDictionary(p => p.Key.ToString(), p => p.Value.ID)
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
            layerEntities = seri.layerEntities.ToDictionary(p => NamespaceID.ParseStrict(p.Key), p => level.FindEntityByID(p.Value));
        }
        #endregion 方法

        #region 属性
        public LevelEngine Level { get; private set; }
        public int Lane { get; set; }
        public int Column { get; set; }
        public GridDefinition Definition { get; set; }
        private Dictionary<NamespaceID, Entity> layerEntities = new Dictionary<NamespaceID, Entity>();
        #endregion 属性
    }
    public enum PlaceType
    {
        Plantable = 1,
        Buildable = 2,
        Solid = 4,
        Aquatic = 8,
    }
}