using System;
using System.Collections;
using System.Linq;
using MVZ2.Cameras;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Level;
using MVZ2.Vanilla.Saves;
using MVZ2Logic.Level;
using PVZEngine;
using UnityEngine;

namespace MVZ2.Level
{
    public partial class LevelController : MonoBehaviour, IDisposable
    {
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
        private IEnumerator MoveCameraToLawn()
        {
            return MoveCameraLawn(cameraLawnPosition, cameraLawnAnchor, 1f);
        }
        private IEnumerator MoveCameraToChoose()
        {
            return MoveCameraLawn(cameraChoosePosition, cameraChooseAnchor, 1f);
        }
        #endregion

        #region 游戏开始
        private void GameStartInstantTransition()
        {
            SetCameraPosition(LevelCameraPosition.Lawn);
            UpdateDifficulty();
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
        private IEnumerator GameStartToLawnTransition()
        {
            yield return MoveCameraToLawn();
            UpdateDifficulty();
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

            var unlocked = Saves.GetUnlockedContraptions();

            var seedSlotCount = level.GetSeedSlotCount();
            if (unlocked.Length > seedSlotCount && level.NeedBlueprints())
            {
                // 选卡。
                var uiPreset = GetUIPreset();
                uiPreset.SetSideUIBlend(0);
                uiPreset.SetBlueprintChooseBlend(0);
                ShowBlueprintChoosePanel(unlocked);
                uiPreset.SetReceiveRaycasts(false);
                yield return new WaitForSeconds(0.5f);
                uiPreset.SetReceiveRaycasts(true);
            }
            else
            {
                var seedPacks = unlocked.Take(seedSlotCount).ToArray();
                level.ReplaceSeedPacks(seedPacks);
                yield return GameStartToLawnTransition();
            }
        }
        private IEnumerator BlueprintChosenTransition()
        {
            var uiPreset = GetUIPreset();
            uiPreset.SetBlueprintsChooseVisible(false);
            uiPreset.SetReceiveRaycasts(false);

            yield return new WaitForSeconds(1);
            yield return GameStartToLawnTransition();
        }
        private IEnumerator BlueprintChooseViewLawnTransition()
        {
            var uiPreset = GetUIPreset();
            uiPreset.SetBlueprintsChooseVisible(false);
            uiPreset.SetReceiveRaycasts(false);

            yield return new WaitForSeconds(1);
            yield return MoveCameraToLawn();
            uiPreset.SetReceiveRaycasts(true);
            ui.SetViewLawnReturnBlockerActive(true);
            level.ShowAdvice(VanillaStrings.CONTEXT_ADVICE, VanillaStrings.ADVICE_CLICK_TO_CONTINUE, 1000, -1);
            while (!viewLawnFinished)
            {
                yield return null;
            }
            ui.SetViewLawnReturnBlockerActive(false);
            level.HideAdvice();
            yield return MoveCameraToChoose();
            uiPreset.SetBlueprintsChooseVisible(true);
            isViewingLawn = false;
            viewLawnFinished = false;
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
            if (NamespaceID.IsValid(endNoteId))
            {
                StartCoroutine(ExitLevelToNoteTransition(endNoteId, delay));
            }
            else
            {
                StartCoroutine(ExitLevelTransition(delay));
            }
        }
        #endregion

        #endregion

        #region 属性字段

        [Header("Cameras")]
        [SerializeField]
        private LevelCamera levelCamera;
        [SerializeField]
        private AnimationCurve cameraMoveCurve;
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
        private bool isViewingLawn;
        private bool viewLawnFinished;
        #endregion
    }
}
