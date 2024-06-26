using NVP.API.Nodes;
using System;
using System.Collections.Generic;
using System.IO;

namespace NVP_Libs
{
    [NodeInput("Link", typeof(string))]


    public class Textview: IRevitNode
    {
        public NodeResult Execute(IVisualViewerData context, List<NodeResult> inputs, object commandData)
        {
            string link = (string)inputs[0].Value;



            string fileContent = File.ReadAllText(link);

            return new NodeResult(fileContent);
        }
    }
}


