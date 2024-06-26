using NVP.API.Nodes;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;

namespace NVP_Libs
{
    [NodeInput("link", typeof(string))]
    public class ExcelParce : IRevitNode
    {
        public NodeResult Execute(IVisualViewerData context, List<NodeResult> inputs, object commandData)
        {
            var link = (string)inputs[0].Value;
            FileInfo fileInfo = new FileInfo(link);

            using (ExcelPackage package = new ExcelPackage(fileInfo))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                int rowCount = worksheet.Dimension.Rows;
                int colCount = worksheet.Dimension.Columns;
                string[][] data = new string[rowCount][];

                for (int i = 1; i <= rowCount; i++)
                {
                    data[i - 1] = new string[colCount];
                    for (int j = 1; j <= colCount; j++)
                    {
                        data[i - 1][j - 1] = worksheet.Cells[i, j].Value?.ToString();
                    }
                }

                return new NodeResult(data);
            }
        }
    }
}
