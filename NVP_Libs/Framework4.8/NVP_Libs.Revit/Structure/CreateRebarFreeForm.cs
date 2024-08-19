using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;

using NVP.API.Nodes;

using System.Collections.Generic;
using System.Linq;


namespace NVP_Libs.Revit.Structure
{
    [NodeInput("профиль", typeof(List<List<Curve>>))]
    [NodeInput("тип арматуры", typeof(RebarBarType))]
    [NodeInput("элемент", typeof(Element))]
    public class CreateRebarFreeForm : INode
    {
        public NodeResult Execute(INVPData context, List<NodeResult> inputs)
        {
            var doc = (context.GetCADContext() as ExternalCommandData).Application.ActiveUIDocument.Document;

            var curves = (inputs[0].Value as IEnumerable<object>).Cast<List<object>>().ToList();
            var profile = new List<IList<Curve>>();
            for (int i = 0; i < curves.Count; i++)
            {
                var list = curves[i].Cast<Curve>().ToList();
                profile.Add(list);
            }
            var rebarBarType = (RebarBarType)inputs[1].Value;
            var host = (Element)inputs[2].Value;
            var validationResult = new RebarFreeFormValidationResult();

            using (Transaction transaction = new Transaction(doc, "Создание арматуры свободной формы"))
            {
                transaction.Start();
                Rebar rebar = Rebar.CreateFreeForm(doc, rebarBarType, host, profile, out validationResult);
                transaction.Commit();
                return new NodeResult(rebar);
            }
        }
    }
}
