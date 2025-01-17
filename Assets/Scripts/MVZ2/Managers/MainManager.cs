using System;
using System.Threading.Tasks;
using MVZ2.Almanacs;
using MVZ2.Assets.Scripts.MVZ2.Managers;
using MVZ2.Audios;
using MVZ2.Cameras;
using MVZ2.Cursors;
using MVZ2.Games;
using MVZ2.IO;
using MVZ2.Level;
using MVZ2.Level.Components;
using MVZ2.Localization;
using MVZ2.Modding;
using MVZ2.Models;
using MVZ2.Options;
using MVZ2.Save;
using MVZ2.Saves;
using MVZ2.Scenes;
using MVZ2.Supporters;
using MVZ2Logic;
using MVZ2Logic.Games;
using PVZEngine;
using UnityEngine;

namespace MVZ2.Managers
{
    public class MainManager : MonoBehaviour, IMainManager
    {
        public async Task Initialize()
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
            Application.targetFrameRate = 60;

            InitSerializable();

            Global.Init(this);
            Game = new Game(BuiltinNamespace, LanguageManager, SaveManager, ResourceManager);

            OptionsManager.InitOptions();
            OptionsManager.LoadOptions();

            await ModManager.LoadMods(Game);

            // 在MOD信息加载之后
            await ResourceManager.LoadAllModResources();
            await LanguageManager.LoadAllLanguagePacks();

            // 在MOD资源加载之后
            ModManager.LoadModLogics(Game);

            // 在MOD逻辑加载之后
            SaveManager.Load();

            await SponsorManager.PullSponsors();

            Scene.Init();

