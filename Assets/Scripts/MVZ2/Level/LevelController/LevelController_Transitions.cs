﻿using System.Collections;
using System.Linq;
using MVZ2.Level.UI;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Level;
using MVZ2Logic.Callbacks;
using MVZ2Logic.Games;
using MVZ2Logic.HeldItems;
using MVZ2Logic.Level;
using MVZ2Logic.SeedPacks;
using PVZEngine;
using PVZEngine.Callbacks;
using UnityEngine;

namespace MVZ2.Level
{
    public partial class LevelController
    {
        #region 游戏开始
        private void GameStartInstantTransition()
        {
            SetCameraPosition(LevelCameraPosition.Lawn);
            UpdateDifficulty();
            Game.RunCallback(LogicLevelCallbacks.PRE_BATTLE, new LevelCallbackParams(level));
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
            Game.RunCallback(LogicLevelCallbacks.PRE_BATTLE, new LevelCallbackParams(level));
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
                UpdateEntityHeldTargetColliders(HeldTargetFlag.Enemy);
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
                var innateChooseItems = innateBlueprints.Select(i => new BlueprintChooseItem(i, innate: true));
                var contraptionChooseItems = unlockedContraptions.Take(seedSlotCount).Select(i => new BlueprintChooseItem(i));
                var chooseItems = innateChooseItems.Concat(contraptionChooseItems);
                level.SetupBattleBlueprints(chooseItems.ToArray());
                Game.RunCallback(LogicLevelCallbacks.POST_BLUEPRINT_SELECTION, new LogicLevelCallbacks.PostBlueprintSelectionParams(level, chooseItems.ToArray()));
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
    }
}
