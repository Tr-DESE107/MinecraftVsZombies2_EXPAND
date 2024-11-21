using MVZ2Logic;
using MVZ2Logic.Level.Components;
using PVZEngine;
using PVZEngine.Level;
using UnityEngine;

namespace MVZ2.Level.Components
{
    public partial class SoundComponent : MVZ2Component, ISoundComponent
    {
        public SoundComponent(LevelEngine level, LevelController controller) : base(level, componentID, controller)
        {
        }
        public void PlaySound(NamespaceID id, Vector3 position, float pitch = 1)
        {
            Main.SoundManager.Play(id, Controller.LawnToTrans(position), pitch, 1);
        }
        public void PlaySound(NamespaceID id, float pitch = 1)
        {
            Main.SoundManager.Play(id, Vector3.zero, pitch, 0);
        }
        public static readonly NamespaceID componentID = new NamespaceID(Builtin.spaceName, "sound");
    }
}