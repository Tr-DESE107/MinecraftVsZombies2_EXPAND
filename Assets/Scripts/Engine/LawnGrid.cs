using System.Collections.Generic;
using PVZEngine.Serialization;
using UnityEngine;

namespace PVZEngine
{
    public class LawnGrid
    {
        #region 公有事件
        public LawnGrid(Game level, GridDefinition definition, int lane, int column)
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
        public Vector3 GetPosition()
        {
            var x = Level.GetColumnX(Column);
            var z = Level.GetEntityLaneZ(Lane);
            var y = Level.GetGroundHeight(x, z);
            return new Vector3(x, y, z);
        }
        public float GetGroundHeight()
        {
            var x = Level.GetColumnX(Column);
            var z = Level.GetEntityLaneZ(Lane);
            return Level.GetGroundHeight(x, z);
        }
        public void AddEntity(Entity entity)
        {
            if (!takenEntities.Contains(entity))
            {
                takenEntities.Add(entity);
            }
        }
        public bool RemoveEntity(Entity entity)
        {
            return takenEntities.Remove(entity);
        }
        public Entity[] GetTakenEntities()
        {
            return takenEntities.ToArray();
        }
        public SerializableGrid Serialize()
        {
            return new SerializableGrid()
            {
                lane = Lane,
                column = Column,
                definitionID = Definition.GetID(),
                takenEntities = takenEntities.ConvertAll(e => e.ID)
            };
        }
        public static LawnGrid Deserialize(SerializableGrid seri, Game level)
        {
            var definition = level.GetGridDefinition(seri.definitionID);
            var grid = new LawnGrid(level, definition, seri.lane, seri.column);
            return grid;
        }
        #endregion 方法

        #region 属性
        public Game Level { get; private set; }
        public int Lane { get; set; }
        public int Column { get; set; }
        public GridDefinition Definition { get; set; }
        private List<Entity> takenEntities = new List<Entity>();
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