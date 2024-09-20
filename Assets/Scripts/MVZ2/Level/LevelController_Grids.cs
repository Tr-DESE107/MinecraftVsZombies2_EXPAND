using System;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using MVZ2.GameContent;
using MVZ2.Vanilla;
using PVZEngine;
using PVZEngine.Game;
using PVZEngine.Level;
using PVZEngine.Serialization;
using Tools;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MVZ2.Level
{
    public partial class LevelController : MonoBehaviour, IDisposable
    {

        #region 私有方法

        #region 事件回调

        private void UI_OnGridEnterCallback(int lane, int column, PointerEventData data)
        {
            if (!IsGameRunning())
                return;
            var grid = gridLayout.GetGrid(lane, column);
            var color = Color.clear;
            if (level.IsHoldingItem())
            {
                color = level.IsGridValidForHeldItem(level.GetGrid(column, lane)) ? Color.green : Color.red;
            }
            grid.SetColor(color);
        }
        private void UI_OnGridExitCallback(int lane, int column, PointerEventData data)
        {
            if (!IsGameRunning())
                return;
            var grid = gridLayout.GetGrid(lane, column);
            grid.SetColor(Color.clear);
        }
        private void UI_OnGridPointerDownCallback(int lane, int column, PointerEventData data)
        {
            if (!IsGameRunning())
                return;
            if (data.button != PointerEventData.InputButton.Left)
                return;
            var grid = level.GetGrid(column, lane);
            if (!level.IsGridValidForHeldItem(grid))
                return;
            if (level.UseOnGrid(grid))
            {
                level.ResetHeldItem();
            }
        }

        #endregion
        private void HideGridSprites()
        {
            foreach (var grid in gridLayout.GetGrids())
            {
                grid.SetColor(Color.clear);
            }
        }
        #endregion

        #region 属性字段
        [SerializeField]
        private GridLayoutController gridLayout;
        #endregion
    }
}
