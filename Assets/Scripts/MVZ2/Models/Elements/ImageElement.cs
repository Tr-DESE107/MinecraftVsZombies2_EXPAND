﻿using UnityEngine.UI;

namespace MVZ2.Models
{
    public class ImageElement : GraphicElement
    {
        public override SerializableGraphicElement ToSerializable()
        {
            return new SerializableImageElement();
        }
        public void LoadFromSerializable(SerializableImageElement serializable)
        {
        }
        public Image Image
        {
            get
            {
                if (!_image)
                {
                    _image = GetComponent<Image>();
                }
                return _image;
            }
        }
        private Image _image;
    }
    public class SerializableImageElement : SerializableGraphicElement
    {
    }
}
