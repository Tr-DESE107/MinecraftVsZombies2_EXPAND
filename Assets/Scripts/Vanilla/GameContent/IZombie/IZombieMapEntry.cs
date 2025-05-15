using PVZEngine;
using PVZEngine.Level;
using UnityEngine;

namespace MVZ2Logic.IZombie
{
    public class IZombieMapEntry
    {
        public IZombieMapEntry(int column, int lane, NamespaceID entity, NamespaceID[] takenLayers)
        {
            this.column = column;
            this.lane = lane;
            this.entity = entity;
            this.takenLayers = takenLayers;
        }
        public void Apply(LevelEngine level)
        {
            var x = level.GetEntityColumnX(column);
            var z = level.GetEntityLaneZ(lane);
            var y = level.GetGroundY(x, z);
            var pos = new Vector3(x, y, z);
            level.Spawn(entity, pos, null);
        }
        public int column;
        public int lane;
        public NamespaceID entity;
        public NamespaceID[] takenLayers;

    }
}
