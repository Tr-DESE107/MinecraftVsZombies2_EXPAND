using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets.ResourceLocators;

namespace MVZ2
{
    public partial class ResourceManager : MonoBehaviour
    {
        public Sprite GetSprite(string nsp, string path)
        {
            var modResource = GetModResource(nsp);
            if (modResource == null)
                return null;
            if (!modResource.Sprites.TryGetValue(path, out var sprite))
                return null;
            return sprite;
        }
        public Sprite[] GetSpriteSheet(string nsp, string path)
        {
            var modResource = GetModResource(nsp);
            if (modResource == null)
                return null;
            if (!modResource.SpriteSheets.TryGetValue(path, out var spritesheet))
                return null;
            return spritesheet;
        }
        private Task<Dictionary<string, Sprite[]>> LoadSpriteSheets(string nsp, IResourceLocator locator)
        {
            return LoadLabeledResources<Sprite[]>(nsp, locator, "Spritesheet");
        }
        private Task<Dictionary<string, Sprite>> LoadSprites(string nsp, IResourceLocator locator)
        {
            return LoadLabeledResources<Sprite>(nsp, locator, "Sprite");
        }
    }
}
