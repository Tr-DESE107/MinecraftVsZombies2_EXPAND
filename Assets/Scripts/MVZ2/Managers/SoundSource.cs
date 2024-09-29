using PVZEngine;
using UnityEngine;

namespace MVZ2.Assets.Scripts.MVZ2.Managers
{
    public class SoundSource : MonoBehaviour
    {
        public NamespaceID SoundID { get; set; }
        public AudioSource AudioSource => audioSource;
        [SerializeField]
        private AudioSource audioSource;
    }
}
