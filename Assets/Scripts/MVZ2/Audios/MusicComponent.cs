using MVZ2.Vanilla;
using MVZ2Logic.Level;
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
        public override void Update()
        {
            base.Update();
            Controller.SetMusicLowQuality(Level.IsMusicLowQuality());
        }
        public void Play(NamespaceID id)
        {
            Main.MusicManager.Play(id);
        }
        public void Stop()
        {
            Main.MusicManager.Stop();
        }
        public bool IsPlayingMusic(NamespaceID id)
        {
            return Main.MusicManager.IsPlaying(id);
        }
        public void SetPlayingMusic(NamespaceID id)
        {
            Main.MusicManager.SetPlayingMusic(id);
        }
        public float GetMusicVolume()
        {
            return Controller.MusicVolume;
        }
        public void SetMusicVolume(float volume)
        {
            Controller.MusicVolume = volume;
        }
        public float GetSubtrackWeight()
        {
            return Controller.MusicTrackWeight;
        }
        public void SetSubtrackWeight(float weight)
        {
            Controller.MusicTrackWeight = weight;
        }
        public static readonly NamespaceID componentID = new NamespaceID(VanillaMod.spaceName, "music");
    }
}