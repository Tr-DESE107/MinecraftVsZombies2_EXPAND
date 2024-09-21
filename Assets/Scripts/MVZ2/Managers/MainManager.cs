using System;
using System.Threading.Tasks;
using PVZEngine.Game;
using UnityEngine;

namespace MVZ2
{
    public class MainManager : MonoBehaviour
    {
        public async Task Initialize()
        {
            Application.targetFrameRate = 60;
            SerializeHelper.init();

            Game = new Game();
            Game.OnGetString += OnGetStringCallback;
            Game.OnGetStringParticular += OnGetStringParticularCallback;

            OptionsManager.InitOptions();
            OptionsManager.LoadOptions();
            await ModManager.LoadMods(Game);
            await ResourceManager.LoadAllModResources();
            await LanguageManager.LoadAllLanguagePacks();

            SaveManager.Load();

            ModManager.InitMods(Game);
        }
        public bool IsMobile()
        {
#if UNITY_EDITOR
            switch (platformMode)
            {
                case PlatformMode.Mobile:
                    return true;
                case PlatformMode.Standalone:
                    return false;
            }
#endif

#if UNITY_ANDROID || UNITY_IOS
            return true;
#else
            return false;
#endif
        }
        public bool IsFastMode()
        {
#if UNITY_EDITOR
            return fastMode;
#else
            return false;
#endif
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
        private string OnGetStringCallback(string textKey)
        {
            return LanguageManager._(textKey);
        }
        private string OnGetStringParticularCallback(string textKey, string context)
        {
            return LanguageManager._p(context, textKey);
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
        public OptionsManager OptionsManager => options;
        public ResolutionManager ResolutionManager => resolution;
        public SceneLoadingManager SceneManager => sceneLoadingManager;
        public MainSceneController Scene => scene;
        [SerializeField]
        private string builtinNamespace = "mvz2";
        [SerializeField]
        private PlatformMode platformMode = PlatformMode.Default;
        [SerializeField]
        private bool fastMode;
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
        private OptionsManager options;
        [SerializeField]
        private ResolutionManager resolution;
        [SerializeField]
        private SceneLoadingManager sceneLoadingManager; 
        [SerializeField]
        private MainSceneController scene;
        public enum PlatformMode
        {
            Default,
            Mobile,
            Standalone
        }
    }
    public class DuplicateInstanceException : Exception
    {
        public DuplicateInstanceException(string instanceName) : base($"There's already an instance of {instanceName} in the game.")
        {

        }
    }
}
