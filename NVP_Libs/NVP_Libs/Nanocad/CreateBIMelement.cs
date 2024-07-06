using NVP.API.Nodes;
using System.Collections.Generic;

namespace NVP_Libs
{

    [NodeInput("Ширина", typeof(double))]
    [NodeInput("Высота", typeof(double))]
    [NodeInput("Длина", typeof(double))]
    public class CreateBIMElement : INode
    {

        public NodeResult Execute(INVPData context, List<NodeResult> inputs)
        {
            var width = (double)inputs[0].Value;
            var height = (double)inputs[1].Value;
            var length = (double)inputs[2].Value;

  
            var bimElement = CreateColumn(width, height, length);

            return new NodeResult(bimElement);
        }


        private object CreateColumn(double width, double height, double length)
        {

            var column = new { Width = width, Height = height, Length = length, Type = "Column" };

            return column;
        }
    }
}