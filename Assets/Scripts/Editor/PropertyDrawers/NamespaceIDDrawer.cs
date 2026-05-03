using MVZ2Logic;
using MVZ2Logic.Entities;
using MVZ2Logic.Definitions;
using PVZEngine.Base;
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

            // ���������ǩ����ȡʣ���������
            var propertyRect = EditorGUI.PrefixLabel(position, label);

            // ���� spacename �ľ��Σ�ռ40%���ȣ�
            var spaceRect = new Rect(propertyRect.x, propertyRect.y,
                                     propertyRect.width * 0.4f, propertyRect.height);
            // ʹ�� PropertyField �����ر�ǩ���Զ�������ѡ
            EditorGUI.PropertyField(spaceRect, spacename, GUIContent.none);

            // ���� path �ľ��Σ�ʣ����ȣ���5���ؼ�ࣩ
            var pathRect = new Rect(propertyRect.x + propertyRect.width * 0.4f + 5,
                                    propertyRect.y,
                                    propertyRect.width - propertyRect.width * 0.4f - 5,
                                    propertyRect.height);
            EditorGUI.PropertyField(pathRect, path, GUIContent.none);

            EditorGUI.EndProperty();
        }
    }
}
