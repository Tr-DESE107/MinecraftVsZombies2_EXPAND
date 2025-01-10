using UnityEngine;
using UnityEngine.UI;

namespace MVZ2.Models
{
    public class ImageElement : MonoBehaviour
    {
        public SerializableImageElement ToSerializable()
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
        public bool ExcludedInGroup => excludedInGroup;
        [SerializeField]
        private bool excludedInGroup;
        private Image _image;
    }
    public class SerializableImageElement
    {
    }
}
