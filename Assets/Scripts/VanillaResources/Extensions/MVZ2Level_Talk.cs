using MVZ2.Level.Components;
using PVZEngine;
using PVZEngine.Level;

namespace MVZ2.Extensions
{
    public static partial class MVZ2Level
    {
        public static ITalkComponent GetTalkComponent(this LevelEngine level)
        {
            return level.GetComponent<ITalkComponent>();
        }
        public static void StartTalk(this LevelEngine level, NamespaceID groupId, int section, float delay = 0)
        {
            var component = level.GetTalkComponent();
            component.StartTalk(groupId, section, delay);
        }
    }
}
