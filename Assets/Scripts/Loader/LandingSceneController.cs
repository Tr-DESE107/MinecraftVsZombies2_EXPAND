using System.IO;
using System.Reflection;
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
            var levelEngineAssembly = typeof(LevelEngine).Assembly;
            var logicAssembly = typeof(LogicDefinitionTypes).Assembly;
            PropertyMapper.InitPropertyMaps("mvz2", levelEngineAssembly.GetTypes());
            PropertyMapper.InitPropertyMaps("mvz2", logicAssembly.GetTypes());
            ModManager.OnRegisterMod += RegisterMod;
            Addressables.LoadSceneAsync("Main", LoadSceneMode.Single);
        }
        private static void RegisterMod(IModManager manager, Game game)
        {
            var mod = new VanillaMod();
            var assemblies = new Assembly[] { Assembly.GetAssembly(typeof(VanillaMod)) };
            mod.Init(game, assemblies);
            manager.RegisterModLogic(mod.Namespace, mod);
        }
    }
}
