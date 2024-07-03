using NVP.API.Nodes;

using Serilog;

using Excel = Microsoft.Office.Interop.Excel;

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;



namespace NVP_Libs.Common
{
    [NodeInput("Полный путь", typeof(string))]
    [NodeInput("Текст который вводим", typeof(string))]
    [NodeInput("Клетка", typeof(string))]
    [NodeInput("Имя Листа", typeof(string))]
    public class ExcelSingleWrite : IRevitNode
    {
        public NodeResult Execute(IVisualViewerData context, List<NodeResult> inputs, object commandData)
        {
            string fileName = (string)inputs[0].Value;
            string text = (string)inputs[1].Value;
            string cell = (string)inputs[2].Value;
            string nameofSheet = (string)inputs[3].Value;

            try
            {
                if (!File.Exists(fileName))
                {
                    Log.Error($"File {fileName} does not exist.");
                    return new NodeResult("Файл не существует.");
                }

                Excel.Application excelApp = new Excel.Application();
                Excel.Workbook workbook = excelApp.Workbooks.Open(fileName);
                Excel.Worksheet worksheet = workbook.Sheets[nameofSheet];

                if (worksheet == null)
                {
                    Log.Error($"Sheet {nameofSheet} does not exist in file {fileName}.");
                    return new NodeResult($"Лист {nameofSheet} не существует в файле {fileName}.");
                }

                worksheet.Range[cell].Value2 = text;

                workbook.Save();
                workbook.Close();
                excelApp.Quit();

                return new NodeResult("Данные успешно записаны в Excel файл.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error writing to Excel file.");
                return new NodeResult("Ошибка записи в Excel файл.");
            }
        }
    }
}