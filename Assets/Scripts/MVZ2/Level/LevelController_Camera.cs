using System;
using System.Collections;
using MVZ2.Extensions;
using MVZ2.GameContent;
using MVZ2.Level.UI;
using MVZ2.Vanilla;
using PVZEngine;
using UnityEngine;

namespace MVZ2.Level
{
    public partial class LevelController : MonoBehaviour, IDisposable
    {
        #region 私有方法

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
        private void GameInstantTransition()
        {
            SetCameraPosition(LevelCameraPosition.Lawn);
            StartGame();
        }
        private IEnumerator GameStartToLawnTransition()
        {
            Main.MusicManager.Play(MusicID.choosing);
            yield return new WaitForSeconds(1);
            yield return MoveCameraToLawn();
            yield return new WaitForSeconds(0.5f);
            StartGame();
        }
        private IEnumerator GameStartTransition()
        {
            Main.MusicManager.Play(MusicID.choosing);
            if (level.StageDefinition is IPreviewStage preview)
            {
                preview.CreatePreviewEnemies(level, BuiltinLevel.GetEnemySpawnRect());
            }
            yield return new WaitForSeconds(1);
            yield return MoveCameraToChoose();
            yield return new WaitForSeconds(1);

            var seedPacks = Main.LevelManager.GetSeedPacksID();
            level.SetSeedPackCount(seedPacks.Length);
            level.ReplaceSeedPacks(seedPacks);
            level.SetDifficulty(Main.OptionsManager.GetDifficulty());

            yield return MoveCameraToLawn();
            level.PrepareForBattle();
            yield return new WaitForSeconds(0.5f);
            PlayReadySetBuild();
        }
        private IEnumerator GameOverByEnemyTransition()
        {
            Main.MusicManager.Stop();
            yield return new WaitForSeconds(1);
            yield return MoveCameraToHouse();
            yield return new WaitForSeconds(3);
            level.PlaySound(SoundID.hit);
            yield return new WaitForSeconds(0.5f);
            level.PlaySound(SoundID.hit);
            yield return new WaitForSeconds(0.5f);
            level.PlaySound(SoundID.hit);
            yield return new WaitForSeconds(0.5f);
            level.PlaySound(SoundID.scream);
            var levelUI = GetLevelUI();
            levelUI.SetYouDiedVisible(true);
            yield return new WaitForSeconds(4);
            ShowGameOverDialog();
        }
        private IEnumerator GameOverNoEnemyTransition()
        {
            Main.MusicManager.Stop();
            level.PlaySound(SoundID.scream);
            var levelUI = GetLevelUI();
            levelUI.SetYouDiedVisible(true);
            yield return new WaitForSeconds(4);
            ShowGameOverDialog();
        }
        private IEnumerator ExitLevelTransition(float delay)
        {
            yield return new WaitForSeconds(delay);
            Main.SoundManager.Play2D(SoundID.travel);
            Main.Scene.SetPortalFadeIn(async () =>
            {
                await ExitLevel();
                Main.Scene.SetPortalFadeOut();
            });
        }
        private IEnumerator ExitLevelToNoteTransition(NamespaceID noteID, float delay)
        {
            yield return new WaitForSeconds(delay);
            var levelUI = GetLevelUI();
            levelUI.SetExitingToNote();
            exitTargetNoteID = noteID;
        }
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
        #endregion
    }
    public enum LevelCameraPosition
    {
        House,
        Lawn,
        Choose,
    }
}
