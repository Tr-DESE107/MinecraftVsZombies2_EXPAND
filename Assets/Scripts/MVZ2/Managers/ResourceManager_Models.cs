using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using PVZEngine;
using UnityEngine;
using UnityEngine.AddressableAssets.ResourceLocators;

namespace MVZ2
{
    public partial class ResourceManager : MonoBehaviour
    {
        #region 元数据
        public ModelsMeta GetModelsMeta(string nsp)
        {
            var modResource = GetModResource(nsp);
            if (modResource == null)
                return null;
            return modResource.ModelMeta;
        }
        private async Task<ModelsMeta> LoadModelMeta(string nsp, IResourceLocator locator)
        {
            var textAsset = await LoadAddressableResource<TextAsset>(locator, "models");
            using var memoryStream = new MemoryStream(textAsset.bytes);
            var document = LoadXmlDocument(memoryStream);
            return ModelsMeta.FromXmlNode(nsp, document["models"]);
        }
        #endregion

        #region 模型资源
        public ModelResource GetModelResource(NamespaceID id, string type)
        {
            var meta = GetModelsMeta(id.spacename);
            if (meta == null)
                return null;
            return meta.resources.FirstOrDefault(m => m.id == id && m.type == type);
        }
        #endregion

        #region 模型
        public Model GetModel(NamespaceID id)
        {
            var meta = GetModelsMeta(id.spacename);
            if (meta == null)
                return null;
            var resource = meta.resources.FirstOrDefault(m => m.id == id);
            if (resource == null)
                return null;
            return GetModel(id.spacename, CombinePath(meta.root, resource.path));
        }
        public Model GetModel(string nsp, string path)
        {
            var modResource = GetModResource(nsp);
            if (modResource == null)
                return null;
            if (modResource.Models.TryGetValue(path, out var model))
                return model;
            return null;
        }
        private async Task<Dictionary<string, Model>> LoadModels(string nsp, IResourceLocator locator, ModelsMeta meta)
        {
            var paths = meta.resources.Select(r => r.path).Distinct();
            var objects = await LoadResourceGroup<GameObject>(nsp, locator, meta.root, paths.ToArray());
            return objects.ToDictionary(p => p.Key, p => p.Value.GetComponent<Model>());
        }
        #endregion

        #region 模型图标
        public Sprite GetModelIcon(NamespaceID id)
        {
            var modResource = GetModResource(id.spacename);
            if (modResource == null)
                return null;
            if (modResource.ModelIcons.TryGetValue(id.name, out var model))
                return model;
            return null;
        }
        private Dictionary<string, Sprite> ShotModelIcons(string nsp, ModelsMeta meta, Dictionary<string, Model> models)
        {
            var dict = new Dictionary<string, Sprite>();
            foreach (var resource in meta.resources)
            {
                var path = Path.Combine(meta.root, resource.path).Replace('\\', '/');
                var model = models[path];
                var sprite = main.ModelManager.ShotIcon(model, resource.width, resource.height, new Vector2(resource.xOffset, resource.yOffset), resource.id.ToString());
                dict.Add(resource.id.name, sprite);
            }
            return dict;
        }
        #endregion
    }
}
