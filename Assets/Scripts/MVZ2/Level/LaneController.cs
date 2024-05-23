using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MVZ2.Level
{
    public class LaneController : MonoBehaviour
    {
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
                grids[i].OnPointerEnter += d => OnPointerEnter?.Invoke(index, d);
                grids[i].OnPointerExit += d => OnPointerExit?.Invoke(index, d);
                grids[i].OnPointerDown += d => OnPointerDown?.Invoke(index, d);
            }
        }
        public event Action<int, PointerEventData> OnPointerEnter;
        public event Action<int, PointerEventData> OnPointerExit;
        public event Action<int, PointerEventData> OnPointerDown;
        [SerializeField]
        private List<GridController> grids;
    }
}
