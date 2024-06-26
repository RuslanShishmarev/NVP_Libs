using NVP.API.Nodes;
using System;
using System.Collections.Generic;
using System.IO;
using OfficeOpenXml;
using System.Linq;
using Serilog; 

namespace NVP_Libs
{
    [NodeInput("FileName", typeof(string))]
    [NodeInput("Text", typeof(string))]
    [NodeInput("Cell", typeof(string))]
    [NodeInput("NameOfSheet", typeof(string))]
    public class ExcelZapis : IRevitNode
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
                using (var package = new ExcelPackage(new FileInfo(fileName)))
                {
                    var worksheet = package.Workbook.Worksheets[nameofSheet];
                    if (worksheet == null)
                    {
                        Log.Error($"Sheet {nameofSheet} does not exist in file {fileName}.");
                        return new NodeResult($"Лист {nameofSheet} не существует в файле {fileName}.");
                    }
                    worksheet.Cells[cell].Value = text;

                    package.Save();
                }
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

