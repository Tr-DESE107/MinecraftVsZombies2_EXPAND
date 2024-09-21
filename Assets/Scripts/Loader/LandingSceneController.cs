using MVZ2.Vanilla;
using PVZEngine.Game;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

namespace MVZ2
{
    public class LandingSceneController : MonoBehaviour
    {
        void Start()
        {
            ModManager.OnRegisterMod += RegisterMod;
            Addressables.LoadSceneAsync("Main", LoadSceneMode.Single);
        }
        private static void RegisterMod(IModManager manager, Game game)
        {
            manager.RegisterModLogic(VanillaMod.spaceName, new VanillaMod(game));
        }
    }
}
