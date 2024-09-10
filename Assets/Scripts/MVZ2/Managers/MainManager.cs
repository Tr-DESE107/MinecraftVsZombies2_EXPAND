using System;
using System.Threading.Tasks;
using MVZ2.Vanilla;
using PVZEngine.Game;
using UnityEngine;

namespace MVZ2
{
    public class MainManager : MonoBehaviour
    {
        public async Task Initialize()
        {
            await ModManager.LoadMods();
            await ResourceManager.LoadAllModResources();
            LanguageManager.LoadAllLanguagePacks();

            Game = new Game();
            var mod = new VanillaMod(Game);
            Game.AddMod(mod);
        }
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
        public static MainManager Instance { get; private set; }
        public Game Game { get; private set; }
        public string BuiltinNamespace => builtinNamespace;
        public ResourceManager ResourceManager => resource;
        public ModelManager ModelManager => model;
        public SoundManager SoundManager => sound;
        public MusicManager MusicManager => music;
        public LevelManager LevelManager => level;
        public LanguageManager LanguageManager => lang;
        public SaveManager SaveManager => save;
        public ModManager ModManager => mod;
        public CursorManager CursorManager => cursor;
        public ShakeManager ShakeManager => shake;
        public TalkManager TalkManager => talk;
        public MainSceneController Scene => scene;
        [SerializeField]
        private string builtinNamespace = "mvz2";
        [SerializeField]
        private ResourceManager resource;
        [SerializeField]
        private ModelManager model;
        [SerializeField]
        private SoundManager sound;
        [SerializeField]
        private MusicManager music;
        [SerializeField]
        private LevelManager level;
        [SerializeField]
        private LanguageManager lang;
        [SerializeField]
        private SaveManager save;
        [SerializeField]
        private ModManager mod;
        [SerializeField]
        private CursorManager cursor;
        [SerializeField]
        private ShakeManager shake;
        [SerializeField]
        private TalkManager talk;
        [SerializeField]
        private MainSceneController scene;
    }
    public class DuplicateInstanceException : Exception
    {
        public DuplicateInstanceException(string instanceName) : base($"There's already an instance of {instanceName} in the game.")
        {

        }
    }
}
