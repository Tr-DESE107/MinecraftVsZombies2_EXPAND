using System;
using log4net.Core;
using MVZ2.GameContent;
using MVZ2.Level;
using PVZEngine.Definitions;
using UnityEngine;

namespace PVZEngine.Level
{
    public partial class SoundComponent : MVZ2Component
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