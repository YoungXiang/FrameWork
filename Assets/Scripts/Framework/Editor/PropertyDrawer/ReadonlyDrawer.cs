using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomPropertyDrawer(typeof(ReadonlyAttribute))]
public class ReadonlyDrawer : PropertyDrawer
{
    // Draw the property inside the given rect
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // First get the attribute since it contains the range for the slider
        ReadonlyAttribute readonlyAttr = attribute as ReadonlyAttribute;

        Rect halfRect = new Rect(position.x, position.y, position.width * 0.5f, position.height);
        EditorGUI.LabelField(halfRect, property.name);
        halfRect.x += position.width * 0.5f;
        switch (property.propertyType)
        {
            case SerializedPropertyType.Boolean:
                EditorGUI.LabelField(halfRect, property.boolValue.ToString());
                break;
            case SerializedPropertyType.Integer:
                EditorGUI.LabelField(halfRect, property.intValue.ToString());
                break;
            case SerializedPropertyType.Float:
                EditorGUI.LabelField(halfRect, property.floatValue.ToString());
                break;
            case SerializedPropertyType.Enum:
                EditorGUI.LabelField(halfRect, property.enumDisplayNames[property.enumValueIndex]);
                break;
            case SerializedPropertyType.Color:
                EditorGUI.LabelField(halfRect, property.colorValue.ToString());
                break;
            default:
                EditorGUI.LabelField(halfRect, "Unknown Type");
                break;
        }
    }
}
