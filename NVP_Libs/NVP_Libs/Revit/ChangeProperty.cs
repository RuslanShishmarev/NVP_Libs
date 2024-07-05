using System.Collections.Generic;

using Autodesk.Revit.UI;
using Autodesk.Revit.DB;

using NVP.API.Nodes;

namespace NVP_Libs.Revit
{
    [NodeInput("свойство", typeof(Parameter))]
    [NodeInput("новое значение", typeof(object))]
    internal class ChangeProperty : IRevitNode
    {
        public NodeResult Execute(IVisualViewerData context, List<NodeResult> inputs, object commandData)
        {
            var doc = (commandData as ExternalCommandData).Application.ActiveUIDocument.Document;

            var parameter = (Parameter)inputs[0].Value;
            var newValue = inputs[1].Value;
            StorageType storageType = parameter.StorageType;

            using (Transaction transaction = new Transaction(doc, "Change Property"))
            {
                if (parameter != null && !parameter.IsReadOnly)
                {
                    transaction.Start();
                    switch (storageType)
                    {
                        case StorageType.String:
                            parameter.Set((string)newValue);
                            break;
                        case StorageType.Integer:
                            parameter.Set((int)newValue);
                            break;
                        case StorageType.Double:
                            parameter.Set((double)newValue);
                            break;
                        case StorageType.ElementId:
                            parameter.Set((ElementId)newValue);
                            break;
                    }
                    transaction.Commit();
                    return new NodeResult(parameter);
                }
                return null;
            }
        }
    }
}

