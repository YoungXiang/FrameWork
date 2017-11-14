using System;
using System.Data;

namespace JsonSharp
{
    public class SheetProperty
    {
        public string sheetName;
        public int rowCount;
        public int columnCount;
        public string[] sNames; // row[0]
        public string[] sTypes; // row[1]
        public string[] sDesc;  // row[3]
        public string[,] datas; // row i, colume j

        public int validColumnCount = 0;
        public int validDataRowCount = 0;

        public void Analyze(DataTable sheet)
        {
            validColumnCount = IsValidSheet(sheet);
            if (validColumnCount <= 0) return;

            sheetName = sheet.TableName;
            rowCount = sheet.Rows.Count;
            columnCount = sheet.Columns.Count;

            // parse header
            sNames = new string[columnCount];
            sTypes = new string[columnCount];
            sDesc = new string[columnCount];
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < columnCount; j++)
                {
                    if (i == 0)
                    {
                        // names;
                        sNames[j] = sheet.Rows[i][j].ToString().Trim();
                    }
                    else if (i == 1)
                    {
                        sTypes[j] = sheet.Rows[i][j].ToString().Trim();
                    }
                    else
                    {
                        sDesc[j] = sheet.Rows[i][j].ToString().Trim();
                    }
                }
            }
            // now parse data
            validDataRowCount = 0;
            for (int i = 3; i < rowCount; i++)
            {
                bool shouldBreak = false;
                for (int j = 0; j < columnCount; j++)
                {
                    string data = sheet.Rows[i][j].ToString().Trim();
                    if (string.IsNullOrEmpty(data))
                    {
                        shouldBreak = true;
                        break;
                    }
                }

                if (shouldBreak) break;
                else validDataRowCount++;
            }

            datas = new string[validDataRowCount, columnCount];
            for (int i = 0; i < validDataRowCount; i++)
            {
                for (int j = 0; j < columnCount; j++)
                {
                    string data = sheet.Rows[i + 3][j].ToString().Trim();
                    if (!string.IsNullOrEmpty(data))
                    {
                        datas[i, j] = data;
                    }
                }
            }
        }

        int IsValidSheet(DataTable sheet)
        {
            int valid = 0;
            for (int i = 0; i < sheet.Columns.Count; i++, valid++)
            {
                DataMemberType dm = DataMemberType.ParseDataMemberType(sheet.Rows[1][i].ToString().Trim());
                if (dm.eType == EDataMemberType.None)
                {
                    Console.WriteLine("[Error]InValid Type {0}: This Sheet is going to be ignored.[{1}]", sheet.Rows[1][i], sheet.TableName);
                    break;
                }
            }

            return valid;
        }
    }

    public class ExcelAnalyzer
    {
        public int sheetCount;
        public SheetProperty[] sheets;
        
        public void Analyze(DataSet book)
        {
            sheetCount = book.Tables.Count;
            sheets = new SheetProperty[sheetCount];
            for (int i = 0; i < sheetCount; i++)
            {
                sheets[i] = new SheetProperty();
                sheets[i].Analyze(book.Tables[i]);
            }
        }
    }
}
