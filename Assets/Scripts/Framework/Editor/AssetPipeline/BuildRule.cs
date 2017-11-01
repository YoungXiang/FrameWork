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
        }

        public List<Parsed> parsedList;
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
        }

        List<Parsed> ParseSingleRule(BuildRule rule)
        {
            List<string> finalFiles = new List<string>();

            string[] filterPathes = rule.filterPath.Split(';');
            for (int i = 0; i < filterPathes.Length; i++)
            {
                string pattern = "*.*";
                if (filterPathes[i].Contains("."))
                {
                    pattern = Path.GetFileName(filterPathes[i]);
                }
                string[] files = Directory.GetFiles(filterPathes[i], pattern, SearchOption.AllDirectories);
                finalFiles.AddRange(files);
            }

            // ignore pathes
            string[] ignorePathes = rule.ignorePath.Split(';');
            for (int i = 0; i < ignorePathes.Length; i++)
            {
                string pattern = "*.*";
                if (filterPathes[i].Contains("."))
                {
                    pattern = Path.GetFileName(filterPathes[i]);
                }
                string[] files = Directory.GetFiles(filterPathes[i], pattern, SearchOption.AllDirectories);
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
                _plist.Add(parsed);
            }
            else
            {
                {
                    Regex folderName = new Regex("FolderName\\((.*)\\)(.*)");
                    Match match = folderName.Match(assetBundleName);
                    if (match.Success)
                    {
                        string rootPath = match.Groups[1].Value;
                        
                    }
                }
                {
                    Regex fileName = new Regex("FileName\\((.*)\\)(.*)");
                    Match match = fileName.Match(assetBundleName);
                    if (match.Success)
                    {

                    }
                }

            }

            return _plist;
        }
    }
}
