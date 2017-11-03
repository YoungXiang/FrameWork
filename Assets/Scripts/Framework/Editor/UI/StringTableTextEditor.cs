using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor.UI;

[CustomEditor(typeof(FrameWork.StringTableText), true)]
[CanEditMultipleObjects]
public class StringTableTextEditor : GraphicEditor
{
    SerializedProperty m_StringTableID;
    SerializedProperty m_Text;
    SerializedProperty m_FontData;

    protected override void OnEnable()
    {
        base.OnEnable();
        m_StringTableID = serializedObject.FindProperty("m_StringTableID");
        m_Text = serializedObject.FindProperty("m_Text");
        m_FontData = serializedObject.FindProperty("m_FontData");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(m_Text);
        EditorGUILayout.PropertyField(m_StringTableID);
        EditorGUILayout.PropertyField(m_FontData);
        AppearanceControlsGUI();
        RaycastControlsGUI();
        serializedObject.ApplyModifiedProperties();
    }
}
