using System;
using CustomBuilder;
using MVZ2.Editor;
using UnityEditor;
using UnityEngine;

namespace MVZ2.Editor
{
    [BuilderStep]
    public class StepUpdateAssets : IBuilderStep
    {
        public void ActionAfter()
        {
        }

        public void ActionBefore(object option)
        {
            AssetsMenu.UpdateAllAssets();
        }
    }
}
