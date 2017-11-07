using System;
using System.Collections.Generic;
using System.Text;

namespace JsonSharp
{
    public class CSharpTypeGenerator : ITypeGenerator
    {
        public string Generate(DataMemberType inputType)
        {
            switch (inputType.eType)
            {
                case EDataMemberType.Int:
                    {
                        return "int";
                    }
                case EDataMemberType.Float:
                    {
                        return "float";
                    }
                case EDataMemberType.Bool:
                    {
                        return "bool";
                    }
                case EDataMemberType.String:
                    {
                        return "string";
                    }
                case EDataMemberType.Array:
                    {
                        return string.Format("{0}", inputType.referenceString);
                    }
                case EDataMemberType.List:
                    {
                        if (inputType.childType == null)
                        {
                            return "Error Converting List Type.";
                        }
                        return string.Format("List<{0}>", Generate(inputType.childType));
                    }
                case EDataMemberType.Map:
                    {
                        return string.Format("Dictionary<{0}, {1}>", Generate(inputType.childType), Generate(inputType.childType1));
                    }
                default: break;
            }
            return "UnSupportedType : " + inputType.eType.ToString();
        }
    }
}
