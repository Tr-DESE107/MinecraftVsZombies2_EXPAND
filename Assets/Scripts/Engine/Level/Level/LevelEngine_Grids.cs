using System.Linq;
using PVZEngine.Definitions;
using PVZEngine.Grids;
using UnityEngine;

namespace PVZEngine.Level
{
    public partial class LevelEngine
    {
        #region 初始化
        private void InitGrids(AreaDefinition area)
        {
            gridWidth = area.GetProperty<float>(EngineAreaProps.GRID_WIDTH);
            gridHeight = area.GetProperty<float>(EngineAreaProps.GRID_HEIGHT);
            gridLeftX = area.GetProperty<float>(EngineAreaProps.GRID_LEFT_X);
            gridBottomZ = area.GetProperty<float>(EngineAreaProps.GRID_BOTTOM_Z);
            entityLaneZOffset = area.GetProperty<float>(EngineAreaProps.ENTITY_LANE_Z_OFFSET);
            maxLaneCount = area.GetProperty<int>(EngineAreaProps.MAX_LANE_COUNT);
            maxColumnCount = area.GetProperty<int>(EngineAreaProps.MAX_COLUMN_COUNT);

            // Initalize current stage info.
            grids = new LawnGrid[maxColumnCount * maxLaneCount];

            var gridDefinitions = area.GetGridLayout().Select(i => Content.GetGridDefinition(i)).ToArray();
            for (int i = 0; i < gridDefinitions.Length; i++)
            {
                var definition = gridDefinitions[i];
                int lane = Mathf.FloorToInt(i / maxColumnCount);
                int column = i % maxColumnCount;
                grids[i] = new LawnGrid(this, definition, lane, column);
            }
        }
        #endregion

        #region 序列化
        private void WriteGridsToSerializable(SerializableLevel level)
        {
            level.grids = grids.Select(g => g.Serialize()).ToArray();
        }
        private void LoadGridsFromSerializable(SerializableLevel seri)
        {
            var count = Mathf.Min(grids.Length, seri.grids.Length);
            for (int i = 0; i < count; i++)
            {
                grids[i].LoadFromSerializable(seri.grids[i], this);
            }
        }
        private void ReadGridsFromSerializable(SerializableLevel seri)
        {
            for (int i = 0; i < grids.Length; i++)
            {
                var grid = grids[i];
                var seriGrid = seri.grids[i];
                grid.LoadAuras(seriGrid);
            }
        }
        #endregion

        #region 坐标相关方法
        public int GetMaxLaneCount() => maxLaneCount;
        public int GetMaxColumnCount() => maxColumnCount;
        public float GetGridLeftX() => gridLeftX;
        public float GetGridRightX()
        {
            return GetGridLeftX() + GetMaxColumnCount() * GetGridWidth();
        }
        public float GetGridBottomZ() => gridBottomZ;
        public float GetGridTopZ()
        {
            return GetGridBottomZ() + GetMaxLaneCount() * GetGridHeight();
        }
        public float GetGridWidth() => gridWidth;
        public float GetGridHeight() => gridHeight;
        public int GetNearestEntityLane(float z)
        {
            return GetLane(z - entityLaneZOffset + GetGridHeight() * 0.5f);
        }
        public float GetEntityLaneZ(int lane) => GetEntityLaneZFloat(lane);
        public float GetLaneZ(int lane) => GetLaneZFloat(lane);
        public float GetEntityLaneZFloat(float lane)
        {
            return GetLaneZFloat(lane) + entityLaneZOffset;
        }
        public float GetLaneZFloat(float lane)
        {
            return GetGridTopZ() - (lane + 1) * GetGridHeight();
        }
        public int GetGridLaneByIndex(int index)
        {
            return index / GetMaxColumnCount();
        }
        public int GetGridColumnByIndex(int index)
        {
            return index % GetMaxColumnCount();
        }
        public float GetLawnCenterX()
        {
            return (GetGridLeftX() + GetGridRightX()) * 0.5f;
        }
        public float GetLawnCenterZ()
        {
            return (GetGridBottomZ() + GetGridTopZ()) * 0.5f;
        }
        public int GetLane(float z)
        {
            return Mathf.FloorToInt((GetGridTopZ() - z) / GetGridHeight());
        }
        public int GetColumn(float x)
        {
            return Mathf.FloorToInt((x - GetGridLeftX()) / GetGridWidth());
        }
        public float GetEntityColumnX(int column) => GetEntityColumnXFloat(column);
        public float GetColumnX(int column) => GetColumnXFloat(column);
        public float GetEntityColumnXFloat(float column)
        {
            return GetColumnXFloat(column) + GetGridWidth() * 0.5f;
        }
        public float GetColumnXFloat(float column)
        {
            return GetGridLeftX() + column * GetGridWidth();
        }
        public Vector3 GetEntityGridPosition(int column, int lane)
        {
            var x = GetEntityColumnX(column);
            var z = GetEntityLaneZ(lane);
            var y = GetGroundY(x, z);
            return new Vector3(x, y, z);
        }
        public Vector3 GetEntityGridPositionByIndex(int index)
        {
            var column = GetGridColumnByIndex(index);
            var lane = GetGridLaneByIndex(index);
            return GetEntityGridPosition(column, lane);
        }
        public float GetGroundY(Vector3 pos)
        {
            return GetGroundY(pos.x, pos.z);
        }
        public float GetGroundY(float x, float z)
        {
            return AreaDefinition.GetGroundY(this, x, z);
        }
        #endregion

        public void UpdateGrids()
        {
            for (int i = 0; i < grids.Length; i++)
            {
                var grid = grids[i];
                grid.Update();
            }
        }

        #region 网格
        public LawnGrid GetGrid(int index)
        {
            if (index < 0 || index >= GetMaxColumnCount() * GetMaxLaneCount())
                return null;
            return grids[index];
        }
        public LawnGrid[] GetAllGrids()
        {
            return grids.ToArray();
        }
        public LawnGrid GetGrid(int column, int lane)
        {
            if (column < 0 || column >= GetMaxColumnCount() || lane < 0 || lane >= GetMaxLaneCount())
                return null;
            return GetGrid(lane * GetMaxColumnCount() + column);
        }
        public LawnGrid GetGrid(Vector2Int pos)
        {
            return GetGrid(pos.x, pos.y);
        }
        public int GetGridIndex(int column, int lane)
        {
            return column + lane * GetMaxColumnCount();
        }
        #endregion 坐标相关方法

        private LawnGrid[] grids;
        private float gridWidth;
        private float gridHeight;
        private float gridLeftX;
        private float gridBottomZ;
        private float entityLaneZOffset;
        private int maxLaneCount;
        private int maxColumnCount;
    }
}