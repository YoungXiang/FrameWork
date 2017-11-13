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

        public bool isValidSheet = true;

        public void Analyze(DataTable sheet)
        {
            isValidSheet = IsValidSheet(sheet);
            if (!isValidSheet) return;

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
                        sNames[j] = sheet.Rows[i][j].ToString();
                    }
                    else if (i == 1)
                    {
                        sTypes[j] = sheet.Rows[i][j].ToString();
                    }
                    else
                    {
                        sDesc[j] = sheet.Rows[i][j].ToString();
                    }
                }
            }
            // now parse data
            datas = new string[rowCount - 3, columnCount];
            for (int i = 3; i < rowCount; i++)
            {
                for (int j = 0; j < columnCount; j++)
                {
                    datas[i - 3, j] = sheet.Rows[i][j].ToString();
                }
            }
        }

        bool IsValidSheet(DataTable sheet)
        {
            for (int i = 0; i < sheet.Columns.Count; i++)
            {
                DataMemberType dm = DataMemberType.ParseDataMemberType(sheet.Rows[1][i].ToString());
                if (dm.eType == EDataMemberType.None)
                {
                    return false;
                }
            }

            return true;
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
