using System.Reflection;
using System.Threading.Tasks;
using MVZ2.Games;
using MVZ2.Managers;
using MVZ2.Modding;
using MVZ2.Vanilla;
using MVZ2Logic;
using PVZEngine;
using PVZEngine.Level;
using UnityEngine;

namespace MVZ2.Tests
{
    public class TestEntrance : MonoBehaviour
    {
        public async Task Init()
        {
            var levelEngineAssembly = typeof(LevelEngine).Assembly;
            var logicAssembly = typeof(LogicDefinitionTypes).Assembly;
            PropertyMapper.InitPropertyMaps("mvz2", levelEngineAssembly.GetTypes());
            PropertyMapper.InitPropertyMaps("mvz2", logicAssembly.GetTypes());
            ModManager.OnRegisterMod += RegisterMod;
            await main.Initialize();
            main.InitLoad();
            var initTask = main.GetInitTask();
            if (initTask != null)
            {
                await initTask;
            }
        }
        private static void RegisterMod(IModManager manager, Game game)
        {
            var mod = new VanillaMod();
            var assemblies = new Assembly[] { Assembly.GetAssembly(typeof(VanillaMod)) };
            mod.Init(game, assemblies);
            manager.RegisterModLogic(mod.Namespace, mod);
        }
        [SerializeField]
        private MainManager main;
    }
}
