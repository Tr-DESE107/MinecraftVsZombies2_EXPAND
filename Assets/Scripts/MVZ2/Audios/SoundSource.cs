using MVZ2.UI;
using PVZEngine;
using UnityEngine;

namespace MVZ2.Audios
{
    public class SoundSource : MonoBehaviour
    {
        public void StartFade(float target, float time)
        {
            volumeFader.StartFade(target, time);
        }
        public void StopFade()
        {
            volumeFader.StopFade();
        }
        private void Awake()
        {
            volumeFader.OnValueChanged += v => audioSource.volume = v;
        }
        public NamespaceID SoundID { get; set; }
        public AudioSource AudioSource => audioSource;
        public float Intensity { get; set; }
        public float Volume
        {
            get => volumeFader.Value;
            set => volumeFader.Value = value;
        }
        public float Pitch
        {
            get => audioSource.pitch;
            set => audioSource.pitch = value;
        }
        [SerializeField]
        private AudioSource audioSource;
        [SerializeField]
        private FloatFader volumeFader;
    }
}
