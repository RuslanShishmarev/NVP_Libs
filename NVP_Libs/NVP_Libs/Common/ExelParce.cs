using NVP.API.Nodes;

using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace NVP_Libs.Common
{
    [NodeInput("полный путь до файла", typeof(string))]

    public class ExcelParce : INode
    {
        public NodeResult Execute(INVPData context, List<NodeResult> inputs)
        {
            string link = (string)inputs[0].Value;
            string fileContent = File.ReadAllText(link);
            string[] rows = fileContent.Split('\n');
            List<List<string>> data = new List<List<string>>();

            foreach (string row in rows)
            {
                string[] cells = Regex.Split(row, @"\t");
                data.Add(new List<string>(cells));
            }
            return new NodeResult(data);
        }
    }
}