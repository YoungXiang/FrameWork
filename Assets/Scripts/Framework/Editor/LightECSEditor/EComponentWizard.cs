using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace FrameWork
{
    public class EComponentWizard : ScriptableWizard
    {

        [MenuItem("FrameWork/Tool/ECS-Component")]
        static void CreateWizard()
        {
            DisplayWizard<EComponentWizard>("New Component", "Create");
        }

        [Tooltip("e.g : Name = xxx, Generated class name will be EComponentxxx")]
        public string componentName;

        private void OnWizardCreate()
        {
            if (string.IsNullOrEmpty(componentName))
            {
                EditorUtility.DisplayDialog("Invalid", "ComponentName is empty.", "确定");
                return;
            }





        }
    }
}