using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MukioI18n;
using MVZ2.Almanacs;
using MVZ2.Audios;
using MVZ2.Cameras;
using MVZ2.Collisions;
using MVZ2.Cursors;
using MVZ2.Games;
using MVZ2.IO;
using MVZ2.Level;
using MVZ2.Level.Components;
using MVZ2.Localization;
using MVZ2.Modding;
using MVZ2.Models;
using MVZ2.Options;
using MVZ2.Saves;
using MVZ2.Scenes;
using MVZ2.Supporters;
using MVZ2Logic;
using MVZ2Logic.Games;
using PVZEngine;
using UnityEditor;
using UnityEngine;

namespace MVZ2.Managers
{
    public class MainManager : MonoBehaviour, IMainManager
    {
        public async Task Initialize()
        {
            InitGameSettings();
            InitSerializable();
            await LoadManagersInit();
            Scene.Init();
            ModManager.PostGameInit();

            InitLoad();
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
        public TaskPipeline GetLoadPipeline()
        {
            return loadPipeline;
        }
        public Task GetInitTask()
        {
            return initTask;
        }
        public bool IsFastMode()
        {
#if UNITY_EDITOR
            return fastMode;
#else
            return false;
#endif
        }
        private void InitLoad()
        {
            loadPipeline = new TaskPipeline();
            loadPipeline.AddTask(new PipelineTask(TASK_LOAD_RESOURCES, (p) => ResourceManager.LoadAllModResourcesMain(p)));
            loadPipeline.AddTask(new PipelineTask(TASK_LOAD_SPONSORS, (p) => SponsorManager.PullSponsors(p)));

            var task = loadPipeline.Run();
            initTask = task;
            task.ContinueWith((t) =>
            {
                loadPipeline = null;
                initTask = null;
            });
        }

        #region 贴图
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
        #endregion

        #region 文本
        public string GetFloatPercentageText(float value)
        {
            return LanguageManager._(VALUE_PERCENT, Mathf.RoundToInt(value * 100));
        }
        #endregion

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
        private void InitGameSettings()
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
            Application.targetFrameRate = 60;

            Global.Init(this);
            Game = new Game(BuiltinNamespace, LanguageManager, SaveManager, ResourceManager);
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

            SerializeHelper.RegisterClass<SerializableUnityCollisionSystem>();
            SerializeHelper.RegisterClass<SerializableUnityCollisionEntity>();
            SerializeHelper.RegisterClass<SerializableUnityEntityCollider>();
        }
        private async Task LoadManagersInit()
        {
            GraphicsManager.Init();
            InputManager.InitKeys();
            OptionsManager.InitOptions();
            OptionsManager.LoadOptions();

            await ModManager.LoadModInfos(Game);

            // 在MOD信息加载之后
            await ResourceManager.InitModResources();
            await LanguageManager.InitLanguagePacks();

            // 在MOD资源加载之后
            ModManager.InitModLogics(Game);
            ModManager.LoadModLogics(Game);
            ModManager.PostReloadMods(Game);

            // 在MOD逻辑加载之后
            SaveManager.Load();
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

        [TranslateMsg("初始化任务名称")]
        public const string TASK_LOAD_RESOURCES = "加载中……";
        [TranslateMsg("初始化任务名称")]
        public const string TASK_LOAD_SPONSORS = "获取赞助者列表……";
        [TranslateMsg("值，{0}为百分数")]
        public const string VALUE_PERCENT = "{0}%";
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
        public GraphicsManager GraphicsManager => graphicsManager;
        public MainSceneController Scene => scene;
        ISceneController IMainManager.Scene => scene;
        IMusicManager IMainManager.Music => music;
        ILevelManager IMainManager.Level => level;
        IOptionsManager IMainManager.Options => options;
        IGlobalSave IMainManager.Saves => save;

        private Task initTask;
        private TaskPipeline loadPipeline;
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
        private GraphicsManager graphicsManager;
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
    public class TaskPipeline
    {
        public async Task Run()
        {
            SetCurrentTask(0);
            for (int i = 0; i < tasks.Count; i++)
            {
                var task = tasks[i];
                await task.Run();
                SetCurrentTask(i + 1);
            }
        }
        public void AddTask(PipelineTask child)
        {
            tasks.Add(child);
        }
        public void RemoveTask(PipelineTask child)
        {
            tasks.Remove(child);
        }
        public void SetCurrentTask(int index)
        {
            currentTaskIndex = index;
        }
        public bool IsFinished()
        {
            return currentTaskIndex >= tasks.Count;
        }
        public string GetCurrentTaskName()
        {
            var task = GetCurrentTask();
            if (task == null)
                return string.Empty;
            return task.GetName();
        }
        public string GetCurrentProgressName()
        {
            var task = GetCurrentTask();
            if (task == null)
                return string.Empty;
            return task.GetProgressName();
        }
        public float GetProgress()
        {
            if (tasks.Count > 0)
            {
                return tasks.Sum(c => c.GetProgress()) / tasks.Count;
            }
            return 1;
        }
        private PipelineTask GetCurrentTask()
        {
            if (currentTaskIndex < 0 || currentTaskIndex >= tasks.Count)
                return null;
            return tasks[currentTaskIndex];
        }
        private int currentTaskIndex;
        private List<PipelineTask> tasks = new List<PipelineTask>();
        private float progress;
    }
    public delegate Task TaskAction(TaskProgress progress);
    public class PipelineTask
    {
        public PipelineTask(string name, TaskAction action) : this(name, action, new TaskProgress())
        {
        }
        public PipelineTask(string name, TaskAction action, TaskProgress progress)
        {
            this.name = name;
            this.action = action;
            this.progress = progress;
        }
        public float GetProgress()
        {
            return progress.GetProgress();
        }
        public Task Run()
        {
            return action?.Invoke(progress);
        }
        public string GetName()
        {
            return name;
        }
        public string GetProgressName()
        {
            return progress.GetCurrentTaskName();
        }
        private string name;
        private TaskProgress progress;
        private TaskAction action;
    }
    public class TaskProgress
    {
        public TaskProgress()
        {
        }
        public TaskProgress AddChild()
        {
            var progress = new TaskProgress();
            children.Add(progress);
            return progress;
        }
        public TaskProgress[] AddChildren(int count)
        {
            var array = new TaskProgress[count];
            for (int i = 0; i < count; i++)
            {
                array[i] = AddChild();
            }
            return array;
        }
        public float GetProgress()
        {
            if (children.Count > 0)
            {
                return children.Sum(c => c.GetProgress()) / children.Count;
            }
            return progress;
        }
        public void SetProgress(float progress)
        {
            this.progress = progress;
        }
        public void SetProgress(float progress, string taskName)
        {
            SetProgress(progress);
            SetCurrentTaskName(taskName);
        }
        public string GetCurrentTaskName()
        {
            if (children.Count > 0)
            {
                var name = children.Where(c => c.progress < 1).Select(e => e.GetCurrentTaskName()).FirstOrDefault(c => !string.IsNullOrEmpty(c));
                if (!string.IsNullOrEmpty(taskName))
                {
                    if (string.IsNullOrEmpty(name))
                        return $"{taskName}/";
                    else
                        return $"{taskName}/{name}";
                }
                else
                {
                    if (string.IsNullOrEmpty(name))
                        return string.Empty;
                    else
                        return name;
                }
            }
            return taskName;
        }
        public void SetCurrentTaskName(string name)
        {
            this.taskName = name;
        }
        private float progress;
        private string taskName;
        private List<TaskProgress> children = new List<TaskProgress>();
    }
}
