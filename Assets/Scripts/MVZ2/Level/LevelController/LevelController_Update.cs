﻿using MVZ2.Level.Components;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Level;
using MVZ2Logic.Level;
using PVZEngine.Entities;
using UnityEngine;

namespace MVZ2.Level
{
    public partial class LevelController
    {
        public bool IsGameRunning()
        {
            return isGameStarted && !isPaused && !isGameOver && !IsConsoleActive();
        }
        public float GetGameSpeed()
        {
            if (IsConsoleActive())
                return 0;
            return speedUp && !isGameOver ? Main.OptionsManager.GetFastForwardMultiplier() : 1;
        }
        private bool IsConsoleActive()
        {
            return Main.DebugManager.IsConsoleActive();
        }
        private void SwitchSpeedUp()
        {
            speedUp = !speedUp;
            GetUIPreset().SetSpeedUp(speedUp);
            level.PlaySound(speedUp ? VanillaSoundID.fastForward : VanillaSoundID.slowDown);
        }

        #region 逻辑更新
        public void UpdateLogic()
        {
            if (IsConsoleActive())
                return;
            if (isGameOver)
            {
                UpdateLogicGameOver();
            }
            else if (!IsGameRunning())
            {
                UpdateLogicNotRunning();
            }
            else
            {
                UpdateLogicRunning();
            }
        }
        private void UpdateLogicGameOver()
        {
            var killerCtrl = killerEntity;
            if (killerCtrl)
            {
                var killerEnt = killerCtrl.Entity;
                var pos = killerEnt.Position;
                pos.x -= 1;
                pos.z = pos.z * 0.5f + level.GetDoorZ() * 0.5f;
                pos.y = pos.y * 0.5f + level.GetGroundY(pos.x, pos.z) * 0.5f;
                killerEnt.Position = pos;
                killerCtrl.UpdateFixed();

                var passenger = killerEnt.GetRideablePassenger();
                if (passenger != null)
                {
                    passenger.Position = pos + killerEnt.GetPassengerOffset();
                    var passengerCtrl = GetEntityController(passenger);
                    if (passengerCtrl)
                    {
                        passengerCtrl.UpdateFixed();
                    }
                }
            }
            foreach (var entity in entities.ToArray())
            {
                if (CanUpdateAfterGameOver(entity.Entity))
                {
                    entity.Entity.Update();
                    entity.UpdateFixed();
                }
            }
        }
        private void UpdateLogicNotRunning()
        {
            foreach (var entity in entities.ToArray())
            {
                bool canRunBeforeGameStart = IsGameStarted() || CanUpdateBeforeGameStart(entity.Entity);
                bool canRunInPause = !IsGamePaused() || CanUpdateInPause(entity.Entity);
                if (canRunBeforeGameStart && canRunInPause)
                {
                    entity.Entity.Update();
                    entity.UpdateFixed();
                }
            }
        }
        private void UpdateLogicRunning()
        {
            var gameSpeed = GetGameSpeed();
            var times = (int)gameSpeed;
            gameRunTimeModular += gameSpeed - times;
            if (gameRunTimeModular > 1)
            {
                times += (int)gameRunTimeModular;
                gameRunTimeModular %= 1;
            }

            for (int time = 0; time < times; time++)
            {
                // 用于中断循环。防止Update后游戏结束，然后以下代码连续执行两次。
                if (!IsGameRunning())
                    break;
                UpdateLogicOnce();
            }
        }
        private void UpdateLogicOnce()
        {
            var entitiesCache = entities.ToArray();
            level.Update();
            foreach (var entity in entitiesCache)
            {
                entity.UpdateFixed();
            }
            gridLayout.UpdateGridsFixed();
            foreach (var part in parts)
            {
                part.UpdateLogic();
            }
            ui.UpdateHeldItemModelFixed();
            UpdateEnemyCry();
        }
        #endregion

