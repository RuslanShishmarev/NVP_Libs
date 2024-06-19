using NVP.API.Nodes;
using System.Collections.Generic;

namespace NVP_Libs
{
    [NodeInput("номер 1", typeof(double))]
    [NodeInput("номер 2", typeof(double))]
    public class ExampleRevit : IRevitNode
    {
        public NodeResult Execute(IVisualViewerData context, List<NodeResult> inputs, object commandData)
        {
            var num1 = (double)inputs[0].Value;
            var num2 = (double)inputs[1].Value;

            return new NodeResult(num1 + num2);
        }
    }
}
