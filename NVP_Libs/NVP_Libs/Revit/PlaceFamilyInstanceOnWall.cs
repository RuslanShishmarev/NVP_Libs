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
    [NodeInput("элемент", typeof(Element))]
    [NodeInput("типоразмер", typeof(FamilySymbol))]
    [NodeInput("координата", typeof(XYZ))]
    public class PlaceFamilyInstanceOnWall : IRevitNode
    {
        public NodeResult Execute(IVisualViewerData context, List<NodeResult> inputs, object commandData)
        {
            var uiDoc = (commandData as ExternalCommandData).Application.ActiveUIDocument;
            var doc = uiDoc.Document;

            var element = (Element)inputs[0].Value;
            var familySymbol = (FamilySymbol)inputs[1].Value;
            var point = (XYZ)inputs[2].Value;
            RevitXYZ revitPoint = ConvertNVPToRevit.ConvertXYZ(point);
            var firstInstance = new FilteredElementCollector(doc)
                .OfClass(typeof(FamilyInstance))
                .Cast<FamilyInstance>()
                .FirstOrDefault(i => i.Name.Equals(familySymbol.Name));
            var structuralType = firstInstance.StructuralType;

            using (Transaction transaction = new Transaction(doc, "Размещение экземпляра семейства на стене"))
            {
                transaction.Start();
                Curve elementCurve = (element.Location as LocationCurve).Curve;
                var revitPointProjection = elementCurve.Project(revitPoint).XYZPoint;

                var instance = doc.Create.NewFamilyInstance(revitPointProjection, familySymbol, element, structuralType);
                transaction.Commit();
                return new NodeResult(instance);
            }
        }
    }
}