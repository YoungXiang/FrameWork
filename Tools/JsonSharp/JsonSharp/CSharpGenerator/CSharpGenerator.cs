#define USE_MESSAGEPACK

using System;
using System.Collections.Generic;
using System.Text;

namespace JsonSharp
{
    public class CSharpGenerator : IClassGenerator
    {
        // MessagePack Template
        public static readonly string template = @"
using System;
using System.Collections.Generic;
using MessagePack;

namespace FrameWork
{
    [MessagePackObject]
    public class @{ClassName}
    {
        @{InsertMember}
    }

    [MessagePackObject]
    public class @{ClassName}Data
    {
        [Key(0)]
        public @{ClassName}[] datas;

        [IgnoreMember]
        public int Count { get { return datas.Length; } }
        
        [IgnoreMember]
        public @{ClassName} this[int index]
        {
            get 
            {
                if (index >= 0 && index < datas.Length) return datas[index];
                return null;
            }
        }

        public delegate bool SearchFilter(@{ClassName} item);

        public @{ClassName} Search(SearchFilter filter)
        {
            for (int i = 0; i < datas.Length; i++)
            {
                if (filter(datas[i])) return datas[i];
            }

            return null;
        }
    }
}
";

        int indentLevel = 0;
        ITypeGenerator converter;
        public CSharpGenerator()
        {
            converter = new CSharpTypeGenerator();
        }

        public StringBuilder GetIndent(StringBuilder sb)
        {
            for (int i = 0; i < indentLevel; i++)
            {
                sb.Append("\t");
            }
            return sb;
        }

        public string Generate(ClassDefine cd)
        {
            if (cd == null || cd.members.Count <= 0) return string.Empty;

            indentLevel = 0;

            StringBuilder sb = new StringBuilder();
            /*
            GenerateHeader(cd, sb);
            GenerateClassHeader(cd, sb);
            GenerateClassBody(cd, sb);
            GenerateClassEnd(cd, sb);
            */
            GenerateUsingTemplate(cd, sb);
            
            return sb.ToString();
        }

        void GenerateUsingTemplate(ClassDefine cd, StringBuilder sb)
        {
            sb.AppendLine("// Auto-Gen " + Common.GetVersion());
            string[] lines = template.Split('\n');
            foreach(string line in lines)
            {
                if (line.Contains("@{InsertMember}"))
                {
                    for (int i = 0; i < cd.members.Count; i++)
                    {
                        sb.Append(line.Replace("@{InsertMember}", string.Format("[Key({0})]", i)));
                        sb.Append(line.Replace("@{InsertMember}", string.Format("public {0} {1};", converter.Generate(cd.members[i].mType), cd.members[i].mName)));
                    }
                }
                else
                {
                    sb.Append(line.Replace("@{ClassName}", cd.className));
                }
            }
        }

        void GenerateHeader(ClassDefine ct, StringBuilder sb)
        {
            GetIndent(sb).AppendLine("// Auto-gen " + Common.GetVersion());
            GetIndent(sb).AppendLine("using System;");
            GetIndent(sb).AppendLine("using System.Collections.Generic;");
#if USE_MESSAGEPACK
            GetIndent(sb).AppendLine("using MessagePack;");
#endif
            GetIndent(sb).AppendLine();
        }

        void GenerateClassHeader(ClassDefine ct, StringBuilder sb)
        {
            sb.AppendLine("namespace FrameWork").AppendLine("{");
            indentLevel++;
#if USE_MESSAGEPACK
            GetIndent(sb).AppendLine("[MessagePackObject]");
#else
            GetIndent(sb).AppendLine("[Serializable]");
#endif
            GetIndent(sb).AppendLine(string.Format("public class {0}", ct.className));
            GetIndent(sb).AppendLine("{");
            indentLevel++;
        }

        void GenerateClassBody(ClassDefine ct, StringBuilder sb)
        {
            for (int i = 0; i < ct.members.Count; i++)
            {
#if USE_MESSAGEPACK
                GetIndent(sb).AppendLine(string.Format("[Key({0})]", i));
#endif
                GetIndent(sb).AppendLine(string.Format("public {0} {1};", converter.Generate(ct.members[i].mType), ct.members[i].mName));
            }
        }

        void GenerateClassEnd(ClassDefine ct, StringBuilder sb)
        {
            indentLevel--;
            GetIndent(sb).AppendLine("}");

            GenerateDataClass(ct, sb);

            indentLevel--;
            GetIndent(sb).AppendLine("}");
        }

        void GenerateDataClass(ClassDefine cd, StringBuilder sb)
        {
            GetIndent(sb).AppendLine();
#if USE_MESSAGEPACK
            GetIndent(sb).AppendLine("[MessagePackObject]");
#else
            GetIndent(sb).AppendLine("[Serializable]");
#endif
            GetIndent(sb).AppendLine(string.Format("public class {0}Data", cd.className));
            GetIndent(sb).AppendLine("{");
            indentLevel++;
#if USE_MESSAGEPACK
            GetIndent(sb).AppendLine("[Key(0)]");
#endif
            GetIndent(sb).AppendLine(string.Format("public {0}[] datas;", cd.className));

            // Add indexers and Search methods
            GetIndent(sb).AppendLine(string.Format("public delegate bool SearchFilter({0} val);", cd.className)).AppendLine();
            GetIndent(sb).AppendLine(string.Format("public {0} this[int index]", cd.className));
            GetIndent(sb).AppendLine("{ get { return datas[index]; } }");
            GetIndent(sb).AppendLine(string.Format("public {0} Search(SearchFilter filter)", cd.className));
            GetIndent(sb).AppendLine("{ for (int i = 0; i < datas.Length; i++) { if (filter(datas[i])) return datas[i]; } return null; }").AppendLine();

            indentLevel--;
            GetIndent(sb).AppendLine("}");
        }
        
        static IClassParser parser = new CSharpClassParser();
        static IClassGenerator gen = new CSharpGenerator();
        /// <summary>
        /// General Methods for conveniency.
        /// </summary>
        /// <param name="classStr"></param>
        /// <returns></returns>
        public static string Generate(string classStr)
        {
            return gen.Generate(parser.Parse(classStr));
        }
    }
}
