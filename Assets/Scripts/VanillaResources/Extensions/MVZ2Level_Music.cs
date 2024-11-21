using MVZ2.Level.Components;
using PVZEngine;
using PVZEngine.Level;

namespace MVZ2.Extensions
{
    public static partial class MVZ2Level
    {
        public static IMusicComponent GetMusicComponent(this LevelEngine level)
        {
            return level.GetComponent<IMusicComponent>();
        }
        public static void PlayMusic(this LevelEngine level, NamespaceID id)
        {
            var component = level.GetMusicComponent();
            component.Play(id);
        }
        public static void StopMusic(this LevelEngine level)
        {
            var component = level.GetMusicComponent();
            component.Stop();
        }
    }
}