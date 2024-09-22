using MVZ2.Managers;
using PVZEngine;
using UnityEngine;

namespace MVZ2.Sounds
{
    public class SoundPlayer : MonoBehaviour
    {
        public void Play2D()
        {
            MainManager.Instance.SoundManager.Play(soundID, Vector3.zero, pitch, 0);
        }
        [SerializeField]
        private NamespaceID soundID;
        [SerializeField]
        private float pitch = 1;
    }
}
