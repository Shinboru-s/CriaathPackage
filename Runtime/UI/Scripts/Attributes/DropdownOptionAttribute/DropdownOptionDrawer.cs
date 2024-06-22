using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

[CustomPropertyDrawer(typeof(DropdownOptionAttribute))]
public class DropdownOptionDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        DropdownOptionAttribute dropdownAttribute = (DropdownOptionAttribute)attribute;
        SerializedObject targetObject = property.serializedObject;

        string[] options = GetOptions(targetObject.targetObject, dropdownAttribute.OptionsFieldName);

        if (property.propertyType == SerializedPropertyType.String)
        {
            int index = Mathf.Max(0, System.Array.IndexOf(options, property.stringValue));
            index = EditorGUI.Popup(position, label.text, index, options);
            property.stringValue = options[index];
        }
        else if (property.propertyType == SerializedPropertyType.Generic)
        {
            if (property.isArray)
            {
                EditorGUI.BeginProperty(position, label, property);
                SerializedProperty arraySizeProp = property.FindPropertyRelative("Array.size");
                arraySizeProp.intValue = EditorGUI.IntField(position, "Size", arraySizeProp.intValue);

                for (int i = 0; i < property.arraySize; i++)
                {
                    SerializedProperty element = property.GetArrayElementAtIndex(i);
                    position.y += EditorGUIUtility.singleLineHeight;

                    int index = Mathf.Max(0, System.Array.IndexOf(options, element.stringValue));
                    index = EditorGUI.Popup(position, $"Element {i}", index, options);

                    element.stringValue = options[index];
                }
                EditorGUI.EndProperty();
            }
        }
        else
        {
            EditorGUI.PropertyField(position, property, label);
        }
    }

    private string[] GetOptions(object target, string fieldName)
    {
        FieldInfo field = target.GetType().GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        if (field != null)
        {
            if (field.FieldType == typeof(string[]))
            {
                return (string[])field.GetValue(target);
            }
            else if (field.FieldType == typeof(List<string>))
            {
                return ((List<string>)field.GetValue(target)).ToArray();
            }
        }
        return new string[] { "None" };
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (property.propertyType == SerializedPropertyType.Generic && property.isArray)
        {
            return EditorGUIUtility.singleLineHeight * (property.arraySize + 1);
        }
        return EditorGUIUtility.singleLineHeight;
    }
}
