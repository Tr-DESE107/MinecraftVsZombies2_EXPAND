using MVZ2Logic.Level.Components;
using PVZEngine;
using PVZEngine.Level;

namespace MVZ2Logic.Level
{
    public static partial class LogicLevelExt
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
        public static void SetMusicVolume(this LevelEngine level, float volume)
        {
            var component = level.GetMusicComponent();
            component.SetMusicVolume(volume);
        }
        public static float GetMusicVolume(this LevelEngine level)
        {
            var component = level.GetMusicComponent();
            return component.GetMusicVolume();
        }
    }
}