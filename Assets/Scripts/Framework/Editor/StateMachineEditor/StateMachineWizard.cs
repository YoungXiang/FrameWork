using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace FrameWork
{

    public class StateMachineWizard : ScriptableWizard
    {
        [MenuItem("FrameWork/Tool/StateMachine")]
        static void CreateWizard()
        {
            DisplayWizard<StateMachineWizard>("New StateMachine", "Create");
        }
        
        public string stateMachineName;
        public string defaultState;
        public List<string> states;
        
        private void OnWizardUpdate()
        {

        }

        private void OnWizardCreate()
        {
            // Create Clicked
            if (string.IsNullOrEmpty(stateMachineName))
            {
                EditorUtility.DisplayDialog("Invalid", "StateMachineName is empty.", "确定");
                return;
            }

            if (string.IsNullOrEmpty(defaultState))
            {
                EditorUtility.DisplayDialog("Invalid", "DefaultState is empty.", "确定");
                return;
            }

            if (states == null && states.Count <= 0)
            {
                EditorUtility.DisplayDialog("Invalid", "States is empty.", "确定");
                return;
            }

            string folderName = EditorUtility.OpenFolderPanel("保存位置", EditorPrefs.GetString("StateMachineSaveFolder", Application.streamingAssetsPath), "");
            if (!string.IsNullOrEmpty(folderName))
            {
                EditorPrefs.SetString("StateMachineSaveFolder", folderName);

                string finalFolder = folderName + "/" + stateMachineName;
                IOUtils.CreateDirectoryIfNotExist(finalFolder);
                CreateStateMachineClass(finalFolder);
            }
        }
        
        private void OnWizardOtherButton()
        {

        }

        void CreateStateMachineClass(string folder)
        {
            string[] lines = File.ReadAllLines("Assets/Scripts/FrameWork/Editor/StateMachineEditor/StateMachineTemplate.txt");
            List<string> newLines = new List<string>();
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].Contains("RegisterState"))
                {
                    for (int s = 0; s < states.Count; s++)
                    {
                        CreateState(folder, states[s]);
                        newLines.Add(lines[i].Replace("@StateName", states[s]));
                    }
                }
                else
                {
                    newLines.Add(lines[i].Replace("@StateMachineName", stateMachineName).Replace("@DefaultStateName", defaultState));
                }
            }
            File.WriteAllLines(string.Format("{0}/{1}.cs", folder, stateMachineName), newLines.ToArray());
        }

        void CreateState(string folder, string stateName)
        {
            string[] lines = File.ReadAllLines("Assets/Scripts/FrameWork/Editor/StateMachineEditor/StateTemplate.txt");
            List<string> newLines = new List<string>();
            for (int i = 0; i < lines.Length; i++)
            {
                newLines.Add(lines[i].Replace("@StateMachineName", stateMachineName).Replace("@StateName", stateName));
            }
            File.WriteAllLines(string.Format("{0}/{1}.cs", folder, stateName), newLines.ToArray());
        }
    }
}
