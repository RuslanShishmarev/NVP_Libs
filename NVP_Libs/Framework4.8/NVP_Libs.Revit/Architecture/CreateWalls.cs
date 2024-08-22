using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

using NVP.API.Nodes;

using System.Collections.Generic;

namespace NVP_Libs.Revit.Architecture
{
    [NodeInput("тип стены", typeof(WallType))]
    [NodeInput("линия построения", typeof(Curve))]
    [NodeInput("уровень", typeof(Level))]
    [NodeInput("высота", typeof(double))]
    [NodeInput("смещение", typeof(double))]
    [NodeInput("внешняя сторона", typeof(bool))]
    [NodeInput("несущая", typeof(bool))]
    public class CreateWalls : INode
    {
        public NodeResult Execute(INVPData context, List<NodeResult> inputs)
        {
            var doc = (context.GetCADContext() as ExternalCommandData).Application.ActiveUIDocument.Document;

            var wallType = (WallType)inputs[0].Value;
            var curve = (Curve)inputs[1].Value;
            var level = (Level)inputs[2].Value;
            var height = (double)inputs[3].Value;
            var offset = (double)inputs[4].Value;
            var flip = (bool)inputs[5].Value;
            var structural = (bool)inputs[6].Value;
            var wallTypeId = wallType.Id;
            var levelId = level.Id;

            using (Transaction transaction = new Transaction(doc, "Создание стены"))
            {
                transaction.Start();
                Wall wall = Wall.Create(doc, curve, wallTypeId, levelId, height, offset, flip, structural);
                transaction.Commit();
                return new NodeResult(wall);
            }
        }
    }
}
