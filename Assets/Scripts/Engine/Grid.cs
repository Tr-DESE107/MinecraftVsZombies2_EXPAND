using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PVZEngine
{
    public class Grid
    {
        #region 公有事件
        public Grid(Game level, GridDefinition definition, int lane, int column)
        {
            Level = level;
            Lane = lane;
            Column = column;
            Definition = definition;
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
            if (!takenEntities.Any(e => e.ID == entity.ID))
            {
                takenEntities.Add(new EntityReference(entity));
            }
        }
        public void RemoveEntity(Entity entity)
        {
            takenEntities.RemoveAll(e => e.ID == entity.ID);
        }
        public Entity[] GetTakenEntities()
        {
            return takenEntities.Select(e => e.GetEntity(Level)).ToArray();
        }
        #endregion 方法

        #region 属性
        public Game Level { get; private set; }
        public int Lane { get; set; }
        public int Column { get; set; }
        public GridDefinition Definition { get; set; }
        private List<EntityReference> takenEntities = new List<EntityReference>();
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