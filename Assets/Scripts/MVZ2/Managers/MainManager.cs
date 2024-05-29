using System;
using UnityEngine;

namespace MVZ2
{
    public class MainManager : MonoBehaviour
    {
        private void Awake()
        {
            if (!Instance)
            {
                Instance = this;
            }
            else
            {
                throw new DuplicateInstanceException(name);
            }
        }
        private async void Start()
        {
            await ModManager.LoadMods();
            await ResourceManager.LoadAllModResources();
            LanguageManager.LoadAllLanguagePacks();
            await LevelManager.GotoLevelScene();
            LevelManager.StartLevel();
        }
        public static MainManager Instance { get; private set; }
        public string BuiltinNamespace => builtinNamespace;
        public ResourceManager ResourceManager => resource;
        public ModelManager ModelManager => model;
        public SoundManager SoundManager => sound;
        public LevelManager LevelManager => level;
        public LanguageManager LanguageManager => lang;
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
        private LanguageManager lang;
        [SerializeField]
        private ModManager mod;
    }
    public class DuplicateInstanceException : Exception
    {
        public DuplicateInstanceException(string instanceName) : base($"There's already an instance of {instanceName} in the game.")
        {

        }
    }
}
