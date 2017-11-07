using System;
using System.Collections.Generic;
using System.Text;

namespace JsonSharp
{
    public class CSharpClassParser : IClassParser
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="classStr">xxx : { xxx:int, aaa:string }, first half is the vName, second half is vType</param>
        public ClassDefine Parse(string classStr)
        {
            ClassDefine cd = new ClassDefine();

            Dictionary<string, string> tree = new Dictionary<string, string>();

            StringBuilder parser = new StringBuilder();
            string left = string.Empty, right = string.Empty;

            int i = 0;
            while (i < classStr.Length)
            {
                char c = classStr[i];
                switch (c)
                {
                    case '{':
                        {
                            if (!string.IsNullOrEmpty(left))
                            {
                                cd.className = left;
                                parser.Remove(0, parser.Length);
                                left = string.Empty;
                            }

                            break;
                        }
                    case '}':
                        {
                            if (!string.IsNullOrEmpty(left))
                            {
                                if (!tree.ContainsKey(left))
                                {
                                    right = parser.ToString();
                                    parser.Remove(0, parser.Length);
                                    tree.Add(left, right);
                                    left = string.Empty; right = string.Empty;
                                }
                            }
                            else
                            {
                                if (parser.Length > 0)
                                {
                                    Console.WriteLine("[Error] Parse failed at {0}, ", parser.ToString());
                                    return cd;
                                }
                            }
                            break;
                        }
                    case ':':
                        {
                            left = parser.ToString();
                            parser.Remove(0, parser.Length);
                            break;
                        }
                    case ',':
                        {
                            if (!string.IsNullOrEmpty(left))
                            {
                                right = parser.ToString();
                                parser.Remove(0, parser.Length);
                                tree.Add(left, right);
                                left = string.Empty; right = string.Empty;
                            }
                            else
                            {
                                if (parser.Length > 0)
                                {
                                    Console.WriteLine("[Error] Parse failed at {0}, ", parser.ToString());
                                    return cd;
                                }
                            }
                            break;
                        }
                    case ' ':
                        break;
                    default:
                        {
                            parser.Append(c);
                            break;
                        }
                }
                i++;
            }

            Create(cd, tree);
            return cd;
        }

        public void Create(ClassDefine cd, Dictionary<string, string> tree)
        {
            foreach (KeyValuePair<string, string> pair in tree)
            {
                DataMemberDefine dm = new DataMemberDefine();
                dm.mName = pair.Key;
                dm.mType = DataMemberType.ParseDataMemberType(pair.Value);
                cd.members.Add(dm);
            }
        }
    }
}