            ModManager.PostGameInit();
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
        public Sprite GetFinalSprite(SpriteReference spriteRef)
        {
            if (!SpriteReference.IsValid(spriteRef))
                return null;
            if (!OptionsManager.HasBloodAndGore())
            {
                var id = spriteRef.id;
                var censoredId = new NamespaceID(id.SpaceName, $"{id.Path}_censored");
                SpriteReference censoredRef;
                if (spriteRef.isSheet)
                {
                    censoredRef = new SpriteReference(censoredId, spriteRef.index);
                }
                else
                {
                    censoredRef = new SpriteReference(censoredId);
                }
                var sprite = LanguageManager.GetCurrentLanguageSprite(censoredRef) ?? ResourceManager.GetSprite(censoredRef);
                if (sprite)
                    return sprite;
            }
            return LanguageManager.GetCurrentLanguageSprite(spriteRef) ?? ResourceManager.GetSprite(spriteRef);
        }
        public Sprite GetFinalSprite(Sprite sprite)
        {
            var spriteID = ResourceManager.GetSpriteReference(sprite);
            if (!SpriteReference.IsValid(spriteID))
                return sprite;
            return GetFinalSprite(spriteID) ?? sprite;
        }
        public Sprite GetFinalSprite(SpriteReference spriteRef, string language)
        {
            if (!OptionsManager.HasBloodAndGore())
            {
                var id = spriteRef.id;
                var censoredId = new NamespaceID(id.SpaceName, $"{id.Path}_censored");
                SpriteReference censoredRef;
                if (spriteRef.isSheet)
                {
                    censoredRef = new SpriteReference(censoredId, spriteRef.index);
                }
                else
                {
                    censoredRef = new SpriteReference(censoredId);
                }
                var sprite = LanguageManager.GetLocalizedSprite(censoredRef, language) ?? ResourceManager.GetSprite(censoredRef);
                if (sprite)
                    return sprite;
            }
            return LanguageManager.GetLocalizedSprite(spriteRef, language) ?? ResourceManager.GetSprite(spriteRef);
        }
        public Sprite GetFinalSprite(Sprite sprite, string language)
        {
            var spriteID = ResourceManager.GetSpriteReference(sprite);
            if (!SpriteReference.IsValid(spriteID))
                return sprite;
            return GetFinalSprite(spriteID, language) ?? sprite;
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
        private void OnApplicationQuit()
        {
            SaveManager.SaveModDatas();
        }
        private void InitSerializable()
        {
            SerializeHelper.init(BuiltinNamespace);
            // MVZ2
            SerializeHelper.RegisterClass<SerializableUserDataList>();
            SerializeHelper.RegisterClass<SerializableSaveDataMeta>();
            SerializeHelper.RegisterClass<SerializableAdviceComponent>();
            SerializeHelper.RegisterClass<SerializableArtifactComponent>();
            SerializeHelper.RegisterClass<SerializableLightComponent>();
            SerializeHelper.RegisterClass<SerializableUIComponent>();
            SerializeHelper.RegisterClass<SerializableSoundComponent>();
            SerializeHelper.RegisterClass<SerializableBlueprintComponent>();
            SerializeHelper.RegisterClass<EmptySerializableLevelComponent>();

            SerializeHelper.RegisterClass<SerializableLevelControllerPart>();
            SerializeHelper.RegisterClass<SerializableLevelBlueprintController>();
            SerializeHelper.RegisterClass<SerializableLevelBlueprintChooseController>();

            SerializeHelper.RegisterClass<SerializableBlueprintController>();
            SerializeHelper.RegisterClass<SerializableClassicBlueprintController>();
            SerializeHelper.RegisterClass<SerializableConveyorBlueprintController>();

            SerializeHelper.RegisterClass<SerializableModelData>();
            SerializeHelper.RegisterClass<SerializableSpriteModelData>();
            SerializeHelper.RegisterClass<SerializableUIModelData>();

            SerializeHelper.RegisterClass<SerializableModelGraphicGroup>();
            SerializeHelper.RegisterClass<SerializableModelRendererGroup>();
            SerializeHelper.RegisterClass<SerializableModelImageGroup>();

            SerializeHelper.RegisterClass<SerializableGraphicElement>();
            SerializeHelper.RegisterClass<SerializableRendererElement>();
            SerializeHelper.RegisterClass<SerializableImageElement>();
        }
        private void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            if (e.Exception == null)
            {
                Debug.LogError(e);
                return;
            }
            if (e.Exception.InnerException != null)
            {
                Debug.LogError(e.Exception.InnerException);
                return;
            }
            if (e.Exception.InnerExceptions != null)
            {
                foreach (var exception in e.Exception.InnerExceptions)
                {
                    Debug.LogError(exception);
                }
                return;
            }
            Debug.LogError(e.Exception);
        }
        IGame IMainManager.Game => Game;
        public static MainManager Instance { get; private set; }
        public Game Game { get; private set; }
        public string BuiltinNamespace => builtinNamespace;
        public CoroutineManager CoroutineManager => coroutine;
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
        public FileManager FileManager => file;
        public OptionsManager OptionsManager => options;
        public ResolutionManager ResolutionManager => resolution;
        public SceneLoadingManager SceneManager => sceneLoadingManager;
        public AlmanacManager AlmanacManager => almanacManager;
        public StoreManager StoreManager => storeManager;
        public InputManager InputManager => inputManager;
        public SponsorManager SponsorManager => sponsorManager;
        public MainSceneController Scene => scene;
        ISceneController IMainManager.Scene => scene;
        IMusicManager IMainManager.Music => music;
        ILevelManager IMainManager.Level => level;
        IOptionsManager IMainManager.Options => options;
        IGlobalSave IMainManager.Saves => save;
        [SerializeField]
        private string builtinNamespace = "mvz2";
        [SerializeField]
        private PlatformMode platformMode = PlatformMode.Default;
        [SerializeField]
        private bool fastMode;
        [SerializeField]
        private CoroutineManager coroutine;
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
        private FileManager file;
        [SerializeField]
        private OptionsManager options;
        [SerializeField]
        private ResolutionManager resolution;
        [SerializeField]
        private SceneLoadingManager sceneLoadingManager;
        [SerializeField]
        private AlmanacManager almanacManager;
        [SerializeField]
        private StoreManager storeManager;
        [SerializeField]
        private InputManager inputManager;
        [SerializeField]
        private SponsorManager sponsorManager;
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
