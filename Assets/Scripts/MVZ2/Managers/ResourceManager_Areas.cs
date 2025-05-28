﻿using System.Linq;
using System.Threading.Tasks;
using MVZ2.Level;
using MVZ2.Metas;
using PVZEngine;
using UnityEngine;

namespace MVZ2.Managers
{
    public partial class ResourceManager : MonoBehaviour
    {
        public AreaMetaList GetAreaMetaList(string spaceName)
        {
            var modResource = GetModResource(spaceName);
            if (modResource == null)
                return null;
            return modResource.AreaMetaList;
        }
        public AreaMeta[] GetModAreaMetas(string spaceName)
        {
            var stageMetalist = GetAreaMetaList(spaceName);
            if (stageMetalist == null)
                return null;
            return stageMetalist.metas.ToArray();
        }
        public AreaMeta GetAreaMeta(NamespaceID mapID)
        {
            if (mapID == null)
                return null;
            var stageMetalist = GetAreaMetaList(mapID.SpaceName);
            if (stageMetalist == null)
                return null;
            return stageMetalist.metas.FirstOrDefault(m => m.ID == mapID.Path);
        }
        #region 模型
        public AreaModel GetAreaModel(string nsp, string path)
        {
            return GetAreaModel(new NamespaceID(nsp, path));
        }
        public AreaModel GetAreaModel(NamespaceID id)
        {
            return FindInMods(id, mod => mod.AreaModels);
        }
        #endregion


        #region 私有方法
        private async Task LoadModAreaModels(string nsp, TaskProgress progress)
        {
            var modResource = GetModResource(nsp);
            if (modResource == null)
                return;
            var resources = await LoadLabeledResources<GameObject>(nsp, "AreaModel", progress);
            foreach (var (id, res) in resources)
            {
                var model = res.GetComponent<AreaModel>();
                modResource.AreaModels.Add(id.Path, model);
            }
        }
        #endregion
    }
}
