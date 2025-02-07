using System.IO;
using MVZ2.Games;
using MVZ2.Modding;
using MVZ2.Vanilla;
using MVZ2Logic;
using PVZEngine;
using PVZEngine.Level;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

namespace MVZ2
{
    public class LandingSceneController : MonoBehaviour
    {
        void Start()
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                logger.gameObject.SetActive(true);
                DontDestroyOnLoad(logger.gameObject);
            }
            var levelEngineAssembly = typeof(LevelEngine).Assembly;
            var logicAssembly = typeof(LogicDefinitionTypes).Assembly;
            PropertyMapper.InitPropertyMaps(string.Empty, levelEngineAssembly.GetTypes());
            PropertyMapper.InitPropertyMaps(string.Empty, logicAssembly.GetTypes());
            ModManager.OnRegisterMod += RegisterMod;
            Addressables.LoadSceneAsync("Main", LoadSceneMode.Single);
        }
        private static void RegisterMod(IModManager manager, Game game)
        {
            var mod = new VanillaMod();
            manager.RegisterModLogic(mod.Namespace, mod);
        }
        [SerializeField]
        private AndroidLogger logger;
    }
}
