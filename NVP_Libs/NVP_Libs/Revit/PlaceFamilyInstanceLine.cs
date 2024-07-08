using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

using NVP.API.Nodes;
using NVP_Libs.Revit.Services;

using System.Collections.Generic;
using System.Linq;

using Line = NVP.API.Geometry.Line;
using RevitLine = Autodesk.Revit.DB.Line;

namespace NVP_Libs.Revit
{
    [NodeInput("линия", typeof(Line))]
    [NodeInput("типоразмер", typeof(FamilySymbol))]
    [NodeInput("уровень", typeof(Level))]
    public class PlaceFamilyInstanceLine : INode
    {
        public NodeResult Execute(INVPData context, List<NodeResult> inputs)
        {
            var doc = (context.GetCADContext() as ExternalCommandData).Application.ActiveUIDocument.Document;

            var line = (Line)inputs[0].Value;
            var familySymbol = (FamilySymbol)inputs[1].Value;
            var level = (Level)inputs[2].Value;
            RevitLine revitLine = ConvertNVPToRevit.ConvertLine(line);
            var firstInstance = new FilteredElementCollector(doc)
                .OfClass(typeof(FamilyInstance))
                .Cast<FamilyInstance>()
                .FirstOrDefault(i => i.Name.Equals(familySymbol.Name));
            var structuralType = firstInstance.StructuralType;

            using (Transaction transaction = new Transaction(doc, "Размещение экземпляра семейства по линии"))
            {
                transaction.Start();
                var instance = doc.Create.NewFamilyInstance(revitLine, familySymbol, level, structuralType);
                transaction.Commit();
                return new NodeResult(instance);
            }
        }
    }
}