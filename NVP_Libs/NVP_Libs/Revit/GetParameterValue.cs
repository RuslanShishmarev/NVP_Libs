using Autodesk.Revit.DB;

using NVP.API.Nodes;

using System;
using System.Collections.Generic;

namespace NVP_Libs.Revit
{
    [NodeInput("параметр", typeof(Parameter))]
    public class GetParameterValue : INode
    {
        public NodeResult Execute(INVPData context, List<NodeResult> inputs)
        {
            var parameter = (Parameter)inputs[0].Value;
            StorageType storageType = parameter.StorageType;
            if (parameter == null)
            {
                throw new ArgumentNullException(nameof(parameter), "Параметр null");
            }
            switch (storageType)
            {
                case StorageType.String:
                    var valueS = parameter.AsString();
                    return new NodeResult(valueS);
                case StorageType.Integer:
                    var valueI = parameter.AsInteger();
                    return new NodeResult(valueI);
                case StorageType.Double:
                    var valueD = parameter.AsDouble();
                    return new NodeResult(valueD);
                case StorageType.ElementId:
                    var valueE = parameter.AsElementId();
                    return new NodeResult(valueE);
            }
            return new NodeResult(null);
        }
    }
}
