using NVP.API.Nodes;

using Excel = Microsoft.Office.Interop.Excel;

using System.Collections.Generic;
using System.IO;

namespace NVP_Libs.Common
{
    [NodeInput("полный путь", typeof(string))]
    [NodeInput("текст который вводим", typeof(string))]
    [NodeInput("клетка", typeof(string))]
    [NodeInput("имя листа", typeof(string))]
    public class ExcelSingleWrite : INode
    {
        public NodeResult Execute(INVPData context, List<NodeResult> inputs)
        {
            string fileName = (string)inputs[0].Value;
            string text = (string)inputs[1].Value;
            string cell = (string)inputs[2].Value;
            string nameofSheet = (string)inputs[3].Value;


                if (!File.Exists(fileName))
                {
                    return new NodeResult("Файл не существует.");
                }

                Excel.Application excelApp = new Excel.Application();
                Excel.Workbook workbook = excelApp.Workbooks.Open(fileName);
                Excel.Worksheet worksheet = workbook.Sheets[nameofSheet];

                if (worksheet == null)
                {
                    return new NodeResult($"Лист {nameofSheet} не существует в файле {fileName}.");
                }

                worksheet.Range[cell].Value2 = text;

                workbook.Save();
                workbook.Close();
                excelApp.Quit();

                return new NodeResult(fileName);

        }
    }
}