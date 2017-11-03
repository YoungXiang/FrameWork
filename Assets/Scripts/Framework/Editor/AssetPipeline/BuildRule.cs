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
        public string filterPath;
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
            }
            
            for (int p = 0; p < parsedList.Count; p++)
            {
                parsedList[p].assetBundleName = parsedList[p].assetBundleName.ToLower();
                //LogUtil.LogColor(LogUtil.Color.yellow, "AssetBundleName : [{0}]", parsed.assetBundleName);
                for (int i = 0; i < parsedList[p].assetFiles.Length; i++)
                {
                    //Debug.LogFormat("AssetFiles : {0}", parsed.assetFiles[i]);
                    parsedList[p].assetFiles[i] = parsedList[p].assetFiles[i].Replace("\\", "/").ToLower();
                }
            }
        }

        List<Parsed> ParseSingleRule(BuildRule rule)
        {
            List<string> finalFiles = new List<string>();

            string[] filterPathes = rule.filterPath.Split(';');
            for (int i = 0; i < filterPathes.Length; i++)
            {
                if (string.IsNullOrEmpty(filterPathes[i])) continue;
                string pattern = "*.*";
                if (filterPathes[i].Contains("."))
                {
                    pattern = Path.GetFileName(filterPathes[i]);
                }

                string searchFolder = Path.GetDirectoryName(filterPathes[i]);

                string[] files = Directory.GetFiles(searchFolder, pattern, SearchOption.AllDirectories);
                finalFiles.AddRange(files);
            }

            // ignore pathes
            string[] ignorePathes = rule.ignorePath.Split(';');
            for (int i = 0; i < ignorePathes.Length; i++)
            {
                if (string.IsNullOrEmpty(ignorePathes[i])) continue;
                string pattern = "*.*";
                if (ignorePathes[i].Contains("."))
                {
                    pattern = Path.GetFileName(ignorePathes[i]);
                }

                string searchFolder = Path.GetDirectoryName(ignorePathes[i]);

                string[] files = Directory.GetFiles(searchFolder, pattern, SearchOption.AllDirectories);
                foreach (string ignored in files)
                {
                    finalFiles.Remove(ignored);
                }
            }

            List<Parsed> _plist = new List<Parsed>();
            // note that one rule could create multiple parsed.
            // now let's parse the assetBundleName carefully
            string assetBundleName = rule.assetBundleName;
            if (!assetBundleName.Contains("{"))
            {
                Parsed parsed = new Parsed();
                parsed.assetBundleName = rule.assetBundleName;
                parsed.assetFiles = finalFiles.ToArray();
                parsed.isBuiltIn = rule.isBuiltIn;
                _plist.Add(parsed);
            }
            else
            {
                if (assetBundleName.Contains("{rootFolder}"))
                {
                    RootFolderFilter(rule, finalFiles, _plist);
                }
                else if (assetBundleName.Contains("{fileName}"))
                {
                    FileNameFilter(rule, finalFiles, _plist);
                }
            }

            return _plist;
        }

        // group allPathes by assetBundleName 
        // e.g : root path = Assets/Res/UI/
        //       file = Assets/Res/UI/main/a.png
        //       rootPath[0] = ui
        //       rootPath[1] = main
        void RootFolderFilter(BuildRule rule, List<string> allPathes, List<Parsed> list)
        {
            int indexFromRoot = 0;
            if (rule.assetBundleName.Contains("["))
            {
                {
                    Regex index = new Regex(@"\[([0-9]+)\]");
                    Match match = index.Match(rule.assetBundleName);
                    if (match.Success)
                    {
                        indexFromRoot = int.Parse(match.Groups[1].Value);
                    }
                }

                {
                    Regex index = new Regex(@"\[[0-9]+\]");
                    rule.assetBundleName = index.Replace(rule.assetBundleName, "");
                }
            }

            //Debug.Log("index from root : " + indexFromRoot);
            //Debug.Log("post replaced : " + rule.assetBundleName);

            string[] filterPathes = rule.filterPath.Split(';');
            List<string> rootFolders = new List<string>(filterPathes.Length);
            Dictionary<string, List<string>> groupByRootFolder = new Dictionary<string, List<string>>(filterPathes.Length);
            for (int i = 0; i < filterPathes.Length; i++)
            {
                string rootFolderName = Path.GetDirectoryName(filterPathes[i]);
                rootFolders.Add(rootFolderName.Replace("\\", "/"));
                groupByRootFolder.Add(rootFolderName.Replace("\\", "/"), new List<string>());
            }

            for (int i = 0; i < allPathes.Count; i++)
            {
                for (int r = 0; r < rootFolders.Count; r++)
                {
                    if (allPathes[i].Contains(rootFolders[r]))
                    {
                        string[] rootSplited = rootFolders[r].Split('/');
                        int rootIndex = rootSplited.Length - 1;
                        string[] pathSplited = allPathes[i].Replace("\\", "/").Split('/');

                        string abNameReplace = pathSplited[rootIndex + indexFromRoot];
                        string abName = rule.assetBundleName.Replace("{rootFolder}", abNameReplace);
                        if (!groupByRootFolder.ContainsKey(abName))
                        {
                            groupByRootFolder.Add(abName, new List<string>());
                        }
                        groupByRootFolder[abName].Add(allPathes[i]);
                        break;
                    }
                }
            }

            foreach (KeyValuePair<string, List<string>> pair in groupByRootFolder)
            {
                Parsed parsed = new Parsed();
                parsed.assetBundleName = pair.Key;
                parsed.assetFiles = pair.Value.ToArray();
                parsed.isBuiltIn = rule.isBuiltIn;
                list.Add(parsed);
            }
        }

        void FileNameFilter(BuildRule rule, List<string> allPathes, List<Parsed> list)
        {
            for (int i = 0; i < allPathes.Count; i++)
            {
                Parsed parsed = new Parsed();
                parsed.assetBundleName = rule.assetBundleName.Replace("{fileName}", Path.GetFileNameWithoutExtension(allPathes[i]));
                parsed.assetFiles = new string[] { allPathes[i] };
                parsed.isBuiltIn = rule.isBuiltIn;
                list.Add(parsed);
            }
        }
    }
}
