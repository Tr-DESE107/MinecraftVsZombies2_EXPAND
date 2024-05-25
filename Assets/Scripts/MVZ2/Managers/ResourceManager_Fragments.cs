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
        public FragmentsMeta GetFragmentsMeta(string nsp)
        {
            var modResource = GetModResource(nsp);
            if (modResource == null)
                return null;
            return modResource.FragmentsMeta;
        }
        private async Task<FragmentsMeta> LoadFragmentsMeta(IResourceLocator locator)
        {
            var textAsset = await LoadAddressableResource<TextAsset>(locator, "fragments");
            using var memoryStream = new MemoryStream(textAsset.bytes);
            var document = LoadXmlDocument(memoryStream);
            return FragmentsMeta.FromXmlNode(document["fragments"]);
        }
        #endregion

        #region 碎片渐变
        public Gradient GetFragmentGradient(NamespaceID id)
        {
            var meta = GetFragmentsMeta(id.spacename);
            if (meta == null)
                return null;
            return meta.resources.FirstOrDefault(m => m.name == id.name)?.gradient;
        }
        #endregion
    }
}
