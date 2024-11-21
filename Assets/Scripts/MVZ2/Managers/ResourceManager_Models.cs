using System;
using System.Linq;
using System.Threading.Tasks;
using MVZ2.Logic.Models;
using MVZ2.Rendering;
using MVZ2.Resources;
using PVZEngine;
using UnityEngine;

namespace MVZ2.Managers
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
        public ModelMeta[] GetModModelMetas(string nsp)
        {
            var metaList = GetModelMetaList(nsp);
            if (metaList == null)
                return Array.Empty<ModelMeta>();
            return metaList.metas.ToArray();
        }
        #endregion

        #region 元数据
        public ModelMeta GetModelMeta(NamespaceID id)
        {
            var meta = GetModelMetaList(id.spacename);
            if (meta == null)
                return null;
            return meta.metas.FirstOrDefault(m => EngineModelID.ConcatName(m.type, m.name) == id.path);
        }
        #endregion

        #region 模型
        public IModel GetModel(string nsp, string path)
        {
            return GetModel(new NamespaceID(nsp, path));
        }
        public IModel GetModel(NamespaceID id)
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
            foreach (var (id, res) in resources)
            {
                var model = res.GetComponent<Model>();
                modResource.Models.Add(id.path, model);
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
                var metaPath = EngineModelID.ConcatName(meta.type, meta.name);
                var metaID = new NamespaceID(metaNamespace, metaPath);
                if (model != null)
                {
                    var sprite = main.ModelManager.ShotIcon(model, meta.width, meta.height, new Vector2(meta.xOffset, meta.yOffset), metaID.ToString());
                    modResource.ModelIcons.Add(metaPath, sprite);
                }
                else
                {
                    modResource.ModelIcons.Add(metaPath, GetDefaultSpriteClone());
                    Debug.LogWarning($"Model {metaID} is missing.");
                }
            }
        }
        #endregion
    }
}
