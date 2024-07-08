using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

using NVP.API.Nodes;
using NVP_Libs.Revit.Services;

using System.Collections.Generic;
using System.Linq;

using RevitXYZ = Autodesk.Revit.DB.XYZ;
using XYZ = NVP.API.Geometry.XYZ;

namespace NVP_Libs.Revit
{
    [NodeInput("координата", typeof(XYZ))]
    [NodeInput("типоразмер", typeof(FamilySymbol))]
    [NodeInput("уровень", typeof(Level))]
    public class PlaceFamilyInstanceXYZ : IRevitNode
    {
        public NodeResult Execute(IVisualViewerData context, List<NodeResult> inputs, object commandData)
        {         
            var doc = (commandData as ExternalCommandData).Application.ActiveUIDocument.Document;

            var point = (XYZ)inputs[0].Value;
            var familySymbol = (FamilySymbol)inputs[1].Value;
            var level = (Level)inputs[2].Value;
            RevitXYZ revitPoint = ConvertNVPToRevit.ConvertXYZ(point);
            var firstInstance = new FilteredElementCollector(doc)
                .OfClass(typeof(FamilyInstance))
                .Cast<FamilyInstance>()
                .FirstOrDefault(i => i.Name.Equals(familySymbol.Name));
            var structuralType = firstInstance.StructuralType;

            using (Transaction transaction = new Transaction(doc, "Размещение экземпляра семейства по точке"))
            {
                transaction.Start();
                var instance = doc.Create.NewFamilyInstance(revitPoint, familySymbol, level, structuralType);
                transaction.Commit();
                return new NodeResult(instance);
            }
        }
    }
}
