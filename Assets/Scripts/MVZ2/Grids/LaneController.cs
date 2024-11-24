using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MVZ2.Grids
{
    public class LaneController : MonoBehaviour
    {
        public void SetLane(int lane)
        {
            Lane = lane;
            for (int i = 0; i < grids.Count; i++)
            {
                grids[i].Lane = lane;
            }
        }
        public GridController GetGrid(int column)
        {
            return grids[column];
        }
        public GridController[] GetGrids()
        {
            return grids.ToArray();
        }
        private void Awake()
        {
            for (int i = 0; i < grids.Count; i++)
            {
                int index = i;
                grids[i].Column = i;
                grids[i].OnPointerEnter += d => OnPointerEnter?.Invoke(index, d);
                grids[i].OnPointerExit += d => OnPointerExit?.Invoke(index, d);
                grids[i].OnPointerDown += d => OnPointerDown?.Invoke(index, d);
                grids[i].OnPointerUp += d => OnPointerUp?.Invoke(index, d);
            }
        }
        public event Action<int, PointerEventData> OnPointerEnter;
        public event Action<int, PointerEventData> OnPointerExit;
        public event Action<int, PointerEventData> OnPointerDown;
        public event Action<int, PointerEventData> OnPointerUp;
        public int Lane { get; private set; }
        [SerializeField]
        private List<GridController> grids;
    }
}
