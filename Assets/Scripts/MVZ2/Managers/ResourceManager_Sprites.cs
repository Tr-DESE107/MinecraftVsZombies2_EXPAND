using System.Collections.Generic;
using System.Threading.Tasks;
using PVZEngine;
using UnityEngine;

namespace MVZ2
{
    public partial class ResourceManager : MonoBehaviour
    {
        public Sprite GetSprite(string nsp, string path)
        {
            return GetSprite(nsp, path);
        }
        public Sprite GetSprite(NamespaceID id)
        {
            return FindInMods(id, mod => mod.Sprites);
        }
        public Sprite GetSprite(SpriteReference spriteRef)
        {
            if (spriteRef == null)
                return null;
            if (spriteRef.isSheet)
            {
                var sheet = GetSpriteSheet(spriteRef.id);
                if (sheet == null)
                    return null;
                return sheet[spriteRef.index];
            }
            else
            {
                return GetSprite(spriteRef.id);
            }
        }
        public Sprite[] GetSpriteSheet(string nsp, string path)
        {
            return GetSpriteSheet(nsp, path);
        }
        public Sprite[] GetSpriteSheet(NamespaceID id)
        {
            return FindInMods(id, mod => mod.SpriteSheets);
        }
        public SpriteReference GetSpriteReference(Sprite sprite)
        {
            if (spriteReferenceCacheDict.TryGetValue(sprite, out var sprRef))
            {
                return sprRef;
            }
            return null;
        }
        private async Task LoadSpriteSheets(string modNamespace)
        {
            var modResource = GetModResource(modNamespace);
            if (modResource == null)
                return;
            var resources = await LoadLabeledResources<Sprite[]>(modNamespace, "Spritesheet");
            foreach (var (path, res) in resources)
            {
                modResource.SpriteSheets.Add(path, res);
                for (int i = 0; i < res.Length; i++)
                {
                    var sprRef = new SpriteReference(path, i);
                    AddSpriteReferenceCache(sprRef, res[i]);
                }
            }
        }
        private async Task LoadSprites(string modNamespace)
        {
            var modResource = GetModResource(modNamespace);
            if (modResource == null)
                return;
            var resources = await LoadLabeledResources<Sprite>(modNamespace, "Sprite");
            foreach (var (path, res) in resources)
            {
                modResource.Sprites.Add(path, res);
                var sprRef = new SpriteReference(path);
                AddSpriteReferenceCache(sprRef, res);
            }
        }
        private void AddSpriteReferenceCache(SpriteReference sprRef, Sprite sprite)
        {
            spriteReferenceCacheDict.Add(sprite, sprRef);
        }
        private Dictionary<Sprite, SpriteReference> spriteReferenceCacheDict = new Dictionary<Sprite, SpriteReference>();
    }
}
