using System.Xml;
using PVZEngine;

namespace MVZ2
{
    public class AudioSample
    {
        public NamespaceID path;
        public float weight;

        public static AudioSample FromXmlNode(XmlNode node)
        {
            float weight = 1;
            var weightAttribute = node.Attributes["weight"];
            if (weightAttribute != null)
            {
                if (float.TryParse(weightAttribute.Value, out var floatValue))
                {
                    weight = floatValue;
                }
            }
            return new AudioSample()
            {
                path = ResourceManager.ParseNamespaceID(node.Attributes["path"].Value),
                weight = weight,
            };
        }
    }
}
