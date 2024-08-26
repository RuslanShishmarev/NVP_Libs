using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

using NVP.API.Nodes;

using System.Collections.Generic;
using System.Linq;

using static NVP_Libs.Revit.Services.CurveService;

using RevitLine = Autodesk.Revit.DB.Line;

namespace NVP_Libs.Revit.Architecture
{
    [NodeInput("профиль", typeof(List<Curve>))]
    [NodeInput("тип перекрытия", typeof(FloorType))]
    [NodeInput("уровень", typeof(Level))]
    [NodeInput("несущее", typeof(bool))]
    [NodeInput("линия наклона", typeof(RevitLine))]
    [NodeInput("угол наклона", typeof(double))]
    public class CreateFloor : INode
    {
        public NodeResult Execute(INVPData context, List<NodeResult> inputs)
        {
            var doc = (context.GetCADContext() as ExternalCommandData).Application.ActiveUIDocument.Document;

            var curves = (inputs[0].Value as IEnumerable<object>).Cast<Curve>().ToList();
            var floorType = (FloorType)inputs[1].Value;
            var floorTypeId = floorType.Id;
            var levelId = (inputs[2].Value as Element).Id;
            var structural = (bool)inputs[3].Value;
            var slopeArrow = (RevitLine)inputs[4].Value;
            var slope = (double)inputs[5].Value;
            List<CurveLoop> profile = new List<CurveLoop>();
            profile = CreateCurveLoops(curves);

            using (Transaction transaction = new Transaction(doc, "Создание перекрытия"))
            {
                transaction.Start();
                Floor floor = Floor.Create(doc, profile, floorTypeId, levelId, structural, slopeArrow, slope);
                transaction.Commit();
                return new NodeResult(floor);
            }
        }
    }
}
