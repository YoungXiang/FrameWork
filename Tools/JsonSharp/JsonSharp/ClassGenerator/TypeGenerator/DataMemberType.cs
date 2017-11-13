using System;
using System.Collections.Generic;
using System.Text;

namespace JsonSharp
{
    public enum EDataMemberType
    {
        Int,
        Float,
        String,
        Bool,
        Array,
        List,
        Map,
        None
    }   

    public class DataMemberType
    {
        public string referenceString = string.Empty;
        public EDataMemberType eType = EDataMemberType.None;
        public DataMemberType childType; // for type like: int[,] or List<List<List<int>>>
        public DataMemberType childType1; // for type like: map<int, string>

        public static DataMemberType ParseDataMemberType(string dataValue)
        {
            DataMemberType tt = new DataMemberType();
            switch (dataValue.Trim())
            {
                case "int":
                    tt.eType = EDataMemberType.Int;
                    break;
                case "float":
                    tt.eType = EDataMemberType.Float;
                    break;
                case "string":
                    tt.eType = EDataMemberType.String;
                    break;
                case "bool":
                    tt.eType = EDataMemberType.Bool;
                    break;
                default: break;
            }

            if (tt.eType == EDataMemberType.None)
            {
                if (dataValue.Contains("[]"))
                {
                    tt.eType = EDataMemberType.Array;
                    tt.referenceString = dataValue;
                    tt.childType = ParseDataMemberType(dataValue.Replace("[]", ""));
                }
                else if (dataValue.StartsWith("list"))  // list<int>
                {
                    tt.eType = EDataMemberType.List;
                    tt.childType = ParseDataMemberType(GetBracketsValue(dataValue, '<', '>'));
                }
                else if (dataValue.StartsWith("map"))      // map<int, int>
                {
                    tt.eType = EDataMemberType.Map;
                    string brackedValue = GetBracketsValue(dataValue, '<', '>');
                    string childValue1 = brackedValue.Substring(0, brackedValue.IndexOf(','));
                    tt.childType = ParseDataMemberType(childValue1);
                    string childValue2 = brackedValue.Substring(childValue1.Length + 1, brackedValue.Length - childValue1.Length - 1);
                    tt.childType1 = ParseDataMemberType(childValue2);
                }
            }
            return tt;
        }

        /// <summary>
        /// Input: <a> , Output: a.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string GetBracketsValue(string input, char brackLeft, char brackRight)
        {
            int length = input.Length;
            int istart = input.IndexOf(brackLeft);
            int ilast = input.LastIndexOf(brackRight);
            if (istart < 0 || ilast < 0)
            {
                return string.Format("Error : {0} - {1}{2} not match", input, brackLeft, brackRight);
            }

            return input.Substring(istart + 1, ilast - istart - 1);
        }

        private static Dictionary<EDataMemberType, Type> typeMap = new Dictionary<EDataMemberType, Type>()
        {
            { EDataMemberType.Int, typeof(int) },
            { EDataMemberType.Float, typeof(float) },
            { EDataMemberType.String, typeof(string) },
            { EDataMemberType.Bool, typeof(bool) }
        };

        public static Type ETypeToSystemType(EDataMemberType etype)
        {
            if (etype < EDataMemberType.Array)
            {
                return typeMap[etype];
            }

            return null;
        }
    }
}
