using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

using NVP.API.Nodes;

using System.Collections.Generic;
using System.Linq;

namespace NVP_Libs.Revit.Architecture
{
    [NodeInput("профиль", typeof(List<Curve>))]
    [NodeInput("несущая", typeof(bool))]
    public class CreateDefaultWallNonRectangular : INode
    {
        public NodeResult Execute(INVPData context, List<NodeResult> inputs)
        {
            var doc = (context.GetCADContext() as ExternalCommandData).Application.ActiveUIDocument.Document;

            var profile = (inputs[0].Value as IEnumerable<object>).Cast<Curve>().ToList();
            var structural = (bool)inputs[1].Value;
            
            using (Transaction transaction = new Transaction(doc, "Создание стены"))
            {
                transaction.Start();
                Wall wall = Wall.Create(doc, profile, structural);
                transaction.Commit();
                return new NodeResult(wall);
            }
        }
    }
}
