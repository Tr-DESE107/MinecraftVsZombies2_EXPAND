using System;
using System.Collections.Generic;
using System.Linq;
using PVZEngine;
using UnityEngine;

namespace MVZ2
{
    public partial class ResourceManager : MonoBehaviour
    {
        public Model GetModel(NamespaceID id)
        {
            var resource = models.FirstOrDefault(m => m.id == id);
            if (resource == null)
                return null;
            return resource.resource;
        }
        public void SetModelResources(ModelResource[] items)
        {
            models.Clear();
            models.AddRange(items);
        }
        public string EditorModelDirectory => editorModelDirectory;
        [SerializeField]
        private string editorModelDirectory;
        [SerializeField]
        private List<ModelResource> models;
    }

    [Serializable]
    public class ModelResource
    {
        public NamespaceID id;
        public Model resource;
    }
}
