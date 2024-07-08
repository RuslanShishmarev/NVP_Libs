using NVP.API.Nodes;

using System.Collections.Generic;
using System.IO;
using OfficeOpenXml;

namespace NVP_Libs.Common
{
    [NodeInput("полный путь до файла", typeof(string))]
    [NodeInput("лист", typeof(string))]
    public class ExcelParceXLS : INode
    {
        public NodeResult Execute(INVPData context, List<NodeResult> inputs)
        {
            string link = (string)inputs[0].Value;
            string sheet = (string)inputs[1].Value;

            using (FileStream stream = File.Open(link, FileMode.Open))
            {
                using (ExcelPackage package = new ExcelPackage(stream))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[sheet];
                    List<string> data = new List<string>();

                    for (int row = 1; row <= worksheet.Dimension.End.Row; row++)
                    {
                        string rowData = "";
                        for (int col = 1; col <= worksheet.Dimension.End.Column; col++)
                        {
                            rowData += worksheet.Cells[row, col].Value?.ToString();
                            if (col < worksheet.Dimension.End.Column)
                            {
                                rowData += ";";
                            }
                        }
                        data.Add(rowData);
                    }
                    return new NodeResult(data);
                }
            }
        }
    }
}