using System.Linq;
using System.Threading.Tasks;
using MVZ2.Map;
using MVZ2Logic.Map;
using MVZ2Logic.Models;
using PVZEngine;
using UnityEngine;

namespace MVZ2.Managers
{
    public partial class ResourceManager : MonoBehaviour
    {
        public NamespaceID GetFirstMapID()
        {
            foreach (var mod in modResources)
            {
                var stageMetalist = mod.MapMetaList;
                if (stageMetalist == null)
                    continue;
                var meta = stageMetalist.metas.FirstOrDefault();
                if (meta == null)
                    continue;
                return new NamespaceID(mod.Namespace, meta.id);
            }
            return null;
        }
        public MapMetaList GetMapMetaList(string spaceName)
        {
            var modResource = GetModResource(spaceName);
            if (modResource == null)
                return null;
            return modResource.MapMetaList;
        }
        public MapMeta[] GetModMapMetas(string spaceName)
        {
            var stageMetalist = GetMapMetaList(spaceName);
            if (stageMetalist == null)
                return null;
            return stageMetalist.metas.ToArray();
        }
        public MapMeta GetMapMeta(NamespaceID mapID)
        {
            if (mapID == null)
                return null;
            var stageMetalist = GetMapMetaList(mapID.spacename);
            if (stageMetalist == null)
                return null;
            return stageMetalist.metas.FirstOrDefault(m => m.id == mapID.path);
        }
        #region 模型
        public IMapModel GetMapModel(string nsp, string path)
        {
            return GetMapModel(new NamespaceID(nsp, path));
        }
        public IMapModel GetMapModel(NamespaceID id)
        {
            return FindInMods(id, mod => mod.MapModels);
        }
        #endregion


        #region 私有方法
        private async Task LoadModMapModels(string nsp)
        {
            var modResource = GetModResource(nsp);
            if (modResource == null)
                return;
            var resources = await LoadLabeledResources<GameObject>(nsp, "MapModel");
            foreach (var (id, res) in resources)
            {
                var model = res.GetComponent<MapModel>();
                modResource.MapModels.Add(id.path, model);
            }
        }
        #endregion
    }
}
