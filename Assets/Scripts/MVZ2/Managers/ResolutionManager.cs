using System;
using MukioI18n;
using UnityEngine;

namespace MVZ2
{
    [ExecuteAlways]
    public class ResolutionManager : MonoBehaviour
    {
        public void SetResolution(Resolution resolution)
        {
            Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreenMode);
        }
        public Resolution[] GetResolutions()
        {
            return Screen.resolutions;
        }
        public Resolution GetCurrentResolution()
        {
            return new Resolution()
            {
                width = Screen.width,
                height = Screen.height
            };
        }
        public string GetResolutionName(Resolution resolution)
        {
            return MainManager.Instance.LanguageManager._(RESOLUTION_NAME, resolution.width, resolution.height);
        }
        private void OnEnable()
        {
            Check();
        }
        private void OnDisable()
        {
            lastWidth = 0;
            lastHeight = 0;
        }
        private void Update()
        {
            Check();
        }
        private void Check()
        {
            if (lastWidth != Screen.width || lastHeight != Screen.height)
            {
                lastWidth = Screen.width;
                lastHeight = Screen.height;
                OnResolutionChanged?.Invoke(lastWidth, lastHeight);
            }
        }
        public event Action<int, int> OnResolutionChanged;

        [TranslateMsg("分辨率名，{0}为宽，{1}为高")]
        public const string RESOLUTION_NAME = "{0}x{1}";
        private int lastWidth;
        private int lastHeight;
    }
}
