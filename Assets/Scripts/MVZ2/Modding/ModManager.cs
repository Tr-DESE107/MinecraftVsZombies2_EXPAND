using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MVZ2.Games;
using MVZ2.Level;
using MVZ2.Managers;
using MVZ2Logic.Modding;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace MVZ2.Modding
{
    public class ModManager : MonoBehaviour, IModManager
    {
        public async Task LoadMods(Game game)
        {
            var locator = await Addressables.InitializeAsync().Task;
            modInfos.Add(new ModInfo()
            {
                Namespace = main.BuiltinNamespace,
                LevelDataVersion = LevelManager.CURRENT_DATA_VERSION,
                DisplayName = "Vanilla",
                CatalogPath = null,
                IsBuiltin = true,
                ResourceLocator = locator,
            });
        }
        public void LoadModLogics(Game game)
        {
            OnRegisterMod?.Invoke(this, game);

            foreach (var modInfo in modInfos)
            {
                modInfo.Logic.Load();
                game.AddMod(modInfo.Logic);
            }
        }
        public void PostGameInit()
        {
            foreach (var modInfo in GetAllModInfos())
            {
                modInfo.Logic.PostGameInit();
            }
        }
        public void RegisterModLogic(string spaceName, IModLogic modLogic)
        {
            var modInfo = GetModInfo(spaceName);
            if (modInfo == null)
                return;
            modInfo.Logic = modLogic;
        }
        public ModInfo GetModInfo(string nsp)
        {
            return modInfos.FirstOrDefault(m => m.Namespace == nsp);
        }
        public ModInfo[] GetAllModInfos()
        {
            return modInfos.ToArray();
        }
        public static event Action<IModManager, Game> OnRegisterMod;
        public MainManager Main => main;
        [SerializeField]
        private MainManager main;
        private List<ModInfo> modInfos = new List<ModInfo>();
    }
    public interface IModManager
    {
        void RegisterModLogic(string spacename, IModLogic logic);
    }
}
