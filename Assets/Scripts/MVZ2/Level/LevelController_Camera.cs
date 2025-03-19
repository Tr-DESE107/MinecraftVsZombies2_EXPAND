using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MVZ2.Cameras;
using MVZ2.Level.UI;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Level;
using MVZ2.Vanilla.Saves;
using MVZ2Logic;
using MVZ2Logic.Callbacks;
using MVZ2Logic.Level;
using MVZ2Logic.SeedPacks;
using PVZEngine;
using PVZEngine.Triggers;
using UnityEngine;

namespace MVZ2.Level
{
    public partial class LevelController : MonoBehaviour, IDisposable
    {
        public Camera GetCamera()
        {
            return levelCamera.Camera;
        }
        #region 私有方法

        #region 移动相机
        private void SetCameraPosition(LevelCameraPosition position)
        {
            var pos = cameraHousePosition;
            var anchor = cameraHouseAnchor;
            switch (position)
            {
                case LevelCameraPosition.Lawn:
                    pos = cameraLawnPosition;
                    anchor = cameraLawnAnchor;
                    break;
                case LevelCameraPosition.Choose:
                    pos = cameraChoosePosition;
                    anchor = cameraChooseAnchor;
                    break;
            }
            levelCamera.SetPosition(pos, anchor);
        }
        private IEnumerator MoveCameraLawn(Vector3 target, Vector2 targetAnchor, float maxTime)
        {
            float time = 0;
            Vector3 start = levelCamera.CameraPosition;
            Vector2 startAnchor = levelCamera.CameraAnchor;
            while (time < maxTime)
            {
                time = Mathf.Clamp(time + Time.deltaTime, 0, maxTime);
                var lerp = cameraMoveCurve.Evaluate(time / maxTime);
                var pos = Vector3.Lerp(start, target, lerp);
                var anchor = Vector2.Lerp(startAnchor, targetAnchor, lerp);
                levelCamera.SetPosition(pos, anchor);
                yield return null;
            }
        }
        private IEnumerator MoveCameraToHouse()
        {
            return MoveCameraLawn(cameraHousePosition, cameraHouseAnchor, 1f);
        }
        public IEnumerator MoveCameraToLawn()
        {
            return MoveCameraLawn(cameraLawnPosition, cameraLawnAnchor, 1f);
        }
        public IEnumerator MoveCameraToChoose()
        {
            return MoveCameraLawn(cameraChoosePosition, cameraChooseAnchor, 1f);
        }
        #endregion

        #region 游戏开始
        private void GameStartInstantTransition()
        {
            SetCameraPosition(LevelCameraPosition.Lawn);
            UpdateDifficulty();
            Game.RunCallback(LogicLevelCallbacks.PRE_BATTLE, c => c(level));
            level.PrepareForBattle();
            StartGame();
        }
        private IEnumerator GameStartToPreviewTransition()
        {
            Music.Play(VanillaMusicID.choosing);
            level.CreatePreviewEnemies(VanillaLevelExt.GetEnemySpawnRect());
            yield return new WaitForSeconds(1);
            yield return MoveCameraToChoose();
            yield return new WaitForSeconds(1);
        }
        public IEnumerator GameStartToLawnTransition()
        {
            yield return MoveCameraToLawn();
            UpdateDifficulty();
            Game.RunCallback(LogicLevelCallbacks.PRE_BATTLE, c => c(level));
            level.PrepareForBattle();
            yield return new WaitForSeconds(0.5f);
            PlayReadySetBuild();
        }
        private IEnumerator GameStartToLawnInstantTransition()
        {
            Music.Play(VanillaMusicID.choosing);
            yield return new WaitForSeconds(1);
            yield return MoveCameraToLawn();
            yield return new WaitForSeconds(0.5f);
            StartGame();
        }
        private IEnumerator GameStartTransition()
        {
            yield return GameStartToPreviewTransition();

            UpdateDifficulty();
            UpdateEnergy();
            level.UpdatePersistentLevelUnlocks();

            var innateBlueprints = Game.GetInnateBlueprints();
            var unlockedContraptions = Saves.GetUnlockedContraptions().Where(id => Main.ResourceManager.IsContraptionInAlmanac(id));
            var unlockedArtifacts = Saves.GetUnlockedArtifacts();
            var seedSlotCount = level.GetSeedSlotCount();
            bool willChooseBlueprint = innateBlueprints.Length + unlockedContraptions.Count() > seedSlotCount || unlockedArtifacts.Length > 0; 

            if (willChooseBlueprint && level.NeedBlueprints())
            {
                // 选卡。
                var uiPreset = GetUIPreset();
                BlueprintChoosePart.ShowBlueprintChoosePanel(unlockedContraptions);
                uiPreset.SetUIVisibleState(LevelUIPreset.VisibleState.ChoosingBlueprints);
                uiPreset.SetReceiveRaycasts(false);
                yield return new WaitForSeconds(0.5f);
                uiPreset.SetReceiveRaycasts(true);
            }
            else
            {
                var chooseItems = new List<BlueprintChooseItem>();
                foreach (var innate in innateBlueprints)
                {
                    chooseItems.Add(new BlueprintChooseItem()
                    {
                        innate = true,
                        id = innate
                    });
                }
                foreach (var contraption in unlockedContraptions.Take(seedSlotCount))
                {
                    chooseItems.Add(new BlueprintChooseItem()
                    {
                        id = contraption
                    });
                }
                var seedPacks = chooseItems.Select(i => i.id).ToArray();
                level.ReplaceSeedPacks(seedPacks);
                Game.RunCallback(LogicLevelCallbacks.POST_BLUEPRINT_SELECTION, c => c(level, chooseItems.ToArray()));
                yield return GameStartToLawnTransition();
            }
        }
        #endregion

