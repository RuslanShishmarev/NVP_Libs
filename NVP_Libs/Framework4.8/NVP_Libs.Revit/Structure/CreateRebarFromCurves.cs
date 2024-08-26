using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;

using NVP.API.Nodes;

using System.Collections.Generic;
using System.Linq;

using RevitXYZ = Autodesk.Revit.DB.XYZ;

namespace NVP_Libs.Revit.Structure
{
    [NodeInput("профиль", typeof(List<Curve>))]
    [NodeInput("тип арматуры", typeof(RebarBarType))]
    [NodeInput("стандартный стержень", typeof(bool))]
    [NodeInput("элемент", typeof(Element))]
    [NodeInput("нормаль", typeof(RevitXYZ))]
    [NodeInput("тип отгиба 1", typeof(RebarHookType))]
    [NodeInput("лево/право", typeof(bool))]
    [NodeInput("тип отгиба 2", typeof(RebarHookType))]
    [NodeInput("лево/право", typeof(bool))]

    public class CreateRebarFromCurves : INode
    {
        public NodeResult Execute(INVPData context, List<NodeResult> inputs)
        {
            var doc = (context.GetCADContext() as ExternalCommandData).Application.ActiveUIDocument.Document;

            var curves = (inputs[0].Value as IEnumerable<object>).Cast<Curve>().ToList();
            var rebarBarType = (RebarBarType)inputs[1].Value;
            var rebarStyle = (bool)inputs[2].Value;
            RebarStyle style = RebarStyle.Standard;
            if (!rebarStyle)
            {
                style = RebarStyle.StirrupTie;
            }
            var host = (Element)inputs[3].Value;
            var norm = (RevitXYZ)inputs[4].Value;
            var startHook = (RebarHookType)inputs[5].Value;
            var endHook = (RebarHookType)inputs[7].Value;
            var startOrientBool = (bool)inputs[6].Value;
            var endOrientBool = (bool)inputs[8].Value;
            RebarHookOrientation startOrient = RebarHookOrientation.Right;
            RebarHookOrientation endOrient = RebarHookOrientation.Right;
            if (startOrientBool)
            {
                startOrient = RebarHookOrientation.Left;
            }
            if (endOrientBool)
            {
                endOrient = RebarHookOrientation.Left;
            }

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
