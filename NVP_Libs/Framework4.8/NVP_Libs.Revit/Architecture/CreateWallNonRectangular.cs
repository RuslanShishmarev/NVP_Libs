using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

using NVP.API.Nodes;

using System.Collections.Generic;
using System.Linq;

namespace NVP_Libs.Revit.Architecture
{
    [NodeInput("профиль", typeof(List<Curve>))]
    [NodeInput("тип стены", typeof(WallType))]
    [NodeInput("уровень", typeof(Level))]
    [NodeInput("несущая", typeof(bool))]
    public class CreateWallNonRectangular : INode
    {
        public NodeResult Execute(INVPData context, List<NodeResult> inputs)
        {
            var doc = (context.GetCADContext() as ExternalCommandData).Application.ActiveUIDocument.Document;

            var profile = (inputs[0].Value as IEnumerable<object>).Cast<Curve>().ToList();
            var wallType = (WallType)inputs[1].Value;
            var levelId = (inputs[2].Value as Element).Id;
            var structural = (bool)inputs[3].Value;
            var wallTypeId = wallType.Id;

            using (Transaction transaction = new Transaction(doc, "Создание стены"))
            {
                transaction.Start();
                Wall wall = Wall.Create(doc, profile, wallTypeId, levelId, structural);
                transaction.Commit();
                return new NodeResult(wall);
            }
        }
    }
}
