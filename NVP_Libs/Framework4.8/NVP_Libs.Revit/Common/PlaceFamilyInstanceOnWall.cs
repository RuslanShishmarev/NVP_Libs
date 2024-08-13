using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;

using NVP.API.Nodes;

using System;
using System.Collections.Generic;

using RevitXYZ = Autodesk.Revit.DB.XYZ;

namespace NVP_Libs.Revit.Common
{
    [NodeInput("элемент", typeof(Wall))]
    [NodeInput("типоразмер", typeof(FamilySymbol))]
    [NodeInput("координата", typeof(RevitXYZ))]
    [NodeInput("структурный тип", typeof(string))]
    public class PlaceFamilyInstanceOnWall : INode
    {
        public NodeResult Execute(INVPData context, List<NodeResult> inputs)
        {
            var doc = (context.GetCADContext() as ExternalCommandData).Application.ActiveUIDocument.Document;

            var element = (Wall)inputs[0].Value;
            var familySymbol = (FamilySymbol)inputs[1].Value;
            var point = (RevitXYZ)inputs[2].Value;
            var name = (string)inputs[3].Value;

            var structuralType = (StructuralType) Enum.Parse(typeof(StructuralType), name);

            using (Transaction transaction = new Transaction(doc, "Размещение экземпляра семейства на стене"))
            {
                transaction.Start();
                Curve elementCurve = (element.Location as LocationCurve).Curve;
                var pointProjection = elementCurve.Project(point).XYZPoint;

                var instance = doc.Create.NewFamilyInstance(pointProjection, familySymbol, element, structuralType);
                transaction.Commit();
                return new NodeResult(instance);
            }
        }
    }
}