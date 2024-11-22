using System.Xml;
using PVZEngine;

namespace MVZ2Logic.Models
{
    public class ModelMeta
    {
        public string name;
        public string type;
        public NamespaceID path;
        public int width;
        public int height;
        public float xOffset;
        public float yOffset;
        public override string ToString()
        {
            return name;
        }
    }
}
