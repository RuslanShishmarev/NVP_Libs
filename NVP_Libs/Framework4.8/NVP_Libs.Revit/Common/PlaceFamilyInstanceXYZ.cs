using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;

using NVP.API.Nodes;

using System;
using System.Collections.Generic;

using RevitXYZ = Autodesk.Revit.DB.XYZ;


namespace NVP_Libs.Revit.Common
{
    [NodeInput("координата", typeof(RevitXYZ))]
    [NodeInput("типоразмер", typeof(FamilySymbol))]
    [NodeInput("уровень", typeof(Level))]
    [NodeInput("структурный тип", typeof(string))]
    public class PlaceFamilyInstanceXYZ : INode
    {
        public NodeResult Execute(INVPData context, List<NodeResult> inputs)
        {         
            var doc = (context.GetCADContext() as ExternalCommandData).Application.ActiveUIDocument.Document;

            var point = (RevitXYZ)inputs[0].Value;
            var familySymbol = (FamilySymbol)inputs[1].Value;
            var level = (Level)inputs[2].Value;
            var name = (string)inputs[3].Value;

            var structuralType = (StructuralType) Enum.Parse(typeof(StructuralType), name);
            
            using (Transaction transaction = new Transaction(doc, "Размещение экземпляра семейства по точке"))
            {
                transaction.Start();
                var instance = doc.Create.NewFamilyInstance(point, familySymbol, level, structuralType);
                transaction.Commit();
                return new NodeResult(instance);
            }
        }
    }
}
