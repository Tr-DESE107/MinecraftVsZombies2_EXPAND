﻿using UnityEngine;

namespace MVZ2.Level.UI
{
    public class StarshardPanelPoint : MonoBehaviour
    {
        public void SetHighlight(bool highlight)
        {
            darkGameObject.SetActive(!highlight);
            highlightGameObject.SetActive(highlight);
        }
        [SerializeField]
        private GameObject darkGameObject;
        [SerializeField]
        private GameObject highlightGameObject;
    }
}
