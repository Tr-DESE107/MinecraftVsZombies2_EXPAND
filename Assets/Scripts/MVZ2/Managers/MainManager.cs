using UnityEngine;

namespace MVZ2
{
    public class MainManager : MonoBehaviour
    {
        private async void Start()
        {
            await ModManager.LoadMods();
            await ResourceManager.LoadAllModResources();
            LevelManager.StartLevel();
        }
        public string BuiltinNamespace => builtinNamespace;
        public ResourceManager ResourceManager => resource;
        public ModelManager ModelManager => model;
        public SoundManager SoundManager => sound;
        public LevelManager LevelManager => level;
        public ModManager ModManager => mod;
        [SerializeField]
        private string builtinNamespace = "mvz2";
        [SerializeField]
        private ResourceManager resource;
        [SerializeField]
        private ModelManager model;
        [SerializeField]
        private SoundManager sound;
        [SerializeField]
        private LevelManager level;
        [SerializeField]
        private ModManager mod;
    }
}
