using System.Linq;
using System.Threading.Tasks;
using PVZEngine;
using UnityEngine;

namespace MVZ2
{
    public partial class ResourceManager : MonoBehaviour
    {
        public Sprite GetSprite(string nsp, string path)
        {
            var modResource = GetModResource(nsp);
            if (modResource == null)
                return null;
            return modResource.Sprites.TryGetValue(path, out var res) ? res : null;
        }
        public Sprite GetSprite(NamespaceID id)
        {
            if (id == null)
                return null;
            return GetSprite(id.spacename, id.path);
        }
        public Sprite[] GetSpriteSheet(string nsp, string path)
        {
            var modResource = GetModResource(nsp);
            if (modResource == null)
                return null;
            return modResource.SpriteSheets.TryGetValue(path, out var res) ? res : null;
        }
        public Sprite[] GetSpriteSheet(NamespaceID id)
        {
            if (id == null)
                return null;
            return GetSpriteSheet(id.spacename, id.path);
        }
        private async Task LoadSpriteSheets(string nsp)
        {
            var modResource = GetModResource(nsp);
            if (modResource == null)
                return;
            var resources = await LoadLabeledResources<Sprite[]>(nsp, "Spritesheet");
            foreach (var (path, res) in resources)
            {
                modResource.SpriteSheets.Add(path, res);
            }
        }
        private async Task LoadSprites(string nsp)
        {
            var modResource = GetModResource(nsp);
            if (modResource == null)
                return;
            var resources = await LoadLabeledResources<Sprite>(nsp, "Sprite");
            foreach (var (path, res) in resources)
            {
                modResource.Sprites.Add(path, res);
            }
        }
    }
}
