using NVP.API.Nodes;

using OfficeOpenXml;

using Serilog;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NVP_Libs.Common
{
    [NodeInput("Полный путь", typeof(string))]
    [NodeInput("Массив значений", typeof(object[]))]
    [NodeInput("Клетка", typeof(string))]
    [NodeInput("Имя Листа", typeof(string))]

    public class TESTSS : IRevitNode
    {
        public NodeResult Execute(IVisualViewerData context, List<NodeResult> inputs, object commandData)
        {
            try
            {
                // Получаем входные параметры
                string fileName = (string)inputs[0].Value;
                var values = (IEnumerable<object>)inputs[1].Value;
                string cell = (string)inputs[2].Value;
                string sheetName = (string)inputs[3].Value;

                // Проверяем, что fileName задан
                if (string.IsNullOrEmpty(fileName))
                {
                    Log.Error("Не указан полный путь к файлу Excel.");
                    return new NodeResult("Не указан полный путь к файлу Excel.");
                }

                // Проверяем, что values не пустой и не null
                if (values == null || !values.Any())
                {
                    Log.Error("Пустой массив значений для записи в Excel.");
                    return new NodeResult("Пустой массив значений для записи в Excel.");
                }

                // Создаем новый файл Excel, если он не существует
                FileInfo file = new FileInfo(fileName);
                ExcelPackage excelPackage;
                ExcelWorksheet worksheet;

                excelPackage = new ExcelPackage(file);

                // Проверка существования файла
                if (!file.Exists)
                {
                    worksheet = excelPackage.Workbook.Worksheets.Add(sheetName);
                }
                else
                {
                    // Если файл существует, проверяем наличие листа
                    worksheet = excelPackage.Workbook.Worksheets.FirstOrDefault(ws => ws.Name == sheetName);
                    if (worksheet == null)
                    {
                        // Создаем новый лист, если он не найден
                        worksheet = excelPackage.Workbook.Worksheets.Add(sheetName);
                    }
                }

                // Определяем начальные индексы строки и столбца
                int rowIndex = int.Parse(cell.Substring(1));
                int colIndex = cell[0] - 'A' + 1;

                // Записываем значения в ячейки Excel
                int startRow = rowIndex;
                foreach (var value in values)
                {
                    string[] parts = value.ToString().Split(';');
                    for (int j = 0; j < parts.Length; j++)
                    {
                        worksheet.Cells[startRow, colIndex + j].Value = parts[j];
                    }
                    startRow++;
                }

                // Сохраняем изменения в файле Excel
                excelPackage.Save();

                excelPackage.Dispose();

                Log.Information($"Значения успешно записаны в файл Excel: {fileName}, Лист: {sheetName}, Клетка: {cell}");
                return new NodeResult($"Значения успешно записаны в файл Excel: {fileName}, Лист: {sheetName}, Клетка: {cell}");
            }
            catch (Exception ex)
            {
                Log.Error($"Ошибка при записи в файл Excel: {ex.Message}");
                return new NodeResult($"Ошибка при записи в файл Excel: {ex.Message}");
            }
        }
    }
}

