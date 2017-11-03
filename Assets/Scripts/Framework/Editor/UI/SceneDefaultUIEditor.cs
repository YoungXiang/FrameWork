using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

/*
[CustomEditor(typeof(SceneDefaultUI), true)]
[CanEditMultipleObjects]
public class SceneDefaultUIEditor : Editor
{
    int GetArrayValue(int[] array, int begin, int length)
    {
        int val = 0;
        for (int i = begin; i < begin + length; i++)
        {
            val += array[i];
        }
        return val;
    }
    
    int uiToLoadCount;
    //bool foldout = false;
    List<int> indexToDelete = new List<int>();
    ReorderableList list;
    private void OnEnable()
    {
        indexToDelete.Clear();
        list = new ReorderableList(serializedObject, serializedObject.FindProperty("uiToLoad"), true, true, false, false);
        uiToLoadCount = list.count;

        list.drawHeaderCallback = (Rect rect) => 
        {
            EditorGUI.LabelField(rect, "UI Load List");
        };

        int[] eleSegment = new int[] { 6, 8, 4, 3, 2, 2 };
        int totalSeg = 0;
        for (int i = 0; i < eleSegment.Length; i++) { totalSeg += eleSegment[i]; }
        list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => 
        {
            var element = list.serializedProperty.GetArrayElementAtIndex(index);
            if (element == null) return;

            //rect.y += 10;
            float width = rect.width;
            float segmentWidth = width / totalSeg;
            
            EditorGUI.LabelField(new Rect(rect.x, rect.y, segmentWidth * eleSegment[0], EditorGUIUtility.singleLineHeight), "UIStateMachine");
            EditorGUI.PropertyField(new Rect(rect.x + segmentWidth * GetArrayValue(eleSegment, 0, 1), rect.y, segmentWidth * eleSegment[1], EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("uiStateName"), GUIContent.none);

            EditorGUI.LabelField(new Rect(rect.x + segmentWidth * GetArrayValue(eleSegment, 0, 2), rect.y, segmentWidth * eleSegment[2], EditorGUIUtility.singleLineHeight), "UILayer");
            EditorGUI.PropertyField(new Rect(rect.x + segmentWidth * GetArrayValue(eleSegment, 0, 3), rect.y, segmentWidth * eleSegment[3], EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("uiLayer"), GUIContent.none);

            if (GUI.Button(new Rect(rect.x + segmentWidth * GetArrayValue(eleSegment, 0, 4), rect.y, segmentWidth * eleSegment[4], EditorGUIUtility.singleLineHeight), "+"))
            {
                // Insert
                list.serializedProperty.InsertArrayElementAtIndex(index);
            }

            if (GUI.Button(new Rect(rect.x + segmentWidth * GetArrayValue(eleSegment, 0, 5), rect.y, segmentWidth * eleSegment[5], EditorGUIUtility.singleLineHeight), "-"))
            {
                // Delete
                indexToDelete.Add(index);
            }
        };
    }

    public override void OnInspectorGUI()
    {
        //foldout = EditorGUILayout.Foldout(foldout, "UI Load List");
        EditorGUI.indentLevel++;
        EditorGUILayout.Space();

        SceneDefaultUI editing = (SceneDefaultUI)target;
        uiToLoadCount = EditorGUILayout.IntField("Size", uiToLoadCount);

        serializedObject.Update();
        list.DoLayoutList();
        if (indexToDelete.Count > 0)
        {
            for (int i = 0; i < indexToDelete.Count; i++)
            {
                list.serializedProperty.DeleteArrayElementAtIndex(indexToDelete[i]);
            }
            uiToLoadCount -= indexToDelete.Count;
            indexToDelete.Clear();
        }
        serializedObject.ApplyModifiedProperties();

        if (uiToLoadCount != editing.uiToLoad.Count)
        {
            if (uiToLoadCount == 0) editing.uiToLoad.Clear();
            else
            {
                List<SceneDefaultUI.UILoadConfig> newList = new List<SceneDefaultUI.UILoadConfig>(uiToLoadCount);
                for (int i = 0; i < uiToLoadCount; i++)
                {
                    newList.Add(new SceneDefaultUI.UILoadConfig());
                }

                for (int i = 0; i < editing.uiToLoad.Count; i++)
                {
                    newList[i].uiStateName = editing.uiToLoad[i].uiStateName;
                    newList[i].uiLayer = editing.uiToLoad[i].uiLayer;
                }

                editing.uiToLoad = newList;
            }
            EditorUtility.SetDirty(target);
        }

        EditorGUILayout.Space();
        EditorGUI.indentLevel--;
    }
}
*/