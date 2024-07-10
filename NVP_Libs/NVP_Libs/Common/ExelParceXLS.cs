using NVP.API.Nodes;

using OfficeOpenXml;

using System.Collections.Generic;
using System.IO;

namespace NVP_Libs.Common
{
    [NodeInput("полный путь до файла", typeof(string))]
    public class ExcelParceXLS : INode
    {
        public NodeResult Execute(INVPData context, List<NodeResult> inputs)
        {
            string link = (string)inputs[0].Value;
            var allData = new Dictionary<string, List<object[]>>();

            using (FileStream stream = File.Open(link, FileMode.Open))
            {
                using (ExcelPackage package = new ExcelPackage(stream))
                {
                    const int startIndex = 1;
                    foreach (ExcelWorksheet workSheet in package.Workbook.Worksheets)
                    {
                        var data = new List<object[]>();

                        for (int row = startIndex; row <= workSheet.Dimension.End.Row; row++)
                        {
                            object[] rowData = new object[workSheet.Dimension.End.Column];
                            for (int col = startIndex; col <= workSheet.Dimension.End.Column; col++)
                            {
                                rowData[col - startIndex] = workSheet.Cells[row, col].Value;
                            }
                            data.Add(rowData);
                        }
                        allData.Add(workSheet.Name, data);
                    }
                    return new NodeResult(allData);
                }
            }
        }
    }
}