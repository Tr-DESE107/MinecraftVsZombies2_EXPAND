﻿using System;
using MukioI18n;
using UnityEngine;


namespace MVZ2.Managers
{
    public class PerformanceManager : MonoBehaviour
    {
        private void Awake()
        {
            animatorBatchData.Init();
        }
        public void Update()
        {
            // 帧率计算
            frameCount++;
            fpsTimer += Time.deltaTime;

            if (fpsTimer >= fpsCheckInterval)
            {
                // 计算当前FPS
                currentFPS = frameCount / fpsCheckInterval;
                frameCount = 0;
                fpsTimer = 0;

                var fpsString = Main.LanguageManager._(FPS_TEMPLATE, currentFPS);
                Main.Scene.SetFPS(fpsString);
            }

        }
        public void UpdatePerformanceMonitor()
        {
            if (frameCount == 0)
            {
                // 动态调整批量大小
                AdjustBatchSize();
            }
        }

        void AdjustBatchSize()
        {
        }

        public MainManager Main => MainManager.Instance;
        [TranslateMsg("帧率显示")]
        public const string FPS_TEMPLATE = "FPS: {0}";
        // 运行时变量
        private float fpsTimer;
        private float frameCount;
        private float currentFPS;

        // 动态调整参数
        [Header("动态调整设置")]
        [SerializeField] private float fpsCheckInterval = 1f;    // 性能检测间隔（秒）

        [SerializeField] private PerformanceData animatorBatchData;
    }
    [Serializable]
    public class PerformanceData
    {
        public int minBatchSize = 40;      // 最小批量
        public int maxBatchSize = 100;    // 最大批量
        public float minFPS = 30;      // 最小批量
        public float maxFPS = 45;    // 最大批量
        public int initialBatchSize = 100; // 初始批量
        [NonSerialized]
        public int currentBatchSize = 100;

        public void Init()
        {
            currentBatchSize = initialBatchSize;
        }
        public void Adjust(float currentFPS)
        {
            var percentage = (currentFPS - minFPS) / (maxFPS - minFPS);
            currentBatchSize = Mathf.CeilToInt(Mathf.Lerp(minBatchSize, maxBatchSize, percentage));
        }
    }
}
