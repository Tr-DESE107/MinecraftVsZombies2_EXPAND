using System;
using MVZ2.Grids;
using MVZ2.Managers;
using MVZ2.Vanilla.Level;
using MVZ2Logic;
using MVZ2Logic.HeldItems;
using MVZ2Logic.Level;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MVZ2.Level
{
    public partial class LevelController : MonoBehaviour, IDisposable
    {

        #region 私有方法

        private void Awake_Grids()
        {
            gridLayout.OnPointerInteraction += UI_OnGridPointerInteractionCallback;
        }

        #region 事件回调

        private void UI_OnGridPointerInteractionCallback(int lane, int column, PointerEventData data, PointerInteraction interaction)
        {
            var gridUI = gridLayout.GetGrid(lane, column);
            if (gridUI)
            {
                if (IsGameRunning())
                {
                    var grid = level.GetGrid(column, lane);
                    var pointerPosition = gridUI.TransformWorld2ColliderPosition(data.pointerCurrentRaycast.worldPosition);
                    var pointerParams = InputManager.GetPointerInteractionParamsFromEventData(data, interaction);
                    var target = new HeldItemTargetGrid(grid, pointerPosition);
                    level.DoHeldItemPointerEvent(target, pointerParams);
                }
            }


            if (interaction == PointerInteraction.Enter)
            {
                if (IsGameRunning())
                {
                    pointingGrid = level.GetGridIndex(column, lane);
                    pointingGridPointerId = data.pointerId;
                    UpdateGridHighlight();
                }
            }
            else if (interaction == PointerInteraction.Exit)
            {
                if (IsGameRunning())
                {
                    pointingGrid = -1;
                    pointingGridPointerId = -1;
                    UpdateGridHighlight();
                }
            }
        }

        #endregion
        private void ClearGridHighlight()
        {
            foreach (var grid in gridLayout.GetGrids())
            {
                grid.SetColor(Color.clear);
                grid.SetDisplaySection(0, 1);
            }
        }
        private void UpdateGridHighlight()
        {
            int pointing = pointingGrid;
            if (!IsGameRunning() || !level.IsHoldingItem())
            {
                pointing = -1;
            }
            if (pointing != lastPointingGrid)
            {
                ClearGridHighlight();
                lastPointingGrid = pointing;
            }
            if (pointing >= 0)
            {
                var lane = level.GetGridLaneByIndex(pointing);
                var column = level.GetGridColumnByIndex(pointing);
                var grid = level.GetGrid(column, lane);
                var gridUI = gridLayout.GetGrid(lane, column);

                Vector2 position;
                if (gridUI)
                {
                    var screenPos = Main.InputManager.GetPointerPosition(pointingGridPointerId);
                    var worldPos = levelCamera.Camera.ScreenToWorldPoint(screenPos);
                    position = gridUI.TransformWorld2ColliderPosition(worldPos);
                }
                else
                {
                    position = Vector2.zero;
                }
                var target = new HeldItemTargetGrid(grid, position);
                var type = InputManager.GetPointerDataFromPointerId(pointingGridPointerId);
                var highlight = level.GetHeldHighlight(target, type);
                HighlightGrid(lane, column, highlight);
                HighlightAxisGrids(lane, column);
            }
        }
        private void HighlightGrid(int lane, int column, HeldHighlight highlight)
        {
            Color color = Color.clear;
            if (highlight.mode == HeldHighlightMode.Grid)
            {
                color = highlight.gridValid ? Color.green : Color.red;
            }
            float rangeStart = highlight.gridRangeStart;
            float rangeEnd = highlight.gridRangeEnd;
            var gridUI = gridLayout.GetGrid(lane, column);
            gridUI.SetColor(color);
            gridUI.SetDisplaySection(rangeStart, rangeEnd);
        }
        private void HighlightAxisGrids(int lane, int column)
        {
            if (!Main.IsMobile())
                return;
            for (int l = 0; l < level.GetMaxLaneCount(); l++)
            {
                for (int c = 0; c < level.GetMaxColumnCount(); c++)
                {
                    if ((l == lane || c == column) && !(l == lane && c == column))
                    {
                        var g = gridLayout.GetGrid(l, c);
                        g.SetColor(gridColorTransparent);
                        g.SetDisplaySection(0, 1);
                    }
                }
            }
        }

        private void InitGrids()
        {
            var maxColumn = level.GetMaxColumnCount();
            var gridWidth = level.GetGridWidth();
            var gridHeight = level.GetGridHeight();
            var viewDatas = new GridViewData[level.GetMaxLaneCount()][];
            var areaMeta = Main.ResourceManager.GetAreaMeta(level.AreaID);
            for (int lane = 0; lane < viewDatas.Length; lane++)
            {
                viewDatas[lane] = new GridViewData[maxColumn];
                for (int column = 0; column < maxColumn; column++)
                {
                    var x = level.GetColumnX(column) + gridWidth * 0.5f;
                    var z = level.GetLaneZ(lane) + gridHeight * 0.5f;
                    var gridIndex = level.GetGridIndex(column, lane);
                    var gridMeta = areaMeta?.Grids?[gridIndex];
                    var yOffset = gridMeta?.YOffset ?? 0;
                    var y = 0 + yOffset;
                    var pos = new Vector3(x, y, z);
                    var worldPos = LawnToTrans(pos);

                    Sprite sprite;
                    if (gridMeta != null && SpriteReference.IsValid(gridMeta.Sprite))
                    {
                        sprite = Main.GetFinalSprite(gridMeta.Sprite);
                    }
                    else
                    {
                        sprite = Main.GetFinalSprite(defaultGridSprite);
                    }

                    var slope = (gridMeta?.Slope ?? 0) * LawnToTransScale;

                    viewDatas[lane][column] = new GridViewData()
                    {
                        position = new Vector2(worldPos.x, worldPos.y),
                        sprite = sprite,
                        slope = slope,
                    };
                }
            }
            gridLayout.UpdateGridViews(viewDatas);
        }
        #endregion

        #region 属性字段
        private int pointingGridPointerId = -1;
        private int pointingGrid = -1;
        private int lastPointingGrid;

        [Header("Grids")]
        [SerializeField]
        private Color gridColorTransparent = new Color(1, 1, 1, 0.5f);
        [SerializeField]
        private GridLayoutController gridLayout;
        [SerializeField]
        private Sprite defaultGridSprite;
        #endregion
    }
}
