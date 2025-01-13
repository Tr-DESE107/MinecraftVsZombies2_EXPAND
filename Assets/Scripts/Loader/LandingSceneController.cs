using System.IO;
using MVZ2.Games;
using MVZ2.Modding;
using MVZ2.Vanilla;
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
            ModManager.OnRegisterMod += RegisterMod;
            Addressables.LoadSceneAsync("Main", LoadSceneMode.Single);
        }
        private static void RegisterMod(IModManager manager, Game game)
        {
            manager.RegisterModLogic(VanillaMod.spaceName, new VanillaMod());
        }
        [SerializeField]
        private AndroidLogger logger;
    }
}
