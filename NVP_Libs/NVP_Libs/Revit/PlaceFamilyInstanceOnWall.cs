using System.Collections.Generic;

using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using RevitXYZ = Autodesk.Revit.DB.XYZ;

using NVP.API.Nodes;
using XYZ = NVP.API.Geometry.XYZ;
using NVP_Libs.Revit.Services;

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
            var family = (FamilySymbol)inputs[1].Value;
            var point = (XYZ)inputs[2].Value;
            RevitXYZ revitPoint = ConvertNVPToRevit.ConvertXYZ(point);

            using (Transaction transaction = new Transaction(doc, "Place Family Instance On Wall"))
            {
                transaction.Start();
                Curve elementCurve = (element.Location as LocationCurve).Curve;
                var revitPointProjection = elementCurve.Project(revitPoint).XYZPoint;

                var instance = doc.Create.NewFamilyInstance(
                    revitPointProjection,
                    family,
                    element,
                    Autodesk.Revit.DB.Structure.StructuralType.NonStructural);
                transaction.Commit();
                return new NodeResult(instance);
            }
        }
    }
}