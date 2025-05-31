using System.Linq;
using UnityEditor;
using UnityEngine;

namespace MVZ2.Editor
{

    [CustomPropertyDrawer(typeof(SortingLayerPicker))]
    public class SortingLayerPickerEditor : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var id = property.FindPropertyRelative("id");

            var layers = SortingLayer.layers;
            var layerNames = layers.Select(layer => layer.name).ToArray();

            EditorGUI.BeginProperty(position, label, property);
            var index = SortingLayer.GetLayerValueFromID(id.intValue) - SortingLayer.GetLayerValueFromID(layers[0].id);
            index = Mathf.Clamp(index, 0, layerNames.Length - 1);
            index = EditorGUI.Popup(position, label.text, index, layerNames);

            id.intValue = layers[index].id;
            EditorGUI.EndProperty();
        }
    }
}
