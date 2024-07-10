using NVP.API.Nodes;
using System;
using System.Collections.Generic;
using System.IO;

namespace NVP_Libs.Common
{
    [NodeInput("полный путь до файла", typeof(string))]
    [NodeInput("разделитель", typeof(char))]
    public class ExcelParceCSV : INode
    {
        public NodeResult Execute(INVPData context, List<NodeResult> inputs)
        {
            char defaultSplitter = ';';
            string link = (string)inputs[0].Value;
            char splitter = defaultSplitter;
            if (inputs[1].Value != null)
            {
                splitter =Convert.ToChar(inputs[1].Value);
            }

            string fileContent = File.ReadAllText(link);
            string[] rows = fileContent.Split('\n');
            var data = new List<string[]>();

            foreach (string row in rows)
            {
                string[] cells = row.Split(splitter);
                data.Add(cells);
            }
            return new NodeResult(data);
        }
    }
}