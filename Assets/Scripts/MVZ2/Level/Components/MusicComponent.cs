using MVZ2.GameContent;
using MVZ2.Level;

namespace PVZEngine.Level
{
    public partial class MusicComponent : MVZ2Component
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
        public static readonly NamespaceID componentID = new NamespaceID(Builtin.spaceName, "music");
    }
}