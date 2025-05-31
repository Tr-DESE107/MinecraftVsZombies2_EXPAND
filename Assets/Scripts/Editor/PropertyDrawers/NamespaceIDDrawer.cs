using UnityEditor;
using UnityEngine;

namespace MVZ2.Editor
{
    [CustomPropertyDrawer(typeof(NamespaceIDReference))]
    public class NamespaceIDDrawer : PropertyDrawer
    {

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var spacename = property.FindPropertyRelative("spacename");
            var path = property.FindPropertyRelative("path");

            EditorGUI.BeginProperty(position, label, property);

            var propertyRect = EditorGUI.PrefixLabel(position, label);

            var spacePosition = propertyRect.position;
            var spaceSize = propertyRect.size;
            spaceSize.x *= 0.4f;
            var spaceRect = new Rect(spacePosition, spaceSize);
            spacename.stringValue = EditorGUI.TextField(spaceRect, spacename.stringValue);

            var pathPosition = propertyRect.position;
            pathPosition.x += propertyRect.size.x * 0.4f + 5;
            var pathSize = propertyRect.size;
            pathSize.x = propertyRect.max.x - pathPosition.x;

            var pathRect = new Rect(pathPosition, pathSize);
            path.stringValue = EditorGUI.TextField(pathRect, path.stringValue);
            EditorGUI.EndProperty();
        }
    }
}
