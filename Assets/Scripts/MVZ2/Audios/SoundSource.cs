using PVZEngine;
using UnityEngine;

namespace MVZ2.Audios
{
    public class SoundSource : MonoBehaviour
    {
        public NamespaceID SoundID { get; set; }
        public AudioSource AudioSource => audioSource;
        [SerializeField]
        private AudioSource audioSource;
    }
}
