using System;
using UnityEngine;

namespace MVZ2.Level
{
    public partial class LevelController
    {
        public ILevelBlueprintController BlueprintController => blueprintController;
        public ILevelBlueprintChooseController BlueprintChoosePart => blueprintChooseController;
        [SerializeField]
        LevelBlueprintController blueprintController;
        [SerializeField]
        LevelBlueprintChooseController blueprintChooseController;
    }
}
