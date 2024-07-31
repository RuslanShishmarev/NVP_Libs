using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;

using NVP.API.Nodes;
using NVP_Libs.Revit.Services;

using System;
using System.Collections.Generic;

using RevitXYZ = Autodesk.Revit.DB.XYZ;
using XYZ = NVP.API.Geometry.XYZ;

namespace NVP_Libs.Revit.Common
{
    [NodeInput("элемент", typeof(Wall))]
    [NodeInput("типоразмер", typeof(FamilySymbol))]
    [NodeInput("координата", typeof(XYZ))]
    [NodeInput("структурный тип", typeof(string))]
    public class PlaceFamilyInstanceOnWall : INode
    {
        public NodeResult Execute(INVPData context, List<NodeResult> inputs)
        {
            var doc = (context.GetCADContext() as ExternalCommandData).Application.ActiveUIDocument.Document;

            var element = (Wall)inputs[0].Value;
            var familySymbol = (FamilySymbol)inputs[1].Value;
            var point = (XYZ)inputs[2].Value;
            var name = (string)inputs[3].Value;

            RevitXYZ revitPoint = point.ToRevit();
            var structuralType = (StructuralType) Enum.Parse(typeof(StructuralType), name);

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