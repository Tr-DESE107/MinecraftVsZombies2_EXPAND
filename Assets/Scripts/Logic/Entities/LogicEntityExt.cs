using PVZEngine;
using PVZEngine.Entities;
using UnityEngine;

namespace MVZ2Logic.Entities
{
    public static class LogicEntityExt
    {
        #region 护甲
        public static Vector3 GetArmorDisplayPosition(this Entity entity, NamespaceID slot, NamespaceID armorID)
        {
            var shapeID = entity.GetShapeID();
            var shapeMeta = Global.Game.GetShapeMeta(shapeID);
            if (shapeMeta != null)
            {
                var offset = shapeMeta.GetArmorPosition(slot, armorID);
                offset.Scale(entity.GetFinalDisplayScale());
                return offset + entity.Position;
            }
            var bounds = entity.GetBounds();
            return bounds.center + Vector3.up * bounds.extents.y;
        }
        public static Vector3 GetArmorDisplayScale(this Entity entity, NamespaceID slot, NamespaceID armorID)
        {
            var shapeID = entity.GetShapeID();
            var shapeMeta = Global.Game.GetShapeMeta(shapeID);
            if (shapeMeta != null)
            {
                var offset = shapeMeta.GetArmorScale(slot, armorID);
                offset.Scale(entity.GetFinalDisplayScale());
                return offset;
            }
            return Vector3.one;
        }
        public static Vector3 GetArmorOffset(this Entity entity, NamespaceID slot, NamespaceID armorID)
        {
            var shapeID = entity.GetShapeID();
            var shapeMeta = Global.Game.GetShapeMeta(shapeID);
            if (shapeMeta != null)
            {
                return shapeMeta.GetArmorPosition(slot, armorID);
            }
            return Vector3.zero;
        }
        public static Vector3 GetArmorScale(this Entity entity, NamespaceID slot, NamespaceID armorID)
        {
            var shapeID = entity.GetShapeID();
            var shapeMeta = Global.Game.GetShapeMeta(shapeID);
            if (shapeMeta != null)
            {
                return shapeMeta.GetArmorScale(slot, armorID);
            }
            return Vector3.one;
        }
        #endregion
    }
}
