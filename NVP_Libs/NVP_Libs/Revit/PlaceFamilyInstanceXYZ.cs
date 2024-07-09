using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;

using NVP.API.Nodes;
using NVP_Libs.Revit.Services;

using System;
using System.Collections.Generic;

using RevitXYZ = Autodesk.Revit.DB.XYZ;
using XYZ = NVP.API.Geometry.XYZ;

namespace NVP_Libs.Revit
{
    [NodeInput("координата", typeof(XYZ))]
    [NodeInput("типоразмер", typeof(FamilySymbol))]
    [NodeInput("уровень", typeof(Level))]
    [NodeInput("структурный тип", typeof(string))]
    public class PlaceFamilyInstanceXYZ : INode
    {
        public NodeResult Execute(INVPData context, List<NodeResult> inputs)
        {         
            var doc = (context.GetCADContext() as ExternalCommandData).Application.ActiveUIDocument.Document;

            var point = (XYZ)inputs[0].Value;
            var familySymbol = (FamilySymbol)inputs[1].Value;
            var level = (Level)inputs[2].Value;
            var name = (string)inputs[3].Value;

            RevitXYZ revitPoint = point.ToRevit();
            var structuralType = (StructuralType) Enum.Parse(typeof(StructuralType), name);
            
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
