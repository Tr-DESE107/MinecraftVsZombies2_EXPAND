using System.Collections;
using MVZ2.Games;
using PVZEngine.Level;
using UnityEngine;

namespace MVZ2.Level
{
    public interface ILevelController : ILevelUIController, ILevelTransitionController
    {
        Game Game { get; }
        ILevelUI GetUI();
        LevelEngine GetEngine();
        Camera GetCamera();
        ILevelBlueprintController BlueprintController { get; }
        ILevelBlueprintChooseController BlueprintChoosePart { get; }

        bool CanChooseBlueprints();
        void OpenAlmanac();
        void OpenStore();
        bool IsOpeningExtraScene();

        float GetTwinkleAlpha();
    }
    public interface ILevelUIController
    {
        void ShowTooltip(ITooltipSource source);
        void HideTooltip();
    }
    public interface ILevelTransitionController
    {
        IEnumerator GameStartToLawnTransition();
        IEnumerator MoveCameraToLawn();
        IEnumerator MoveCameraToChoose();
    }
}
