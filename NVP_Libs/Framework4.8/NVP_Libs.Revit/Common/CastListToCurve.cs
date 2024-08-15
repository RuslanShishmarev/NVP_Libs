using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

using NVP.API.Nodes;

using System;
using System.Collections.Generic;
using System.Linq;

namespace NVP_Libs.Revit.Common
{
    [NodeInput("список", typeof(List<object>))]
    internal class CastListToCurve : INode
    {
        public NodeResult Execute(INVPData context, List<NodeResult> inputs)
        {
            var list = (inputs[0].Value as IEnumerable<object>).Cast<Curve>().ToList();
            return new NodeResult(list);
        }
    }
}
