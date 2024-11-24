using System.Xml;
using MVZ2Logic;
using PVZEngine;

namespace MVZ2.Metas
{
    public class AudioSample
    {
        public NamespaceID path;
        public float weight;

        public static AudioSample FromXmlNode(XmlNode node, string defaultNsp)
        {
            float weight = 1;
            var weightAttribute = node.Attributes["weight"];
            if (weightAttribute != null)
            {
                if (ParseHelper.TryParseFloat(weightAttribute.Value, out var floatValue))
                {
                    weight = floatValue;
                }
            }
            return new AudioSample()
            {
                path = NamespaceID.Parse(node.Attributes["path"].Value, defaultNsp),
                weight = weight,
            };
        }
    }
}