        #region 游戏结束
        private IEnumerator GameOverByEnemyTransition()
        {
            Music.Stop();
            yield return new WaitForSeconds(1);
            yield return MoveCameraToHouse();
            yield return new WaitForSeconds(3);
            level.PlaySound(VanillaSoundID.hit);
            yield return new WaitForSeconds(0.5f);
            level.PlaySound(VanillaSoundID.hit);
            yield return new WaitForSeconds(0.5f);
            level.PlaySound(VanillaSoundID.hit);
            yield return new WaitForSeconds(0.5f);
            level.PlaySound(VanillaSoundID.scream);
            ui.ShowYouDied();
            yield return new WaitForSeconds(4);
            ShowGameOverDialog();
        }
        private IEnumerator GameOverNoEnemyTransition()
        {
            Music.Stop();
            level.PlaySound(VanillaSoundID.scream);
            ui.ShowYouDied();
            yield return new WaitForSeconds(4);
            ShowGameOverDialog();
        }
        #endregion

        #region 退出关卡
        private IEnumerator ExitLevelTransition(float delay)
        {
            yield return new WaitForSeconds(delay);
            Sounds.Play2D(VanillaSoundID.travel);
            Scene.PortalFadeIn(async () =>
            {
                await ExitLevel();
                Scene.PortalFadeOut();
            });
        }
        private IEnumerator ExitLevelToNoteTransition(NamespaceID noteID, float delay)
        {
            yield return new WaitForSeconds(delay);
            ui.SetExitingToNote();
            exitTargetNoteID = noteID;
        }
        private void StartExitLevelTransition(float delay)
        {
            var endNoteId = level.GetEndNoteID();
            if (NamespaceID.IsValid(endNoteId) && !level.IsRerun)
            {
                StartCoroutine(ExitLevelToNoteTransition(endNoteId, delay));
            }
            else
            {
                StartCoroutine(ExitLevelTransition(delay));
            }
        }
        #endregion

        private void UpdateCamera()
        {
            var targetRotation = level.GetCameraRotation();
            if (!IsGameStarted() || IsGameOver())
            {
                targetRotation = 0;
            }
            var rotation = levelCamera.GetRotation();
            levelCamera.SetSpace(Main.IsMobile() ? cameraLeftSpaceMobile : cameraLeftSpaceStandalone);
            levelCamera.SetRotation(rotation * 0.8f + targetRotation * 0.2f);

            var camera = levelCamera.Camera;
            var cameraHeight = camera.orthographicSize * 2;
            var cameraWidth = cameraHeight * camera.aspect;
            var cameraSize = new Vector2(cameraWidth, cameraHeight);
            var anchorOffset = levelCamera.CameraAnchor - Vector2.one * 0.5f;
            var cameraCenter = levelCamera.CameraPosition - (Vector3)(anchorOffset * cameraSize);
            var cameraMin = cameraCenter - (Vector3)(cameraSize * 0.5f);
            var width = (cameraLimitX - cameraMin.x) / cameraLimitX;
            var preset = ui.GetUIPreset();
            preset.SetCameraLimitWidth(width);
        }

        #endregion

        #region 属性字段

        [Header("Cameras")]
        [SerializeField]
        private LevelCamera levelCamera;
        [SerializeField]
        private AnimationCurve cameraMoveCurve;
        [SerializeField]
        private float cameraLimitX = 2.2f;
        [SerializeField]
        private float cameraLeftSpaceMobile = 2.2f;
        [SerializeField]
        private float cameraLeftSpaceStandalone = 0;
        [SerializeField]
        private Vector3 cameraHousePosition = new Vector3(0, 3, -10);
        [SerializeField]
        private Vector2 cameraHouseAnchor = new Vector2(0, 0.5f);
        [SerializeField]
        private Vector3 cameraLawnPosition = new Vector3(10.2f, 3, -10);
        [SerializeField]
        private Vector2 cameraLawnAnchor = new Vector2(1, 0.5f);
        [SerializeField]
        private Vector3 cameraChoosePosition = new Vector3(14, 3, -10);
        [SerializeField]
        private Vector2 cameraChooseAnchor = new Vector2(1, 0.5f);
        #endregion
    }
}
