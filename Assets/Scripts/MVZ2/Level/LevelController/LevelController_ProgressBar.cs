using System;
using System.Linq;
using MVZ2.Level.UI;
using MVZ2.Vanilla.Entities;
using PVZEngine;
using PVZEngine.Entities;
using PVZEngine.Level;
using UnityEngine;

namespace MVZ2.Level
{
    public partial class LevelController
    {
        private void Awake_ProgressBar()
        {
            var uiPreset = GetUIPreset();
            uiPreset.SetProgressBarVisible(false);
        }
        private void StartGame_ProgressBar()
        {
            levelProgress = 0;
            bannerProgresses = new float[level.GetTotalFlags()];

        }
        private void WriteToSerializable_ProgressBar(SerializableLevelController seri)
        {
            seri.bannerProgresses = bannerProgresses?.ToArray();
            seri.levelProgress = levelProgress;
            seri.bossProgress = bossProgress;
            seri.progressBarMode = progressBarMode;
            seri.bossProgressBarStyle = bossProgressBarStyle;
        }
        private void ReadFromSerializable_ProgressBar(SerializableLevelController seri)
        {
            bannerProgresses = seri.bannerProgresses?.ToArray();
            levelProgress = seri.levelProgress;
            bossProgress = seri.bossProgress;
            progressBarMode = seri.progressBarMode;
            bossProgressBarStyle = seri.bossProgressBarStyle;
        }


        public void SetProgressToBoss(NamespaceID barStyleID)
        {
            var ui = GetUIPreset();
            progressBarMode = true;
            bossProgressBarStyle = barStyleID;
            ui.SetProgressBarMode(progressBarMode);
            var meta = Main.ResourceManager.GetProgressBarMeta(barStyleID);
            if (meta == null)
                return;
            var background = Main.GetFinalSprite(meta.BackgroundSprite);
            var foreground = Main.GetFinalSprite(meta.ForegroundSprite);
            var bar = Main.GetFinalSprite(meta.BarSprite);
            var icon = Main.GetFinalSprite(meta.IconSprite);
            var viewData = new ProgressBarTemplateViewData()
            {
                backgroundSprite = background,
                foregroundSprite = foreground,
                barSprite = bar,
                fromLeft = meta.FromLeft,
                barMode = meta.BarMode,
                iconSprite = icon,
                padding = meta.Padding,
                size = meta.Size,
            };
            ui.SetBossProgressTemplate(viewData);
        }
        public void SetProgressToStage()
        {
            var ui = GetUIPreset();
            progressBarMode = false;
            ui.SetProgressBarMode(progressBarMode);
        }
        private void AdvanceLevelProgress()
        {
            var deltaTime = Time.deltaTime;
            if (progressBarMode)
            {
                // BOSS血条
                var bosses = level.FindEntities(e => e.Type == EntityTypes.BOSS && e.IsHostileEntity());
                if (bosses.Count() <= 0)
                {
                    bossProgress = 0;
                }
                else
                {
                    bossProgress = bosses.Sum(b => b.Health) / bosses.Sum(b => b.GetMaxHealth());
                }
            }
            else
            {
                // 关卡进度
                var totalFlags = level.GetTotalFlags();
                if (bannerProgresses == null || bannerProgresses.Length != totalFlags)
                {
                    var newProgresses = new float[totalFlags];
                    if (bannerProgresses != null)
                    {
                        bannerProgresses.CopyTo(newProgresses, 0);
                    }
                    bannerProgresses = newProgresses;
                }
                for (int i = 0; i < bannerProgresses.Length; i++)
                {
                    float value = (level.CurrentWave >= (totalFlags - i) * level.GetWavesPerFlag()) ? deltaTime : -deltaTime;
                    bannerProgresses[i] = Mathf.Clamp01(bannerProgresses[i] + value);
                }
                int totalWaveCount = level.GetTotalWaveCount();
                float targetProgress = totalWaveCount <= 0 ? 0 : level.CurrentWave / (float)totalWaveCount;
                int progressDirection = Math.Sign(targetProgress - levelProgress);
                if (progressDirection != 0)
                {
                    levelProgress += Time.deltaTime * 0.1f * progressDirection;
                    var newDirection = Mathf.Sign(targetProgress - levelProgress);
                    if (progressDirection != newDirection)
                    {
                        levelProgress = targetProgress;
                    }
                }
            }
        }
        private void UpdateLevelProgressUI()
        {
            var ui = GetUIPreset();
            ui.SetProgressBarVisible(level.LevelProgressVisible);
            ui.SetLevelProgress(levelProgress);
            ui.SetBannerProgresses(bannerProgresses);
            ui.SetBossProgress(bossProgress);
        }
        private void RefreshProgressBar()
        {
            if (progressBarMode)
            {
                SetProgressToBoss(bossProgressBarStyle);
            }
            else
            {
                SetProgressToStage();
            }
        }


        #region 属性字段
        private float levelProgress;
        private float[] bannerProgresses;
        private float bossProgress;
        private bool progressBarMode;
        private NamespaceID bossProgressBarStyle;
        #endregion
    }
}
