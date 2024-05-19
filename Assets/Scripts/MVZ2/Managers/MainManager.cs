using UnityEngine;

namespace MVZ2
{
    public class MainManager : MonoBehaviour
    {
        public ResourceManager ResourceManager => resource;
        public LevelManager LevelManager => level;
        [SerializeField]
        private ResourceManager resource;
        [SerializeField]
        private LevelManager level;
    }
}
