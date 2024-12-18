using MVZ2.Vanilla;
using MVZ2Logic.Level.Components;
using PVZEngine;
using PVZEngine.Level;

namespace MVZ2.Level.Components
{
    public partial class MusicComponent : MVZ2Component, IMusicComponent
    {
        public MusicComponent(LevelEngine level, LevelController controller) : base(level, componentID, controller)
        {
        }
        public void Play(NamespaceID id)
        {
            Main.MusicManager.Play(id);
        }
        public void Stop()
        {
            Main.MusicManager.Stop();
        }
        public void SetMusicVolume(float volume)
        {
            Main.MusicManager.SetVolume(volume);
        }
        public float GetMusicVolume()
        {
            return Main.MusicManager.GetVolume();
        }
        public static readonly NamespaceID componentID = new NamespaceID(VanillaMod.spaceName, "music");
    }
}