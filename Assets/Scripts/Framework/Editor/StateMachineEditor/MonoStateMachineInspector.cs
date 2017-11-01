using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(FrameWork.MonoStateMachine), true)]
public class MonoStateMachineInspector : Editor
{
    GUIStyle textStyle = new GUIStyle();
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        FrameWork.MonoStateMachine inst = target as FrameWork.MonoStateMachine;

        textStyle.normal.textColor = Color.green;
        EditorGUILayout.LabelField("Current State", inst.currentStateName, textStyle);
    }
}
