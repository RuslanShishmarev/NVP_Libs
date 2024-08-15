using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;

using NVP.API.Nodes;

using System.Collections.Generic;
using System.Linq;

using RevitXYZ = Autodesk.Revit.DB.XYZ;

namespace NVP_Libs.Revit.Structure
{
    [NodeInput("профили", typeof(List<Curve>))]
    [NodeInput("тип арматуры", typeof(string))]
    [NodeInput("стандартный стержень", typeof(bool))]
    [NodeInput("элемент", typeof(Element))]
    [NodeInput("нормаль", typeof(RevitXYZ))]
    [NodeInput("отгиб 1", typeof(List<object>))]
    [NodeInput("отгиб 2", typeof(List<object>))]

    internal class CreateRebarFromCurves : INode
    {
        public NodeResult Execute(INVPData context, List<NodeResult> inputs)
        {
            var doc = (context.GetCADContext() as ExternalCommandData).Application.ActiveUIDocument.Document;

            var curves = (inputs[0].Value as IEnumerable<object>).Cast<Curve>().ToList();
            var rebarBarTypeName = (string)inputs[1].Value;
            var rebarBarType = new FilteredElementCollector(doc)
                .OfClass(typeof(RebarBarType))
                .Cast<RebarBarType>()
                .FirstOrDefault(bt => bt.Name == rebarBarTypeName);
            var rebarStyle = (bool)inputs[2].Value;
            RebarStyle style = RebarStyle.Standard;
            if (!rebarStyle)
            {
                style = RebarStyle.StirrupTie;
            }
            var host = (Element)inputs[3].Value;
            var norm = (RevitXYZ)inputs[4].Value;
            var list1 = (inputs[5].Value as IEnumerable<object>).Cast<object>().ToList();
            var list2 = (inputs[6].Value as IEnumerable<object>).Cast<object>().ToList();
            RebarHookType startHook = list1[0] as RebarHookType;
            RebarHookType endHook = list2[0] as RebarHookType;
            RebarHookOrientation startOrient = (RebarHookOrientation)list1[1];
            RebarHookOrientation endOrient = (RebarHookOrientation)list2[1];

            using (Transaction transaction = new Transaction(doc, "Создание арматуры по кривым"))
            {
                transaction.Start();
                Rebar rebar = Rebar.CreateFromCurves(doc, style, rebarBarType, startHook, endHook, host, norm, curves, startOrient, endOrient, true, true);
                transaction.Commit();
                return new NodeResult(rebar);
            }
        }
    }
}
