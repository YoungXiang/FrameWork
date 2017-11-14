using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace JsonSharp
{
    public class ObjectFill
    {
        public static void TryFill(ref object obj, string fieldName, string fieldValue, DataMemberType dmt)
        {
            try
            {
                FieldInfo fieldInfo = obj.GetType().GetField(fieldName);
                if (fieldInfo == null)
                {
                    Console.WriteLine("[ObjectFill] FiedInfo {0} is null.", fieldName);
                    return;
                }
                switch(dmt.eType)
                {
                    case EDataMemberType.Int:
                        fieldInfo.SetValue(obj, Convert.ChangeType(fieldValue, fieldInfo.FieldType));
                        break;
                    case EDataMemberType.Float:
                        fieldInfo.SetValue(obj, Convert.ChangeType(fieldValue, fieldInfo.FieldType));
                        break;
                    case EDataMemberType.Bool:
                        fieldInfo.SetValue(obj, Convert.ChangeType(fieldValue, fieldInfo.FieldType));
                        break;
                    case EDataMemberType.String:
                        fieldInfo.SetValue(obj, Convert.ChangeType(fieldValue, fieldInfo.FieldType));
                        break;
                    case EDataMemberType.Array: // array
                        string[] arrayValue = fieldValue.Split(',');
                        int validCount = 0;
                        for (int i = 0; i < arrayValue.Length; i++)
                        {
                            if (!string.IsNullOrEmpty(arrayValue[i])) validCount++;
                        }
                        Type elementType = DataMemberType.ETypeToSystemType(dmt.childType.eType);
                        if (elementType == null)
                        {
                            Console.WriteLine("[Error]: Array element type not supported! {0}", fieldName);
                            break;
                        }
                        Array dataArray = Array.CreateInstance(elementType, validCount);
                        validCount = 0;
                        for (int i = 0; i < arrayValue.Length; i++)
                        {
                            if (!string.IsNullOrEmpty(arrayValue[i]))
                            {
                                dataArray.SetValue(Convert.ChangeType(arrayValue[i], elementType), validCount++);
                            }
                        }
                        fieldInfo.SetValue(obj, dataArray);
                        break;
                    case EDataMemberType.List:

                    case EDataMemberType.Map:

                    default: break;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("[ObjectFill] Failed. Fill {0} with {1}", fieldName, fieldValue);
                Console.WriteLine(e.Message);
            }
        }
    }
}
