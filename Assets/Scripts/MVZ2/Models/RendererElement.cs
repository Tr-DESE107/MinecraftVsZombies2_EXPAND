using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MVZ2.Rendering
{
    public class RendererElement : MonoBehaviour
    {
        public void SetInt(string name, int value)
        {
            PropertyBlock.Clear();
            Renderer.GetPropertyBlock(PropertyBlock);
            PropertyBlock.SetInt(name, value);
            Renderer.SetPropertyBlock(PropertyBlock);
            intProperties[name] = value;
        }
        public void SetFloat(string name, float value)
        {
            PropertyBlock.Clear();
            Renderer.GetPropertyBlock(PropertyBlock);
            PropertyBlock.SetFloat(name, value);
            Renderer.SetPropertyBlock(PropertyBlock);
            floatProperties[name] = value;
        }

        public void SetColor(string name, Color value)
        {
            PropertyBlock.Clear();
            Renderer.GetPropertyBlock(PropertyBlock);
            PropertyBlock.SetColor(name, value);
            Renderer.SetPropertyBlock(PropertyBlock);
            colorProperties[name] = value;
        }

        public SerializableRendererElement ToSerializable()
        {
            return new SerializableRendererElement()
            {
                colorProperties = colorProperties.ToDictionary(p => p.Key, p => p.Value),
                intProperties = intProperties.ToDictionary(p => p.Key, p => p.Value),
                floatProperties = floatProperties.ToDictionary(p => p.Key, p => p.Value),
            };
        }
        public void LoadFromSerializable(SerializableRendererElement serializable)
        {
            intProperties.Clear();
            floatProperties.Clear();
            colorProperties.Clear();

            PropertyBlock.Clear();
            Renderer.GetPropertyBlock(PropertyBlock);
            foreach (var prop in serializable.intProperties)
            {
                PropertyBlock.SetInt(prop.Key, prop.Value);
                intProperties[prop.Key] = prop.Value;
            }
            foreach (var prop in serializable.floatProperties)
            {
                PropertyBlock.SetFloat(prop.Key, prop.Value);
                floatProperties[prop.Key] = prop.Value;
            }
            foreach (var prop in serializable.colorProperties)
            {
                PropertyBlock.SetColor(prop.Key, prop.Value);
                colorProperties[prop.Key] = prop.Value;
            }
            Renderer.SetPropertyBlock(PropertyBlock);
        }
        public MaterialPropertyBlock PropertyBlock
        {
            get
            {
                if (propertyBlock == null)
                {
                    propertyBlock = new MaterialPropertyBlock();
                }
                return propertyBlock;
            }
        }
        public Renderer Renderer
        {
            get
            {
                if (!_renderer)
                {
                    _renderer = GetComponent<Renderer>();
                }
                return _renderer;
            }
        }
        public bool ExcludedInGroup => excludedInGroup;
        [SerializeField]
        private bool excludedInGroup;
        private MaterialPropertyBlock propertyBlock;
        private Dictionary<string, float> floatProperties = new Dictionary<string, float>();
        private Dictionary<string, int> intProperties = new Dictionary<string, int>();
        private Dictionary<string, Color> colorProperties = new Dictionary<string, Color>();
        private Renderer _renderer;
    }
    public class SerializableRendererElement
    {
        public Dictionary<string, float> floatProperties;
        public Dictionary<string, int> intProperties;
        public Dictionary<string, Color> colorProperties;
    }
}
