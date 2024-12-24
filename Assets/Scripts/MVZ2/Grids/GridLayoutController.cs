using System;
using System.Collections.Generic;
using System.Linq;
using MVZ2.UI;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MVZ2.Grids
{
    public class GridLayoutController : MonoBehaviour
    {
        public void UpdateGrids(GridViewData[][] viewDatas)
        {
            lanes.updateList(viewDatas.Length, (i, obj) =>
            {
                var lane = obj.GetComponent<LaneController>();
                lane.SetLane(i);
                lane.UpdateGrids(viewDatas[i]);
            },
            obj =>
            {
                var lane = obj.GetComponent<LaneController>();
                lane.OnPointerEnter += OnGridPointerEnterCallback;
                lane.OnPointerExit += OnGridPointerExitCallback;
                lane.OnPointerDown += OnGridPointerDownCallback;
                lane.OnPointerUp += OnGridPointerUpCallback;
            },
            obj =>
            {
                var lane = obj.GetComponent<LaneController>();
                lane.OnPointerEnter -= OnGridPointerEnterCallback;
                lane.OnPointerExit -= OnGridPointerExitCallback;
                lane.OnPointerDown -= OnGridPointerDownCallback;
                lane.OnPointerUp -= OnGridPointerUpCallback;
            });
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
        private void OnGridPointerEnterCallback(LaneController lane, int column, PointerEventData data)
        {
            var index = lanes.indexOf(lane);
            OnPointerEnter?.Invoke(index, column, data);
        }
        private void OnGridPointerExitCallback(LaneController lane, int column, PointerEventData data)
        {
            var index = lanes.indexOf(lane);
            OnPointerExit?.Invoke(index, column, data);
        }
        private void OnGridPointerDownCallback(LaneController lane, int column, PointerEventData data)
        {
            var index = lanes.indexOf(lane);
            OnPointerDown?.Invoke(index, column, data);
        }
        private void OnGridPointerUpCallback(LaneController lane, int column, PointerEventData data)
        {
            var index = lanes.indexOf(lane);
            OnPointerUp?.Invoke(index, column, data);
        }
        public event Action<int, int, PointerEventData> OnPointerEnter;
        public event Action<int, int, PointerEventData> OnPointerExit;
        public event Action<int, int, PointerEventData> OnPointerDown;
        public event Action<int, int, PointerEventData> OnPointerUp;
        [SerializeField]
        private ElementList lanes;
    }
}
