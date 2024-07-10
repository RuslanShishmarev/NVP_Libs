using NVP.API.Nodes;

using System.Collections.Generic;
using System.IO;

namespace NVP_Libs.Common
{
    [NodeInput("полный путь до файла", typeof(string))]
    public class TextWatch: INode
    {
        public NodeResult Execute(INVPData context, List<NodeResult> inputs)
        {
            string link = (string)inputs[0].Value;

            string fileContent = File.ReadAllText(link);

            return new NodeResult(fileContent);
        }
    }
}


