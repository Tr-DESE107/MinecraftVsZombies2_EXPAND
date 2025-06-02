using System;
using System.Collections.Generic;
using System.Linq;
using MVZ2.UI;
using MVZ2Logic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MVZ2.Grids
{
    public class GridLayoutController : MonoBehaviour
    {
        public void UpdateGridsFixed()
        {
            if (grids == null)
                return;
            foreach (var grid in grids)
            {
                grid.UpdateFixed();
            }
        }
        public void UpdateGridViews(GridViewData[][] viewDatas)
        {
            List<GridController> gridList = new List<GridController>();
            lanes.updateList(viewDatas.Length, (i, obj) =>
            {
                var lane = obj.GetComponent<LaneController>();
                lane.SetLane(i);
                lane.UpdateGrids(viewDatas[i]);
                gridList.AddRange(lane.GetGrids());
            },
            obj =>
            {
                var lane = obj.GetComponent<LaneController>();
                lane.OnPointerInteraction += OnGridPointerEnterCallback;
            },
            obj =>
            {
                var lane = obj.GetComponent<LaneController>();
                lane.OnPointerInteraction -= OnGridPointerEnterCallback;
            });
            grids = gridList.ToArray();
        }
        public GridController GetGrid(int lane, int column)
        {
            return GetLane(lane).GetGrid(column);
        }
        public GridController[] GetGrids()
        {
            return lanes.getElements<LaneController>().SelectMany(l => l.GetGrids()).ToArray();
        }
        public LaneController GetLane(int lane)
        {
            return lanes.getElement<LaneController>(lane);
        }
        public LaneController[] GetLanes()
        {
            return lanes.getElements<LaneController>().ToArray();
        }
        private void OnGridPointerEnterCallback(LaneController lane, int column, PointerEventData data, PointerInteraction interaction)
        {
            var index = lanes.indexOf(lane);
            OnPointerInteraction?.Invoke(index, column, data, interaction);
        }
        public event Action<int, int, PointerEventData, PointerInteraction> OnPointerInteraction;
        [SerializeField]
        private ElementList lanes;
        private GridController[] grids;
    }
}
