using UnityEngine;

namespace MVZ2
{
    public class MainManager : MonoBehaviour
    {
        public ResourceManager ResourceManager => resource;
        public SoundManager SoundManager => sound;
        public LevelManager LevelManager => level;
        [SerializeField]
        private ResourceManager resource;
        [SerializeField]
        private SoundManager sound;
        [SerializeField]
        private LevelManager level;
    }
}
