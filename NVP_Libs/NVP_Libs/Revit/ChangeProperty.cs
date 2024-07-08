using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

using NVP.API.Nodes;

using System.Collections.Generic;

namespace NVP_Libs.Revit
{
    [NodeInput("свойство", typeof(Parameter))]
    [NodeInput("новое значение", typeof(object))]
    internal class ChangeProperty : INode
    {
        public NodeResult Execute(INVPData context, List<NodeResult> inputs)
        {
            var doc = (context.GetCADContext() as ExternalCommandData).Application.ActiveUIDocument.Document;

            var parameter = (Parameter)inputs[0].Value;
            var newValue = inputs[1].Value;
            StorageType storageType = parameter.StorageType;

            using (Transaction transaction = new Transaction(doc, "Изменение свойства"))
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
                            parameter.Set((double)newValue * 3.28084);
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

