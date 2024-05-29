using System.IO;
using System.Linq;
using System.Threading.Tasks;
using PVZEngine;
using UnityEngine;

namespace MVZ2
{
    public partial class ResourceManager : MonoBehaviour
    {
        #region 元数据列表
        public ModelMetaList GetModelMetaList(string nsp)
        {
            var modResource = GetModResource(nsp);
            if (modResource == null)
                return null;
            return modResource.ModelMetaList;
        }
        #endregion

        #region 元数据
        public ModelMeta GetModelMeta(NamespaceID id)
        {
            var meta = GetModelMetaList(id.spacename);
            if (meta == null)
                return null;
            return meta.metas.FirstOrDefault(m => ModelID.ConcatName(m.type, m.name) == id.path);
        }
        #endregion

        #region 模型
        public Model GetModel(string nsp, string path)
        {
            var modResource = GetModResource(nsp);
            if (modResource == null)
                return null;
            return modResource.Models.TryGetValue(path, out var res) ? res : null;
        }
        public Model GetModel(NamespaceID id)
        {
            if (id == null)
                return null;
            return GetModel(id.spacename, id.path);
        }
        #endregion

        #region 模型图标
        public Sprite GetModelIcon(string nsp, string path)
        {
            var modResource = GetModResource(nsp);
            if (modResource == null)
                return null;
            return modResource.ModelIcons.TryGetValue(path, out var res) ? res : null;
        }
        public Sprite GetModelIcon(NamespaceID id)
        {
            if (id == null)
                return null;
            return GetModelIcon(id.spacename, id.path);
        }
        #endregion

        #region 私有方法
        private async Task<ModelMetaList> LoadModelMetaList(string nsp)
        {
            var textAsset = await LoadModResource<TextAsset>(nsp, "models", ResourceType.Meta);
            using var memoryStream = new MemoryStream(textAsset.bytes);
            var document = LoadXmlDocument(memoryStream);
            return ModelMetaList.FromXmlNode(document["models"]);
        }
        private async Task LoadModModels(string nsp)
        {
            var modResource = GetModResource(nsp);
            if (modResource == null)
                return;
            var resources = await LoadLabeledResources<GameObject>(nsp, "Model");
            foreach (var (path, res) in resources)
            {
                var model = res.GetComponent<Model>();
                modResource.Models.Add(path, model);
            }
        }
        private void ShotModelIcons(string nsp, ModelMetaList metaList)
        {
            var modResource = GetModResource(nsp);
            if (modResource == null)
                return;
            foreach (var meta in metaList.metas)
            {
                var model = GetModel(meta.path);
                var metaPath = ModelID.ConcatName(meta.type, meta.name);
                var metaID = new NamespaceID(nsp, metaPath);
                var sprite = main.ModelManager.ShotIcon(model, meta.width, meta.height, new Vector2(meta.xOffset, meta.yOffset), metaID.ToString());
                modResource.ModelIcons.Add(metaPath, sprite);
            }
        }
        #endregion
    }
}
