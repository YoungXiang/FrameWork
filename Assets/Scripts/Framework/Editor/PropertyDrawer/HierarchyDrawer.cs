using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class HierarchyDragData
{
    public const string identifier = "ADXWJDFA_HIERARCHY";
    public string hierarchy;
}

[CustomPropertyDrawer(typeof(HierarchyAttribute))]
public class HierarchyDrawer : PropertyDrawer
{
    // Draw the property inside the given rect
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // First get the attribute since it contains the range for the slider
        HierarchyAttribute hierarchy = attribute as HierarchyAttribute;
        
        Event currentEvent = Event.current;
        EventType currentEventType = currentEvent.type;

        EditorGUI.LabelField(position, label.text);
        position.x += 70;
        property.stringValue = EditorGUI.TextField(position, property.stringValue);

        // https://forum.unity.com/threads/drag-and-drop-in-the-editor-explanation.223242/
        switch (currentEventType)
        {
            case EventType.MouseDown:
                //Debug.Log("MouseDown");
                break;
            case EventType.MouseDrag:
                //Debug.Log("MouseDrag");
                break;
            case EventType.DragUpdated:
                //Debug.Log("DragUpdated");
                if (IsDragTargetValid())
                {
                    if (position.Contains(currentEvent.mousePosition) && GUI.enabled)
                        DragAndDrop.visualMode = DragAndDropVisualMode.Link;
                }
                else DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;

                //Debug.LogFormat("DragAndDrop : {0} - {1}", DragAndDrop.objectReferences, DragAndDrop.objectReferences.Length);

                currentEvent.Use();                
                break;
            case EventType.Repaint:
                break;
            case EventType.DragPerform:
                //Debug.Log("DragPerform");
                if (position.Contains(currentEvent.mousePosition) && GUI.enabled)
                {
                    property.stringValue = GetGameObjectHierarchy(DragAndDrop.objectReferences[0]);
                    currentEvent.Use();
                }
                break;
            case EventType.DragExited:
                //Debug.Log("DragExited");
                if (GUI.enabled)
                {
                    HandleUtility.Repaint();
                }
                break; 
                           
            case EventType.MouseUp:
                //Debug.Log("MouseUp");
                break;
            default:break;
        }
    }

    bool IsDragTargetValid()
    {
        GameObject go = DragAndDrop.objectReferences[0] as GameObject;
        if (go != null)
        {
            return true;
        }

        return false;
    }

    string GetGameObjectHierarchy(Object obj)
    {
        GameObject go = obj as GameObject;
        if (go == null) return string.Empty;

        System.Text.StringBuilder sb = new System.Text.StringBuilder();

        Transform trans = go.transform;
        while (trans != null)
        {
            sb.Append(trans.name).Append("/");
            trans = trans.parent;
        }
        sb.Remove(sb.Length - 1, 1);

        string[] rets = sb.ToString().Split('/');
        sb.Remove(0, sb.Length);

        for (int i = rets.Length - 1; i >= 0; i--)
        {
            sb.Append(rets[i]).Append("/");
        }
        sb.Remove(sb.Length - 1, 1);
        
        return sb.ToString();
    }
}
