using PVZEngine.Game;
using UnityEngine;

namespace MVZ2.Vanilla.Loader
{
    public class VanillaLoader : MonoBehaviour
    {
        void Start()
        {
            ModManager.OnRegisterMod += RegisterMod;
        }
        private static void RegisterMod(IModManager manager, Game game)
        {
            manager.RegisterModLogic(VanillaMod.spaceName, new VanillaMod(game));
        }
    }
}
