using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

using NVP.API.Nodes;

using System;
using System.Collections.Generic;

namespace NVP_Libs.Revit.Common
{
    [NodeInput("параметр", typeof(Parameter))]
    [NodeInput("новое значение", typeof(object))]
    public class ChangeParameter : INode
    {
        public NodeResult Execute(INVPData context, List<NodeResult> inputs)
        {
            var doc = (context.GetCADContext() as ExternalCommandData).Application.ActiveUIDocument.Document;

            var parameter = (Parameter)inputs[0].Value;
            var newValue = inputs[1].Value;
            StorageType storageType = parameter.StorageType;

            if (parameter == null)
            {
                throw new ArgumentNullException(nameof(parameter), "Параметр не может быть null");
            }
            else if (parameter.IsReadOnly)
            {
                throw new InvalidOperationException("Параметр только для чтения");
            }
            using (Transaction transaction = new Transaction(doc, "Изменение свойства"))
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
        }
    }
}

