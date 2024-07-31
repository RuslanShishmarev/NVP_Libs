using Multicad.DatabaseServices.StandardObjects;
using Multicad.Geometry;

using NVP.API.Nodes;

using System.Collections.Generic;
using System.Linq;


using NVPLine = NVP.API.Geometry.Line;

namespace NVP_Libs.Nanocad
{
    [NodeInput("линия", typeof(NVPLine))]
    public class CreateLine : INode
    {
        public NodeResult Execute(INVPData context, List<NodeResult> inputs)
        {
            var lineFromInput = inputs.FirstOrDefault().Value as NVPLine;

            DbLine line = new DbLine();
            line.StartPoint = new Point3d(lineFromInput.Start.X, lineFromInput.Start.Y, lineFromInput.Start.Z);
            line.EndPoint = new Point3d(lineFromInput.End.X, lineFromInput.End.Y, lineFromInput.End.Z);
            line.DbEntity.AddToCurrentDocument();

            return new NodeResult(line);
        }
    }
}
