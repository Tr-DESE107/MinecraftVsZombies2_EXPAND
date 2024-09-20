using System;
using UnityEngine;

namespace MVZ2.UI
{
    public class MainSceneUI : MonoBehaviour
    {
        public void ShowDialog(string title, string desc, string[] options, Action<int> onSelect = null)
        {
            dialog.gameObject.SetActive(true);
            dialog.SetDialog(title, desc, options, (i) =>
            {
                onSelect?.Invoke(i);
                dialog.gameObject.SetActive(false);
            });
            dialog.ResetPosition();
        }
        [SerializeField]
        private Dialog dialog;
    }
}
