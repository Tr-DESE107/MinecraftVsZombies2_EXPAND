using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MVZ2.Models
{
    public class RendererElement : GraphicElement
    {
        public void SetInt(string name, int value)
        {
            var propertyBlock = PropertyBlock;
            var renderer = Renderer;
            propertyBlock.Clear();
            renderer.GetPropertyBlock(propertyBlock);
            propertyBlock.SetInt(name, value);
            renderer.SetPropertyBlock(propertyBlock);
            intProperties[name] = value;
        }
        public void SetFloat(string name, float value)
        {
            var propertyBlock = PropertyBlock;
            var renderer = Renderer;
            propertyBlock.Clear();
            renderer.GetPropertyBlock(propertyBlock);
            propertyBlock.SetFloat(name, value);
            renderer.SetPropertyBlock(propertyBlock);
            floatProperties[name] = value;
        }

        public void SetColor(string name, Color value)
        {
            var propertyBlock = PropertyBlock;
            var renderer = Renderer;
            propertyBlock.Clear();
            renderer.GetPropertyBlock(propertyBlock);
            propertyBlock.SetColor(name, value);
            renderer.SetPropertyBlock(propertyBlock);
            colorProperties[name] = value;
        }

        public override SerializableGraphicElement ToSerializable()
        {
            return new SerializableRendererElement()
            {
                colorProperties = colorProperties.ToDictionary(p => p.Key, p => p.Value),
                intProperties = intProperties.ToDictionary(p => p.Key, p => p.Value),
                floatProperties = floatProperties.ToDictionary(p => p.Key, p => p.Value),
            };
        }
        public override void LoadFromSerializable(SerializableGraphicElement serializable)
        {
            if (serializable is not SerializableRendererElement rendererElement)
                return;
            intProperties.Clear();
            floatProperties.Clear();
            colorProperties.Clear();

            var propertyBlock = PropertyBlock;
            var renderer = Renderer;
            propertyBlock.Clear();
            renderer.GetPropertyBlock(propertyBlock);
            foreach (var prop in rendererElement.intProperties)
            {
                propertyBlock.SetInt(prop.Key, prop.Value);
                intProperties[prop.Key] = prop.Value;
            }
            foreach (var prop in rendererElement.floatProperties)
            {
                propertyBlock.SetFloat(prop.Key, prop.Value);
                floatProperties[prop.Key] = prop.Value;
            }
            foreach (var prop in rendererElement.colorProperties)
            {
                propertyBlock.SetColor(prop.Key, prop.Value);
                colorProperties[prop.Key] = prop.Value;
            }
            renderer.SetPropertyBlock(propertyBlock);
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
        private MaterialPropertyBlock propertyBlock;
        private Dictionary<string, float> floatProperties = new Dictionary<string, float>();
        private Dictionary<string, int> intProperties = new Dictionary<string, int>();
        private Dictionary<string, Color> colorProperties = new Dictionary<string, Color>();
        private Renderer _renderer;
    }
    public class SerializableRendererElement : SerializableGraphicElement
    {
        public Dictionary<string, float> floatProperties;
        public Dictionary<string, int> intProperties;
        public Dictionary<string, Color> colorProperties;
    }
}
