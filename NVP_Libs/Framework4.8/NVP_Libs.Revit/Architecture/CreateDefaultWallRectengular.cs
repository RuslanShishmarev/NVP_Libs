using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

using NVP.API.Nodes;

using System.Collections.Generic;

namespace NVP_Libs.Revit.Architecture
{
    [NodeInput("линия построения", typeof(Curve))]
    [NodeInput("уровень", typeof(Level))]
    [NodeInput("несущая", typeof(bool))]
    public class CreateDefaultWallRectengular : INode
    {
        public NodeResult Execute(INVPData context, List<NodeResult> inputs)
        {
            var doc = (context.GetCADContext() as ExternalCommandData).Application.ActiveUIDocument.Document;

            var curve = (Curve)inputs[0].Value;
            var levelId = (inputs[1].Value as Element).Id;
            var structural = (bool)inputs[2].Value;

            using (Transaction transaction = new Transaction(doc, "Создание стены"))
            {
                transaction.Start();
                Wall wall = Wall.Create(doc, curve, levelId, structural);
                transaction.Commit();
                return new NodeResult(wall);
            }

        }
    }
}
