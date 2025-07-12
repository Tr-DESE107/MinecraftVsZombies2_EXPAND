using PVZEngine;
using UnityEngine;

namespace MVZ2.Models
{
    public struct ModelViewData
    {
        public ModelViewData(NamespaceID id, Camera camera, int seed = 0)
        {
            this.id = id;
            this.camera = camera;
            this.seed = 0;
        }
        public NamespaceID id;
        public Camera camera;
        public int seed;
    }
}
