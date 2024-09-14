using System.Linq;
using System.Threading.Tasks;
using PVZEngine;
using PVZEngine.LevelManagement;
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
            return GetModel(new NamespaceID(nsp, path));
        }
        public Model GetModel(NamespaceID id)
        {
            return FindInMods(id, mod => mod.Models);
        }
        #endregion

        #region 模型图标
        public Sprite GetModelIcon(string nsp, string path)
        {
            return GetModelIcon(new NamespaceID(nsp, path));
        }
        public Sprite GetModelIcon(NamespaceID id)
        {
            return FindInMods(id, mod => mod.ModelIcons);
        }
        #endregion

        #region 私有方法
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
        private void ShotModelIcons(string modNamespace, string metaNamespace, ModelMetaList metaList)
        {
            var modResource = GetModResource(modNamespace);
            if (modResource == null)
                return;
            foreach (var meta in metaList.metas)
            {
                var model = GetModel(meta.path);
                var metaPath = ModelID.ConcatName(meta.type, meta.name);
                var metaID = new NamespaceID(metaNamespace, metaPath);
                var sprite = main.ModelManager.ShotIcon(model, meta.width, meta.height, new Vector2(meta.xOffset, meta.yOffset), metaID.ToString());
                modResource.ModelIcons.Add(metaID, sprite);
            }
        }
        #endregion
    }
}
