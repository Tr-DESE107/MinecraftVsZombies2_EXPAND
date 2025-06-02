using System;
using System.Linq;
using MVZ2.UI;
using MVZ2Logic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MVZ2.Grids
{
    public class LaneController : MonoBehaviour
    {
        public void UpdateGrids(GridViewData[] viewDatas)
        {
            grids.updateList(viewDatas.Length, (i, obj) =>
            {
                var grid = obj.GetComponent<GridController>();
                grid.Lane = Lane;
                grid.Column = i;
                grid.UpdateGridView(viewDatas[i]);
            },
            obj =>
            {
                var grid = obj.GetComponent<GridController>();
                grid.OnPointerInteraction += OnGridPointerInteractionCallback;
            },
            obj =>
            {
                var grid = obj.GetComponent<GridController>();
                grid.OnPointerInteraction -= OnGridPointerInteractionCallback;
            });
        }
        public void SetLane(int lane)
        {
            Lane = lane;
            for (int i = 0; i < grids.Count; i++)
            {
                GetGrid(i).Lane = lane;
            }
        }
        public GridController GetGrid(int column)
        {
            return grids.getElement<GridController>(column);
        }
        public GridController[] GetGrids()
        {
            return grids.getElements<GridController>().ToArray();
        }
        private void OnGridPointerInteractionCallback(GridController grid, PointerEventData data, PointerInteraction interaction)
        {
            var index = grids.indexOf(grid);
            OnPointerInteraction?.Invoke(this, index, data, interaction);
        }
        public event Action<LaneController, int, PointerEventData, PointerInteraction> OnPointerInteraction;
        public int Lane { get; private set; }
        [SerializeField]
        private ElementList grids;
    }
}