        #region 画面更新
        public void UpdateFrame(float deltaTime)
        {
            float gameSpeed = GetGameSpeed();

            // 更新实体动画。
            UpdateFrameAnimators(deltaTime, gameSpeed);

            // 更新光标。
            UpdateHeldItemCursorEnabled();

            // 更新UI。
            UpdateFrameUI(deltaTime, gameSpeed);

            // 更新网格。
            UpdateGridHighlight();

            // 更新输入。
            UpdateInput();

            // 更新相机。
            levelCamera.ShakeOffset = (Vector3)Shakes.GetShake2D();
            UpdateCamera();

            // 设置光照。
            UpdateFrameLighting(deltaTime, gameSpeed);

            // 更新场景。
            UpdateFrameModel(deltaTime, gameSpeed);

            // 更新关卡。
            UpdateFrameLevelEngine(deltaTime, gameSpeed);
        }
        private void UpdateFrameAnimators(float deltaTime, float gameSpeed)
        {
            var perf = Main.PerformanceManager;
            if (IsGameRunning())
            {
                perf.UpdatePerformanceMonitor();
            }
            var maxBatchPercentage = Main.OptionsManager.GetAnimationFrequency();

            entityAnimatorBuffer.Clear();
            foreach (var entity in entities)
            {
                bool modelActive = false;
                var ent = entity.Entity;
                if (isGameOver)
                {
                    // 如果游戏结束，则只有在实体是杀死玩家的实体，或者在游戏结束后能行动时，才会动起来。
                    var killerCtrl = killerEntity;
                    var killerEnt = killerCtrl?.Entity;
                    modelActive = CanUpdateAfterGameOver(ent) || ent == killerEnt || ent == killerEnt?.GetRideablePassenger();
                }
                else
                {
                    // 游戏没有结束，则只有在游戏运行中，或者实体可以在游戏开始前行动，或者实体是预览敌人时，才会动起来。
                    bool canRunBeforeGameStart = IsGameStarted() || CanUpdateBeforeGameStart(ent);
                    bool canRunInPause = !IsGamePaused() || CanUpdateInPause(ent);
                    modelActive = (canRunBeforeGameStart && canRunInPause) || ent.IsPreviewEnemy();
                }
                float speed = modelActive ? gameSpeed : 0;
                entity.SetSimulationSpeed(speed);
                entity.UpdateFrame(deltaTime * speed);

                if (modelActive)
                {
                    entity.GetAnimatorsToUpdate(entityAnimatorBuffer);
                }
            }
            UpdateEntityAnimators(entityAnimatorBuffer, deltaTime, gameSpeed, maxBatchPercentage);
        }
        private void UpdateFrameUI(float deltaTime, float gameSpeed)
        {
            var gameRunning = IsGameRunning();
            if (!isGameOver)
            {
                // 游戏运行时更新UI。
                if (gameRunning)
                {
                    AdvanceLevelProgress();

                    UpdateHeldItemPosition();
                    UpdateInLevelUI(deltaTime * gameSpeed);
                }
                // 更新手持物品。
                var speed = gameRunning ? gameSpeed : 0;
                ui.UpdateHeldItemModelFrame(deltaTime * speed);
                ui.SetHeldItemModelSimulationSpeed(speed);
                UpdateTwinkle(gameRunning ? deltaTime : 0);
            }

            bool paused = IsGamePaused();
            // 暂停时显示金钱。
            if (paused)
            {
                ShowMoney();
            }

            // 设置射线检测。
            ui.SetRaycastDisabled(IsInputDisabled());

            var uiSimulationSpeed = paused ? 0 : gameSpeed;
            var uiDeltaTime = deltaTime * uiSimulationSpeed;

            var uiPreset = GetUIPreset();
            uiPreset.UpdateFrame(uiDeltaTime);
        }
        private void UpdateFrameModel(float deltaTime, float gameSpeed)
        {
            if (model)
            {
                var uiSimulationSpeed = IsGamePaused() ? 0 : gameSpeed;
                var uiDeltaTime = deltaTime * uiSimulationSpeed;
                model.UpdateAnimators(uiDeltaTime);
                model.UpdateFrame(uiDeltaTime);
                model.SetSimulationSpeed(uiSimulationSpeed);
            }
        }
        private void UpdateFrameLighting(float deltaTime, float gameSpeed)
        {
            var uiSimulationSpeed = IsGamePaused() ? 0 : gameSpeed;
            var uiDeltaTime = deltaTime * uiSimulationSpeed;

            float darknessSpeed = 2;
            if (!IsGameStarted() || IsGameOver() || level.IsCleared)
            {
                darknessSpeed = -2;
            }
            darknessFactor = Mathf.Clamp01(darknessFactor + darknessSpeed * uiDeltaTime);
            UpdateLighting();
        }
        private void UpdateFrameLevelEngine(float deltaTime, float gameSpeed)
        {
            if (level == null)
                return;

            ui.SetScreenCover(level.GetScreenCover());
            UpdateCameraByLevel(level);
            UpdateMoney();
            ValidateHeldItem();
            UpdateEntityHighlight();

            var uiSimulationSpeed = IsGamePaused() ? 0 : gameSpeed;
            foreach (var component in level.GetComponents())
            {
                if (component is IMVZ2LevelComponent comp)
                {
                    comp.UpdateFrame(deltaTime, uiSimulationSpeed);
                }
            }
            foreach (var part in parts)
            {
                part.UpdateFrame(deltaTime, uiSimulationSpeed);
            }
        }
        #endregion

        private bool CanUpdateBeforeGameStart(Entity entity)
        {
            return entity.CanUpdateBeforeGameStart();
        }
        private bool CanUpdateInPause(Entity entity)
        {
            return entity.CanUpdateInPause();
        }
        private bool CanUpdateAfterGameOver(Entity entity)
        {
            return entity.CanUpdateAfterGameOver();
        }

        #region 属性字段
        private bool speedUp;
        private float gameRunTimeModular;
        #endregion
    }
}
