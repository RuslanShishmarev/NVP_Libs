using Multicad.DatabaseServices.StandardObjects;
using Multicad.Geometry;

using NVP.API.Nodes;

using System.Collections.Generic;
using System.Linq;


using NVPLine = NVP.API.Geometry.Line;

namespace NVP_Libs.Nanocad
{
    [NodeInput("линии", typeof(List<NVPLine>))]
    public class CreateLines : INode
    {
        public NodeResult Execute(INVPData context, List<NodeResult> inputs)
        {
            var linesFromInput = (inputs.FirstOrDefault().Value as IEnumerable<object>).Cast<NVPLine>();

            var result = new List<DbLine>();
            foreach (var lineNVP in linesFromInput)
            {
                DbLine line = new DbLine();
                line.StartPoint = new Point3d(lineNVP.Start.X, lineNVP.Start.Y, lineNVP.Start.Z);
                line.EndPoint = new Point3d(lineNVP.End.X, lineNVP.End.Y, lineNVP.End.Z);
                line.DbEntity.AddToCurrentDocument();
                result.Add(line);
            }

            return new NodeResult(result);
        }
    }
}
