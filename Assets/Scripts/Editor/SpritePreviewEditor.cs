using MVZ2.Sprites;
using UnityEditor;
using UnityEngine;

namespace MVZ2.Editor
{
    [CustomPropertyDrawer(typeof(SpritePreview))]
    public class SpritePreviewEditor : PropertyDrawer
    {
        public const float MAX_WIDTH = 200;
        public const float MAX_HEIGHT = 200;
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var height = base.GetPropertyHeight(property, label);
            var spriteProperty = property.FindPropertyRelative("sprite");
            var sprite = spriteProperty.objectReferenceValue as Sprite;
            if (sprite != null)
            {
                var preview = AssetPreview.GetAssetPreview(sprite);
                var size = CalculateSize(preview);
                height += size.y;
            }
            return height;
        }
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var spriteProperty = property.FindPropertyRelative("sprite");
            var sprite = spriteProperty.objectReferenceValue as Sprite;
            if (sprite == null)
                return;
            var namePos = position;
            namePos.height = 18;
            GUI.Label(namePos, label);

            var preview = AssetPreview.GetAssetPreview(sprite);
            var size = CalculateSize(preview);
            var previewPos = new Rect(position.x, position.y + 18f, size.x, size.y);
            GUI.Label(previewPos, preview);
        }
        private Vector2 CalculateSize(Texture2D texture)
        {
            if (texture.width > texture.height)
            {
                var width = Mathf.Min(MAX_WIDTH, texture.width);
                return new Vector2(width, width * texture.height / texture.width);
            }
            else
            {
                var height = Mathf.Min(MAX_HEIGHT, texture.height);
                return new Vector2(height * texture.width / texture.height, height);
            }
        }
    }
}
