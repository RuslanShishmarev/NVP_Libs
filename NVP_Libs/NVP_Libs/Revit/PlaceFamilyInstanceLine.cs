using System.Collections.Generic;

using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using RevitLine = Autodesk.Revit.DB.Line;

using NVP.API.Nodes;
using Line = NVP.API.Geometry.Line;
using System.Linq;
using NVP_Libs.Revit.Services;

namespace NVP_Libs.Revit
{
    [NodeInput("линия", typeof(Line))]
    [NodeInput("типоразмер", typeof(FamilySymbol))]
    [NodeInput("уровень", typeof(Level))]
    public class PlaceFamilyInstanceLine : IRevitNode
    {
        public NodeResult Execute(IVisualViewerData context, List<NodeResult> inputs, object commandData)
        {
            var doc = (commandData as ExternalCommandData).Application.ActiveUIDocument.Document;

            var line = (Line)inputs[0].Value;
            var familySymbol = (FamilySymbol)inputs[1].Value;
            var level = (Level)inputs[2].Value;
            RevitLine revitLine = ConvertNVPToRevit.ConvertLine(line);
            var inst = new FilteredElementCollector(doc)
                .OfClass(typeof(FamilyInstance))
                .Cast<FamilyInstance>()
                .FirstOrDefault(i => i.Symbol == familySymbol);
            var structuralType = inst.StructuralType;

            using (Transaction transaction = new Transaction(doc, "Place Family Instance By Line"))
            {
                transaction.Start();
                var instance = doc.Create.NewFamilyInstance(
                    revitLine,
                    familySymbol,
                    level,
                    structuralType);
                transaction.Commit();
                return new NodeResult(instance);
            }
        }
    }
}