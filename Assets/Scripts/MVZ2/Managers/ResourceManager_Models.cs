﻿using System;
using System.Linq;
using System.Threading.Tasks;
using MVZ2.Metas;
using MVZ2.Models;
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
            if (!NamespaceID.IsValid(id))
                return null;
            var meta = GetModelMetaList(id.SpaceName);
            if (meta == null)
                return null;
            return meta.metas.FirstOrDefault(m => EngineModelID.ConcatName(m.Type, m.Name) == id.Path);
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
        private async Task LoadModModels(string nsp, TaskProgress progress)
        {
            var modResource = GetModResource(nsp);
            if (modResource == null)
                return;
            var resources = await LoadLabeledResources<GameObject>(nsp, "Model", progress);

            foreach (var (id, res) in resources)
            {
                var model = res.GetComponent<Model>();
                modResource.Models.Add(id.Path, model);
            }
        }
        private async Task ShotModelIcons(string modNamespace, TaskProgress progress, int maxYieldCount = 4)
        {
            var modResource = GetModResource(modNamespace);
            if (modResource == null)
                return;
            var metaList = modResource.ModelMetaList;
            if (metaList == null)
                return;

            var metas = metaList.metas;
            var count = metas.Length;
            int yieldCounter = 0;
            for (int i = 0; i < count; i++)
            {
                var meta = metas[i];

                var model = GetModel(meta.Path);
                var metaPath = EngineModelID.ConcatName(meta.Type, meta.Name);
                var metaID = new NamespaceID(modNamespace, metaPath);
                if (model != null)
                {
                    Sprite sprite;
                    if (meta.Shot)
                    {
                        sprite = main.ModelManager.ShotIcon(model, meta.Width, meta.Height, new Vector2(meta.XOffset, meta.YOffset), metaID.ToString());
                    }
                    else
                    {
                        sprite = GetDefaultSpriteClone();
                    }
                    modResource.ModelIcons.Add(metaPath, sprite);
                }
                else
                {
                    modResource.ModelIcons.Add(metaPath, GetDefaultSpriteClone());
                    Debug.LogWarning($"Model prefab {metaID} is missing.");
                }
                progress.SetProgress(1 / (float)count, metaID.ToString());
                yieldCounter++;
                if (yieldCounter >= maxYieldCount)
                {
                    yieldCounter = 0;
                    await Task.Yield();
                }
            }
            progress.SetProgress(1, "Finished");
        }
        #endregion
    }
}
