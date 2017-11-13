#define USE_MESSAGEPACK

using System;
using System.IO;
using System.Data;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using MessagePack;
using FrameWork;

using Excel;

namespace JsonSharp.Test
{
    public class MainTest
    {
        public static void ExcelStart(string excelPath, string csOutputDir, string dataOutputDir, bool useFileName)
        {
            Console.WriteLine("***********************************");
            Console.WriteLine("Converting File: [{0}]", excelPath);
            string fileName = Path.GetFileNameWithoutExtension(excelPath);
            using (FileStream excelFile = File.Open(excelPath, FileMode.Open, FileAccess.Read))
            {
                // Reading from a OpenXml Excel file (2007 format; *.xlsx)
                IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(excelFile);

                // The result of each spreadsheet will be created in the result.Tables
                excelReader.IsFirstRowAsColumnNames = false;
                DataSet book = excelReader.AsDataSet();

                ExcelAnalyzer analyzer = new ExcelAnalyzer();
                analyzer.Analyze(book);

                CSharpGenerator generator = new CSharpGenerator();
                for (int i = 0; i < analyzer.sheetCount; i++)
                {
                    SheetProperty sp = analyzer.sheets[i];
                    if (!sp.isValidSheet) continue;

                    ClassDefine cd = new ClassDefine();
                    cd.className = useFileName ? fileName : sp.sheetName;
                    for (int m = 0; m < sp.sTypes.Length; m++)
                    {
                        DataMemberDefine dmd = new DataMemberDefine();
                        dmd.mType = DataMemberType.ParseDataMemberType(sp.sTypes[m]);
                        dmd.mName = sp.sNames[m];
                        cd.members.Add(dmd);
                    }

                    //-- 保存文件
                    string ret = generator.Generate(cd);
                    //Console.WriteLine(ret);
                    string classPath = Path.Combine(csOutputDir, cd.className + ".cs");
                    if (File.Exists(classPath)) File.Delete(classPath);
                    if (!Directory.Exists(Path.GetDirectoryName(classPath))) Directory.CreateDirectory(Path.GetDirectoryName(classPath));
                    File.WriteAllText(classPath, ret);

                    Type[] types = CSharpSourceCompiler.CompileAndReturnInstance(ret);
                    Type dataClassType = types[0];
                    Type containterClassType = types[1];

                    //-- 导出数据
                    object container = Activator.CreateInstance(containterClassType);
                    if (container == null)
                    {
                        Console.WriteLine("Instance Create Failed : No classType found = " + containterClassType.ToString());
                        return;
                    }

                    Array dataArray = Array.CreateInstance(dataClassType, sp.datas.GetLength(0));
                    for (int r = 0; r < sp.datas.GetLength(0); r++)
                    {
                        object element = Activator.CreateInstance(dataClassType);
                        if (element == null)
                        {
                            Console.WriteLine("Instance Create Failed : No classType found = " + dataClassType.ToString());
                            return;
                        }

                        // each row
                        for (int c = 0; c < sp.datas.GetLength(1); c++)
                        {
                            string dataName = sp.sNames[c];
                            string dataValue = sp.datas[r, c];
                            ObjectFill.TryFill(ref element, dataName, dataValue, cd.members[c].mType);
                        }

                        dataArray.SetValue(element, r);
                        //OutputObject(obj);
                    }

                    FieldInfo datasField = container.GetType().GetField("datas");
                    if (datasField == null) Console.WriteLine("FieldInfo datas is null.");
                    datasField.SetValue(container, dataArray);

                    // serialize
                    byte[] data = null;
#if USE_MESSAGEPACK
                    data = MessagePackSerializer.Serialize(container);
#else
                    var binFormatter = new BinaryFormatter();
                    var memStream = new MemoryStream();
                    binFormatter.Serialize(memStream, container);
                    //This gives you the byte array.
                    data = memStream.ToArray();
#endif

                    string dataPath = Path.Combine(dataOutputDir, cd.className + "Data.data");
                    if (File.Exists(dataPath)) File.Delete(dataPath);
                    if (!Directory.Exists(Path.GetDirectoryName(dataPath))) Directory.CreateDirectory(Path.GetDirectoryName(dataPath));
                    File.WriteAllBytes(dataPath, data);
                }

                Console.WriteLine("Convert Success!");
                Console.WriteLine("***********************************");
            }
        }

        public static void OutputObject(object obj)
        {
            Console.WriteLine("===================================");
            Console.WriteLine("Type : ", obj.GetType().ToString());
            FieldInfo[] infos = obj.GetType().GetFields();
            foreach (FieldInfo info in infos)
            {
                Console.WriteLine("{0}:{1}", info.ToString(), info.GetValue(obj));
            }
            Console.WriteLine("===================================");
        }

        public static void Main(string[] args)
        {
            //string classStr = "classA : { aaa:int, bbb:string, ccc:float[] }";
            //Console.WriteLine(CSharpGenerator.Generate(classStr));
            if (args == null || args.Length < 2)
            {
                Console.WriteLine("Argument Is InValid: Length must be >= 2.");
                Console.WriteLine("[searchDirectory, outputDirectory]");
                Console.ReadKey();
                return;
            }

            string rootDir = args[0];
            string csOutputDir = args[1];
            string dataOutputDir = args[2];
            // else use sheetName
            bool useFileName = args.Length > 3 ? int.Parse(args[3]) == 0 ? false : true : true;
            
            if (!Directory.Exists(rootDir))
            {
                Console.WriteLine("Target directory = [{0}] does not exist.", rootDir);
                Console.ReadKey();
                return;
            }

            string[] files = Directory.GetFiles(rootDir, "*.xlsx", SearchOption.AllDirectories);
            if (files.Length <= 0)
            {
                Console.WriteLine("None Excel(.xlsx) files were found in directory = [{0}].", rootDir);
                Console.ReadKey();
                return;
            }

            foreach (string xlsxFile in files)
            {
                //"D:/Project/C#/JsonSharp/JsonSharp/Character.xlsx"
                ExcelStart(xlsxFile, csOutputDir, dataOutputDir, useFileName);
            }
        }
    }
}