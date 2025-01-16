using System.Collections.Generic;
using System.Threading.Tasks;
using MVZ2.Sprites;
using MVZ2Logic;
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
            if (!SpriteReference.IsValid(spriteRef))
                return null;
            if (spriteRef.isSheet)
            {
                var sheet = GetSpriteSheet(spriteRef.id);
                if (sheet == null)
                    return null;
                if (spriteRef.index >= sheet.Length)
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
            if (!sprite)
                return null;
            if (spriteReferenceCacheDict.TryGetValue(sprite, out var sprRef))
            {
                return sprRef;
            }
            return null;
        }
        public Sprite GetDefaultSprite()
        {
            return defaultSprite;
        }
        public Sprite GetDefaultSpriteClone()
        {
            return Instantiate(defaultSprite);
        }
        public Sprite CreateSprite(Texture2D texture, Rect rect, Vector2 pivot, string name, string category = "default")
        {
            var sprite = Sprite.Create(texture, rect, pivot);
            sprite.name = name;
            if (generatedSpriteManifest && Application.isEditor)
            {
                var textureClone = GetGeneratedSpriteTexture(texture);
                var spriteClone = Sprite.Create(textureClone, rect, pivot);
                spriteClone.name = name;
                generatedSpriteManifest.AddSprite(category, spriteClone);
            }
            return sprite;
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
                modResource.SpriteSheets.Add(id.Path, res);
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
                modResource.Sprites.Add(id.Path, res);
                var sprRef = new SpriteReference(id);
                AddSpriteReferenceCache(sprRef, res);
            }
        }
        private void AddSpriteReferenceCache(SpriteReference sprRef, Sprite sprite)
        {
            spriteReferenceCacheDict.Add(sprite, sprRef);
        }
        private Texture2D GetGeneratedSpriteTexture(Texture2D texture)
        {
            if (!generatedSpriteTextureDict.TryGetValue(texture, out var tex))
            {
                tex = Instantiate(texture);
                var width = tex.width;
                var pixels = tex.GetPixels();
                for (int i = 0; i < pixels.Length; i++)
                {
                    var x = i % width;
                    var y = i / width;
                    var col = ((x / 16) + (y / 16)) % 2 == 0 ? Color.gray : new Color(0.25f, 0.25f, 0.25f, 1);
                    var pixel = pixels[i];
                    pixel.r = pixel.r * pixel.a + col.r * (1 - pixel.a);
                    pixel.g = pixel.g * pixel.a + col.g * (1 - pixel.a);
                    pixel.b = pixel.b * pixel.a + col.b * (1 - pixel.a);
                    pixel.a = pixel.a * pixel.a + col.a * (1 - pixel.a);
                    pixels[i] = pixel;
                }
                tex.SetPixels(pixels);
                tex.Apply();
                generatedSpriteTextureDict.Add(texture, tex);
            }
            return tex;
        }
        private Dictionary<Sprite, SpriteReference> spriteReferenceCacheDict = new Dictionary<Sprite, SpriteReference>();
        private Dictionary<Texture2D, Texture2D> generatedSpriteTextureDict = new Dictionary<Texture2D, Texture2D>();
        [Header("Sprites")]
        [SerializeField]
        private Sprite defaultSprite;
        [SerializeField]
        private GeneratedSpriteManifest generatedSpriteManifest;
    }
}
