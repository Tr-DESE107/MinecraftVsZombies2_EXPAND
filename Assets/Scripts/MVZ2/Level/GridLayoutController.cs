using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MVZ2.Level
{
    public class GridLayoutController : MonoBehaviour
    {
        public GridController GetGrid(int lane, int column)
        {
            return GetLane(lane).GetGrid(column);
        }
        public GridController[] GetGrids()
        {
            return lanes.SelectMany(l => l.GetGrids()).ToArray();
        }
        public LaneController GetLane(int lane)
        {
            return lanes[lane];
        }
        public LaneController[] GetLanes()
        {
            return lanes.ToArray();
        }
        private void Awake()
        {
            for (int i = 0; i < lanes.Count; i++)
            {
                int index = i;
                lanes[i].SetLane(i);
                lanes[i].OnPointerEnter += (c, d) => OnPointerEnter?.Invoke(index, c, d);
                lanes[i].OnPointerExit += (c, d) => OnPointerExit?.Invoke(index, c, d);
                lanes[i].OnPointerDown += (c, d) => OnPointerDown?.Invoke(index, c, d);
                lanes[i].OnPointerUp += (c, d) => OnPointerUp?.Invoke(index, c, d);
            }
        }
        public event Action<int, int, PointerEventData> OnPointerEnter;
        public event Action<int, int, PointerEventData> OnPointerExit;
        public event Action<int, int, PointerEventData> OnPointerDown;
        public event Action<int, int, PointerEventData> OnPointerUp;
        [SerializeField]
        private List<LaneController> lanes;
    }
}
