using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
[CustomPropertyDrawer(typeof(ShowIfAttribute))]
public class ShowIfDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        ShowIfAttribute showIf = (ShowIfAttribute)attribute;
        SerializedProperty conditionProperty = property.serializedObject.FindProperty(showIf.conditionField);

        if (conditionProperty != null && conditionProperty.enumValueIndex == (int)showIf.conditionValue)
        {
            EditorGUI.PropertyField(position, property, label);
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        ShowIfAttribute showIf = (ShowIfAttribute)attribute;
        SerializedProperty conditionProperty = property.serializedObject.FindProperty(showIf.conditionField);

        if (conditionProperty != null && conditionProperty.enumValueIndex == (int)showIf.conditionValue)
        {
            return EditorGUI.GetPropertyHeight(property);
        }
        return 0; // Hide property
    }
}
