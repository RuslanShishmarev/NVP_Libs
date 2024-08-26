using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;

using NVP.API.Nodes;

using System.Collections.Generic;
using System.Linq;

using static NVP_Libs.Revit.Services.CurveService;

namespace NVP_Libs.Revit.Structure
{
    [NodeInput("профиль", typeof(List<Curve>))]
    [NodeInput("тип арматуры", typeof(RebarBarType))]
    [NodeInput("элемент", typeof(Element))]
    public class CreateRebarFreeFormCurveLoop : INode
    {
        public NodeResult Execute(INVPData context, List<NodeResult> inputs)
        {
            var doc = (context.GetCADContext() as ExternalCommandData).Application.ActiveUIDocument.Document;

            var curves = (inputs[0].Value as IEnumerable<object>).Cast<Curve>().ToList();
            var rebarBarType = (RebarBarType)inputs[1].Value;
            var host = (Element)inputs[2].Value;
            var validationResult = new RebarFreeFormValidationResult();
            List<CurveLoop> curveLoops = new List<CurveLoop>();
            curveLoops = CreateCurveLoops(curves);

            using (Transaction transaction = new Transaction(doc, "Создание арматуры свободной формы"))
            {
                transaction.Start();
                Rebar rebar = Rebar.CreateFreeForm(doc, rebarBarType, host, curveLoops, out validationResult);
                transaction.Commit();
                return new NodeResult(rebar);
            }
        }
    }
}
