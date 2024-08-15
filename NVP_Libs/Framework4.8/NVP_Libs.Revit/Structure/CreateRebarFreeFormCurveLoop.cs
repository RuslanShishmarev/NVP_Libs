using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;

using NVP.API.Nodes;

using System.Collections.Generic;
using System.Linq;


namespace NVP_Libs.Revit.Structure
{
    [NodeInput("профили", typeof(List<CurveLoop>))]
    [NodeInput("тип арматуры", typeof(string))]
    [NodeInput("элемент", typeof(Element))]
    internal class CreateRebarFreeFormCurveLoop : INode
    {
        public NodeResult Execute(INVPData context, List<NodeResult> inputs)
        {
            var doc = (context.GetCADContext() as ExternalCommandData).Application.ActiveUIDocument.Document;

            var curves = (inputs[0].Value as IEnumerable<object>).Cast<CurveLoop>().ToList();
            var rebarBarTypeName = (string)inputs[1].Value;
            var rebarBarType = new FilteredElementCollector(doc)
                .OfClass(typeof(RebarBarType))
                .Cast<RebarBarType>()
                .FirstOrDefault(bt => bt.Name == rebarBarTypeName);
            var host = (Element)inputs[2].Value;
            var validationResult = new RebarFreeFormValidationResult();

            using (Transaction transaction = new Transaction(doc, "Создание арматуры свободной формы"))
            {
                transaction.Start();
                Rebar rebar = Rebar.CreateFreeForm(doc, rebarBarType, host, curves, out validationResult);
                transaction.Commit();
                return new NodeResult(rebar);
            }
        }
    }
}
