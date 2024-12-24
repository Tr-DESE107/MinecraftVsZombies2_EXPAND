using System;
using MVZ2.Grids;
using MVZ2.Vanilla.Level;
using MVZ2Logic;
using MVZ2Logic.HeldItems;
using MVZ2Logic.Level;
using PVZEngine;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MVZ2.Level
{
    public partial class LevelController : MonoBehaviour, IDisposable
    {

        #region 私有方法

        private void Awake_Grids()
        {
            gridLayout.OnPointerEnter += UI_OnGridEnterCallback;
            gridLayout.OnPointerExit += UI_OnGridExitCallback;
            gridLayout.OnPointerDown += UI_OnGridPointerDownCallback;
        }

        #region 事件回调

        private void UI_OnGridEnterCallback(int lane, int column, PointerEventData data)
        {
            if (!IsGameRunning())
                return;
            pointingGridLane = lane;
            pointingGridColumn = column;
            UpdateGridHighlight();
        }
        private void UI_OnGridExitCallback(int lane, int column, PointerEventData data)
        {
            if (!IsGameRunning())
                return;
            pointingGridLane = -1;
            pointingGridColumn = -1;
            UpdateGridHighlight();
        }
        private void UI_OnGridPointerDownCallback(int lane, int column, PointerEventData data)
        {
            if (data.button != PointerEventData.InputButton.Left)
                return;
            ClickOnGrid(lane, column, PointerPhase.Press);
        }

        #endregion
        private void ClickOnGrid(int lane, int column, PointerPhase phase)
        {
            if (!IsGameRunning())
                return;

            var grid = level.GetGrid(column, lane);
            var heldFlags = level.GetHeldFlagsOnGrid(grid);
            bool reset = heldFlags.HasFlag(HeldFlags.ForceReset);
            if (heldFlags.HasFlag(HeldFlags.Valid))
            {
                reset = level.UseOnGrid(grid, phase);
            }
            else
            {
                var errorMessage = level.GetHeldErrorMessageOnGrid(grid);
                if (!string.IsNullOrEmpty(errorMessage))
                {
                    level.ShowAdvice(Vanilla.VanillaStrings.CONTEXT_ADVICE, errorMessage, 0, 150);
                }
            }
            if (reset)
            {
                level.ResetHeldItem();
            }
        }
        private void ClearGridHighlight()
        {
            foreach (var grid in gridLayout.GetGrids())
            {
                grid.SetColor(Color.clear);
            }
        }
        private void UpdateGridHighlight()
        {
            bool pointingGrid = IsGameRunning() && pointingGridLane >= 0 && pointingGridColumn >= 0 && level.IsHoldingItem() && level.IsHeldItemForGrid(level.GetHeldItemType());
            if (pointingGrid != lastPointingGrid)
            {
                ClearGridHighlight();
                HighlightAxisGrids(pointingGridLane, pointingGridColumn);
                lastPointingGrid = pointingGrid;
            }
            if (pointingGrid)
            {
                HighlightGrid(pointingGridLane, pointingGridColumn);
            }
        }
        private void HighlightGrid(int lane, int column)
        {
            var grid = gridLayout.GetGrid(lane, column);
            var heldFlags = level.GetHeldFlagsOnGrid(level.GetGrid(column, lane));
            Color color = Color.clear;
            if (!heldFlags.HasFlag(HeldFlags.NoHighlight))
            {
                color = heldFlags.HasFlag(HeldFlags.Valid) ? Color.green : Color.red;
            }
            grid.SetColor(color);
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
            for (int lane = 0; lane < viewDatas.Length; lane++)
            {
                viewDatas[lane] = new GridViewData[maxColumn];
                for (int column = 0; column < maxColumn; column++)
                {
                    var x = level.GetColumnX(column) + gridWidth * 0.5f;
                    var z = level.GetLaneZ(lane) + gridHeight * 0.5f;
                    var y = level.GetGroundY(x, z);
                    var pos = new Vector3(x, y, z);
                    var worldPos = LawnToTrans(pos);

                    var sprite = Main.GetFinalSprite(defaultGridSprite);
                    viewDatas[lane][column] = new GridViewData()
                    {
                        position = new Vector2(worldPos.x, worldPos.y),
                        sprite = sprite
                    };
                }
            }
            gridLayout.UpdateGrids(viewDatas);
        }
        #endregion

        #region 属性字段
        private int pointingGridLane = -1;
        private int pointingGridColumn = -1;
        private bool lastPointingGrid;

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
