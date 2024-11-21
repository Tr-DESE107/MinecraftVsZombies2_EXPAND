using System.Collections.Generic;
using System.Linq;
using MVZ2.GameContent;
using MVZ2.Level.Components;
using MVZ2.Vanilla;
using PVZEngine;
using PVZEngine.Level;
using UnityEngine;

namespace MVZ2.Extensions
{
    public static partial class MVZ2Level
    {
        public static IAdviceComponent GetAdviceComponent(this LevelEngine level)
        {
            return level.GetComponent<IAdviceComponent>();
        }
        public static void ShowAdvice(this LevelEngine level, string context, string textKey, int priority, int timeout)
        {
            var component = level.GetAdviceComponent();
            component.ShowAdvice(context, textKey, priority, timeout);
        }
        public static void HideAdvice(this LevelEngine level)
        {
            var component = level.GetAdviceComponent();
            component.HideAdvice();
        }
    }
}
