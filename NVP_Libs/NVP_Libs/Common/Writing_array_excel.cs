﻿using NVP.API.Nodes;

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace NVP_Libs.Common
{
    [NodeInput("полный путь", typeof(string))]
    [NodeInput("массив значений", typeof(object[]))]
    [NodeInput("клетка", typeof(string))]
    [NodeInput("имя листа", typeof(string))]
    public class Writing_array_excel : IRevitNode
    {
        public NodeResult Execute(IVisualViewerData context, List<NodeResult> inputs, object commandData)
        {
            // Получаем входные параметры
            string fileName = (string)inputs[0].Value;
            var values = (IEnumerable<object>)inputs[1].Value;
            string cell = (string)inputs[2].Value;
            string nameofSheet = (string)inputs[3].Value;

            try
            {
                // Проверяем, существует ли файл
                if (!File.Exists(fileName))
                {
                    return new NodeResult("Файл не существует.");
                }

                // Создаем экземпляр приложения Excel
                Type excelType = Type.GetTypeFromCLSID(new Guid("00024500-0000-0000-C000-000000000046"));
                dynamic excel = Activator.CreateInstance(excelType);
                dynamic workbook = null;
                dynamic worksheet = null;
                try
                {
                    workbook = excel.Workbooks.Open(fileName);
                    dynamic worksheets = workbook.Worksheets;

                    // Check if the worksheet already exists
                    foreach (dynamic ws in worksheets)
                    {
                        if (ws.Name == nameofSheet)
                        {
                            worksheet = ws;
                            break;
                        }
                    }
                    if (worksheet == null)
                    {
                        worksheet = worksheets.Add();
                        worksheet.Name = nameofSheet;
                    }

                    // Парсим координаты клетки
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
                    workbook.Save();
                }
                finally
                {
                    if (workbook != null)
                    {
                        workbook.Close(false);
                        Marshal.ReleaseComObject(workbook);
                    }
                    if (excel != null)
                    {
                        excel.Quit();
                        Marshal.ReleaseComObject(excel);
                        GC.Collect();
                        GC.WaitForPendingFinalizers();
                    }
                }

                return new NodeResult("Данные успешно записаны в Excel файл.");
            }
            catch (COMException ex)
            {

                return new NodeResult("Ошибка сохранения Excel файла.1" + "Error code: " + ex.ErrorCode + "Error message: " + ex.Message);
            }
            catch (IOException ex)
            {
                return new NodeResult("Ошибка записи в Excel файла.2");
            }
            catch (Exception ex)
            {
                return new NodeResult("Ошибка сохранения Excel файла.3" + "Error message: " + ex.Message);
            }
        }
    }
}
