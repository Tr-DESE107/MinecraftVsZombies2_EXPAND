using System;
using System.Linq;
using MVZ2.UI;
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
                grid.UpdateGrid(viewDatas[i]);
            },
            obj =>
            {
                var grid = obj.GetComponent<GridController>();
                grid.OnPointerEnter += OnGridPointerEnterCallback;
                grid.OnPointerExit += OnGridPointerExitCallback;
                grid.OnPointerDown += OnGridPointerDownCallback;
                grid.OnPointerUp += OnGridPointerUpCallback;
            },
            obj =>
            {
                var grid = obj.GetComponent<GridController>();
                grid.OnPointerEnter -= OnGridPointerEnterCallback;
                grid.OnPointerExit -= OnGridPointerExitCallback;
                grid.OnPointerDown -= OnGridPointerDownCallback;
                grid.OnPointerUp -= OnGridPointerUpCallback;
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
        private void OnGridPointerEnterCallback(GridController grid, PointerEventData data)
        {
            var index = grids.indexOf(grid);
            OnPointerEnter?.Invoke(this, index, data);
        }
        private void OnGridPointerExitCallback(GridController grid, PointerEventData data)
        {
            var index = grids.indexOf(grid);
            OnPointerExit?.Invoke(this, index, data);
        }
        private void OnGridPointerDownCallback(GridController grid, PointerEventData data)
        {
            var index = grids.indexOf(grid);
            OnPointerDown?.Invoke(this, index, data);
        }
        private void OnGridPointerUpCallback(GridController grid, PointerEventData data)
        {
            var index = grids.indexOf(grid);
            OnPointerUp?.Invoke(this, index, data);
        }
        public event Action<LaneController, int, PointerEventData> OnPointerEnter;
        public event Action<LaneController, int, PointerEventData> OnPointerExit;
        public event Action<LaneController, int, PointerEventData> OnPointerDown;
        public event Action<LaneController, int, PointerEventData> OnPointerUp;
        public int Lane { get; private set; }
        [SerializeField]
        private ElementList grids;
    }
}
