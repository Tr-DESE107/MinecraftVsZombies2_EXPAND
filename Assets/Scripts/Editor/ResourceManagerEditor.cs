using System.IO;
using System.Linq;
using PVZEngine;
using UnityEditor;
using UnityEngine;

namespace MVZ2.Editor
{
    [CustomEditor(typeof(ResourceManager))]
    public class ResourceManagerEditor : UnityEditor.Editor
    {
        private ResourceManager resourceManager;
        private void OnEnable()
        {
            resourceManager = target as ResourceManager;
        }
    }
}
