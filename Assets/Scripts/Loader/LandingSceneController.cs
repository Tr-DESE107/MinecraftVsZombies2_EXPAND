using System.Reflection;
using MVZ2.Managers;
using MVZ2.Modding;
using MVZ2.Vanilla;
using MVZ2Logic;
using MVZ2Logic.Definitions;
using PVZEngine;
using PVZEngine.Level;
using PVZEngine.Definitions;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

namespace MVZ2
{
    public class LandingSceneController : MonoBehaviour
    {
        void Start()
        {
            ModManager.OnRegisterMods += RegisterMod;
            Addressables.LoadSceneAsync("Main", LoadSceneMode.Single);
        }
        private static void RegisterMod(IModManager manager)
        {
            var mod = new VanillaMod();
            var assemblies = new Assembly[] { Assembly.GetAssembly(typeof(VanillaMod)), Assembly.GetAssembly(typeof(LogicMain)) };
            var main = MainManager.Instance;
            var game = main.Game;
            var modLoader = new ModLoader(main);
            modLoader.Load(mod, assemblies);
            mod.Init(game);
            manager.RegisterMod(mod);
        }
    }
}
