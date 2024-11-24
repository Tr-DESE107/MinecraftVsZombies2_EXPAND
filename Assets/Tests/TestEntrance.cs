using System.Threading.Tasks;
using MVZ2.Games;
using MVZ2.Managers;
using MVZ2.Modding;
using MVZ2.Vanilla;
using UnityEngine;

namespace MVZ2.Tests
{
    public class TestEntrance : MonoBehaviour
    {
        public async Task Init()
        {
            ModManager.OnRegisterMod += RegisterMod;
            await main.Initialize();
        }
        private static void RegisterMod(IModManager manager, Game game)
        {
            manager.RegisterModLogic(VanillaMod.spaceName, new VanillaMod());
        }
        [SerializeField]
        private MainManager main;
    }
}
