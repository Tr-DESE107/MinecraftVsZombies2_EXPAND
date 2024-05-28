using System.Collections.Generic;
using System.Threading.Tasks;
using PVZEngine;
using UnityEngine;
using UnityEngine.AddressableAssets.ResourceLocators;

namespace MVZ2
{
    public partial class ResourceManager : MonoBehaviour
    {
        public Sprite GetSprite(NamespaceID id)
        {
            var modResource = GetModResource(id.spacename);
            if (modResource == null)
                return null;
            if (!modResource.Sprites.TryGetValue(id.name, out var sprite))
                return null;
            return sprite;
        }
        public Sprite[] GetSpriteSheet(NamespaceID id)
        {
            var modResource = GetModResource(id.spacename);
            if (modResource == null)
                return null;
            if (!modResource.SpriteSheets.TryGetValue(id.name, out var spritesheet))
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
