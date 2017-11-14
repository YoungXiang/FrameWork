using System.IO;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

namespace FrameWork
{
    // Todo: Add Custom Attribute : AddMenuItem("Path", "Params");
    [System.Serializable]
    public class BuildRule
    {
        public string ruleName;
        // the main filter path, doesn't allow multi pathes
        public string filterPath;
        // the path should be patch into filterPath
        public string includePath;
        // the path should be ignored
        public string ignorePath;

        public string assetBundleName;
        public bool isBuiltIn;
    }

    [System.Serializable]
    public class BuildRuleJson
    {
        public BuildRule[] rules;
    }

    public class BuildRuleParser
    {
        public class Parsed
        {
            public string assetBundleName;
            public string[] assetFiles;
            public bool isBuiltIn;
        }

        public List<Parsed> parsedList = new List<Parsed>();
        public BuildRuleJson ruleJson;

        void LoadRules(string path)
        {
            ruleJson = JsonUtility.FromJson<BuildRuleJson>(File.ReadAllText(path));
        }

        public void Parse(string path)
        {
            LoadRules(path);

            for (int i = 0; i < ruleJson.rules.Length; i++)
            {
                parsedList.AddRange(ParseSingleRule(ruleJson.rules[i]));
                LogUtil.LogColor(LogUtil.Color.yellow, "[Build] : Build rule = [{0}] parsed.", ruleJson.rules[i].ruleName);
            }
            
            for (int p = 0; p < parsedList.Count; p++)
            {
                parsedList[p].assetBundleName = parsedList[p].assetBundleName.ToLower();
                //LogUtil.LogColor(LogUtil.Color.yellow, "AssetBundleName : [{0}]", parsedList[p].assetBundleName);
                for (int i = 0; i < parsedList[p].assetFiles.Length; i++)
                {
                    parsedList[p].assetFiles[i] = parsedList[p].assetFiles[i].Replace("\\", "/").ToLower();
                    //Debug.LogFormat("AssetFiles : {0}", parsedList[p].assetFiles[i]);
                }
            }
        }

        string[] GetFiles(string path)
        {
            string pattern = "*.*";
            if (path.Contains("."))
            {
                pattern = Path.GetFileName(path);
            }
            List<string> finalFiles = new List<string>();
            try
            {
                string root = Path.GetDirectoryName(path);
                if (!Directory.Exists(root)) return finalFiles.ToArray();
                string[] files = Directory.GetFiles(root, pattern, SearchOption.AllDirectories);
                for (int i = 0; i < files.Length; i++)
                {
                    if (files[i].EndsWith(".meta")) continue;
                    finalFiles.Add(files[i]);
                }
            }
            catch(System.Exception e)
            {
                Debug.LogErrorFormat("[Error]: {0}\n{1}", path, e.Message);
            }
            return finalFiles.ToArray();
        }

