using System.Collections.Generic;
using System.Threading.Tasks;
using MVZ2.Resources;
using PVZEngine;
using UnityEngine;

namespace MVZ2.Managers
{
    public partial class ResourceManager : MonoBehaviour
    {
        public Sprite GetSprite(string nsp, string path)
        {
            return GetSprite(new NamespaceID(nsp, path));
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
        private async Task LoadSpriteManifests(string modNamespace)
        {
            var modResource = GetModResource(modNamespace);
            if (modResource == null)
                return;
            var resources = await LoadLabeledResources<SpriteManifest>(modNamespace, "SpriteManifest");
            foreach (var (path, manifest) in resources)
            {
                foreach (var entry in manifest.spriteEntries)
                {
                    var sprite = entry.sprite;
                    var id = new NamespaceID(modNamespace, entry.name);
                    modResource.Sprites.Add(entry.name, sprite);
                    AddSpriteReferenceCache(new SpriteReference(id), sprite);
                }
                foreach (var entry in manifest.spritesheetEntries)
                {
                    var sheet = entry.spritesheet;
                    var id = new NamespaceID(modNamespace, entry.name);
                    modResource.SpriteSheets.Add(entry.name, sheet);
                    for (int i = 0; i < sheet.Length; i++)
                    {
                        AddSpriteReferenceCache(new SpriteReference(id, i), sheet[i]);
                    }
                }
            }
        }
        private async Task LoadSpriteSheets(string modNamespace)
        {
            var modResource = GetModResource(modNamespace);
            if (modResource == null)
                return;
            var resources = await LoadLabeledResources<Sprite[]>(modNamespace, "Spritesheet");
            foreach (var (id, res) in resources)
            {
                modResource.SpriteSheets.Add(id.path, res);
                for (int i = 0; i < res.Length; i++)
                {
                    var sprRef = new SpriteReference(id, i);
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
            foreach (var (id, res) in resources)
            {
                modResource.Sprites.Add(id.path, res);
                var sprRef = new SpriteReference(id);
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
