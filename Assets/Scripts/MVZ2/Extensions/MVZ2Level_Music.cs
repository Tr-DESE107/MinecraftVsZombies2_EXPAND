using PVZEngine;
using PVZEngine.Level;

namespace MVZ2
{
    public static partial class MVZ2Level
    {
        public static MusicComponent GetMusicComponent(this LevelEngine level)
        {
            return level.GetComponent<MusicComponent>();
        }
        public static void PlayMusic(this LevelEngine level, NamespaceID id)
        {
            var component = level.GetMusicComponent();
            component.Play(id);
        }
        public static void Stop(this LevelEngine level, NamespaceID id)
        {
            var component = level.GetMusicComponent();
            component.Stop();
        }
    }
}