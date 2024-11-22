using MVZ2.Managers;
using PVZEngine;
using UnityEngine;

namespace MVZ2.Sounds
{
    public class SoundPlayer : MonoBehaviour
    {
        public void Play2D()
        {
            Play2D(soundID);
        }
        public void PlaySound2D(string idString)
        {
            Play2D(NamespaceID.Parse(idString, MainManager.Instance.BuiltinNamespace));
        }
        public void Play2D(NamespaceID id)
        {
            MainManager.Instance.SoundManager.Play(id, Vector3.zero, pitch, 0);
        }
        [SerializeField]
        private NamespaceID soundID;
        [SerializeField]
        private float pitch = 1;
    }
}
