using System;
using UnityEngine;
using UnityEngine.UI;

namespace MVZ2.Archives
{
    public abstract class ArchivePage : MonoBehaviour
    {
        public void SetActive(bool active)
        {
            gameObject.SetActive(active);
        }
    }
}
