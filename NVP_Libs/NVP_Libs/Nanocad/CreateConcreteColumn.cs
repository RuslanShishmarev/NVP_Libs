using BIMStructureMgd.Common;
using BIMStructureMgd.DatabaseObjects;

using HostMgd.ApplicationServices;

using Teigha.DatabaseServices;
using Teigha.Geometry;

using System.Linq;
using System.Collections.Generic;

using NVP.API.Nodes;

using NVPXYZ = NVP.API.Geometry.XYZ;
using NVPLine = NVP.API.Geometry.Line;

namespace NVP_Libs.Nanocad
{
    [NodeInput("начальная точка", typeof(NVPXYZ))]
    [NodeInput("конечная точка", typeof(NVPXYZ))]
    [NodeInput("вектор ориентации", typeof(NVPLine))]
    public class CreateConcreteColumn : INode
    {
        public NodeResult Execute(INVPData context, List<NodeResult> inputs)
        {
            var startPoint = (NVPXYZ)inputs[0].Value;
            var endPoint = (NVPXYZ)inputs[1].Value;
            var orientationLine = (NVPLine)inputs[2].Value;

            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            doc.Editor.WriteMessage("Создание колонны");

            // Запросить из базы объектов все профили Ж/Б объектов
            var request = LibraryRequest.CreateDatabaseRequest();
            request.AddCategoryCondition(LibraryObject.ConcreteProfileCategory);

            var profiles = request.Execute();

            // Выбрать из профилей (их было пять штук - прямоугольное, круглое, два тавровых и трапецивидное) прямоугольный.
            // Можно отфильтровать по первой букве имени. Если вдруг такой профиль не найден - взять первый попавшийся.
            var profile = profiles.FirstOrDefault(p => p.Name.StartsWith("П")) ?? profiles.FirstOrDefault();

            if (profile == null)
            {
                return new NodeResult("Профиль не найден");
            }

            var column = ConcreteColumn.Create(profile, null);
            column.SetLocation(
                new Point3d(startPoint.X, startPoint.Y, startPoint.Z),
                new Point3d(endPoint.X, endPoint.Y, endPoint.Z),
                new Vector3d(orientationLine.Vector.X, orientationLine.Vector.Y, orientationLine.Vector.Z).GetNormal()
            );

            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                Utilities.AddEntityToDatabase(db, tr, column);
                tr.Commit();
            }

            return new NodeResult(column);
        }
    }
}
