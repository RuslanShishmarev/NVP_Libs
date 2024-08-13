using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;

using NVP.API.Nodes;

using System;
using System.Collections.Generic;

using RevitLine = Autodesk.Revit.DB.Line;

namespace NVP_Libs.Revit.Common
{
    [NodeInput("линия", typeof(RevitLine))]
    [NodeInput("типоразмер", typeof(FamilySymbol))]
    [NodeInput("уровень", typeof(Level))]
    [NodeInput("структурный тип", typeof(string))]
    public class PlaceFamilyInstanceLine : INode
    {
        public NodeResult Execute(INVPData context, List<NodeResult> inputs)
        {
            var doc = (context.GetCADContext() as ExternalCommandData).Application.ActiveUIDocument.Document;

            var line = (RevitLine)inputs[0].Value;
            var familySymbol = (FamilySymbol)inputs[1].Value;
            var level = (Level)inputs[2].Value;
            var name = (string)inputs[3].Value;

            var structuralType = (StructuralType)Enum.Parse(typeof(StructuralType), name);

            using (Transaction transaction = new Transaction(doc, "Размещение экземпляра семейства по линии"))
            {
                transaction.Start();
                var instance = doc.Create.NewFamilyInstance(line, familySymbol, level, structuralType);
                transaction.Commit();
                return new NodeResult(instance);
            }
        }
    }
}