        List<Parsed> ParseSingleRule(BuildRule rule)
        {
            List<Parsed> _plist = new List<Parsed>();
            if (rule.filterPath.Contains("{fileName}"))
            {
                List<string> filteredFiles = new List<string>();
                // grouped by fileName
                Dictionary<string, List<string>> gf = new Dictionary<string, List<string>>();
                string filterPath = rule.filterPath.Replace("{fileName}", "*");
                filteredFiles.AddRange(GetFiles(filterPath));

                string[] ignorePathes = rule.ignorePath.Split(';');
                foreach (string ignorePath in ignorePathes)
                {
                    if (string.IsNullOrEmpty(ignorePath)) continue;
                    string path = ignorePath.Replace("{fileName}", "*");
                    string[] ignoreFiles = GetFiles(path);
                    foreach (string ignored in ignoreFiles)
                    {
                        filteredFiles.Remove(ignored);
                    }
                }

                for (int i = 0; i < filteredFiles.Count; i++)
                {
                    string fileName = Path.GetFileNameWithoutExtension(filteredFiles[i]);
                    gf.Add(fileName, new List<string>());
                    gf[fileName].Add(filteredFiles[i]);

                    string[] incPathes = rule.includePath.Split(';');
                    foreach (string incPath in incPathes)
                    {
                        if (string.IsNullOrEmpty(incPath) || !incPath.Contains("{fileName}")) continue; // gets ignored
                        string[] incFiles = GetFiles(incPath.Replace("{fileName}", fileName));
                        gf[fileName].AddRange(incFiles);
                    }
                }
               
                // fill the parsed
                foreach (KeyValuePair<string, List<string>> pair in gf)
                {
                    Parsed p = new Parsed();
                    p.assetBundleName = rule.assetBundleName.Replace("{fileName}", pair.Key);
                    if (p.assetBundleName.Contains("{id}"))
                    {
                        p.assetBundleName = p.assetBundleName.Replace("{id}", IDFromFiles(pair.Value).ToString());
                    }
                    p.assetFiles = pair.Value.ToArray();
                    p.isBuiltIn = rule.isBuiltIn;
                    _plist.Add(p);
                } 
            }
            else if (rule.filterPath.Contains("{folderName}"))
            {
                List<string> filteredFiles = new List<string>();
                // grouped by folderName
                Dictionary<string, List<string>> gf = new Dictionary<string, List<string>>();
                int indexStart = rule.filterPath.IndexOf("{folderName}");
                string filterPrePath = rule.filterPath.Substring(0, indexStart);
                string[] folders = Directory.GetDirectories(filterPrePath);

                for (int i = 0; i < folders.Length; i++)
                {
                    string[] folderNames = folders[i].Replace("\\", "/").Split('/');
                    string folderName = folderNames[folderNames.Length - 1];

                    string filterPath = rule.filterPath.Replace("{folderName}", folderName);
                    filteredFiles.AddRange(GetFiles(filterPath));
                    
                    string[] ignorePathes = rule.ignorePath.Split(';');
                    foreach (string ignorePath in ignorePathes)
                    {
                        if (string.IsNullOrEmpty(ignorePath)) continue;
                        string path = ignorePath.Replace("{folderName}", folderName);
                        string[] ignoreFiles = GetFiles(path);
                        foreach (string ignored in ignoreFiles)
                        {
                            filteredFiles.Remove(ignored);
                        }
                    }
                    
                    gf.Add(folderName, new List<string>());
                    gf[folderName].AddRange(filteredFiles);

                    string[] incPathes = rule.includePath.Split(';');
                    foreach (string incPath in incPathes)
                    {
                        if (string.IsNullOrEmpty(incPath) || !incPath.Contains("{folderName}")) continue; // gets ignored
                        string[] incFiles = GetFiles(incPath.Replace("{folderName}", folderName));
                        gf[folderName].AddRange(incFiles);
                    }
                }

                // fill the parsed
                foreach (KeyValuePair<string, List<string>> pair in gf)
                {
                    Parsed p = new Parsed();
                    p.assetBundleName = rule.assetBundleName.Replace("{folderName}", pair.Key);
                    if (p.assetBundleName.Contains("{id}"))
                    {
                        p.assetBundleName = p.assetBundleName.Replace("{id}", IDFromFiles(pair.Value).ToString());
                    }
                    p.assetFiles = pair.Value.ToArray();
                    p.isBuiltIn = rule.isBuiltIn;
                    _plist.Add(p);
                }
            }
            else
            {
                List<string> filteredFiles = new List<string>();
                filteredFiles.AddRange(GetFiles(rule.filterPath));

                string[] ignorePathes = rule.ignorePath.Split(';');
                foreach (string ignorePath in ignorePathes)
                {
                    if (string.IsNullOrEmpty(ignorePath)) continue;
                    string[] ignoreFiles = GetFiles(ignorePath);
                    foreach (string ignored in ignoreFiles)
                    {
                        filteredFiles.Remove(ignored);
                    }
                }

                string[] incPathes = rule.includePath.Split(';');
                foreach (string incPath in incPathes)
                {
                    if (string.IsNullOrEmpty(incPath)) continue; // gets ignored
                    string[] incFiles = GetFiles(incPath);
                    filteredFiles.AddRange(incFiles);
                }

                Parsed p = new Parsed();
                p.assetBundleName = rule.assetBundleName;
                if (p.assetBundleName.Contains("{id}"))
                {
                    p.assetBundleName = p.assetBundleName.Replace("{id}", IDFromFiles(filteredFiles).ToString());
                }
                p.assetFiles = filteredFiles.ToArray();
                p.isBuiltIn = rule.isBuiltIn;
                _plist.Add(p);
            }

            return _plist;
        }

        int IDFromFiles(List<string> filePathes)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            for (int i = 0; i < filePathes.Count; i++)
            {
                sb.Append(filePathes[i].GetHashCode());
            }

            return sb.GetHashCode();
        }

        // filterStr : "ui/icon/{rootFolder}[1].unity3d"
        void ExtractRootFolderIndex(ref string filterStr, out int index)
        {
            index = 0;
            if (filterStr.Contains("["))
            {
                {
                    Regex pattern = new Regex(@"\[([0-9]+)\]");
                    Match match = pattern.Match(filterStr);
                    if (match.Success)
                    {
                        index = int.Parse(match.Groups[1].Value);
                    }
                }

                {
                    Regex pattern = new Regex(@"\[[0-9]+\]");
                    filterStr = pattern.Replace(filterStr, "");
                }
            }
        }
    }
}
