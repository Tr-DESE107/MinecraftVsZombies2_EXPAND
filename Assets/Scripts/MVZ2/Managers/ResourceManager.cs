using System;
using System.Collections.Generic;
using System.Linq;
using PVZEngine;
using UnityEngine;

namespace MVZ2
{
    public class ResourceManager : MonoBehaviour
    {
        public Model GetModel(NamespaceID id)
        {
            var resource = models.FirstOrDefault(m => m.id == id);
            if (resource == null)
                return null;
            return resource.model;
        }
        public MainManager Main => main;
        [SerializeField]
        private MainManager main;
        [SerializeField]
        private List<ModelResource> models = new List<ModelResource>();
    }

    [Serializable]
    public class ModelResource
    {
        public NamespaceID id;
        public Model model;
    }
}
