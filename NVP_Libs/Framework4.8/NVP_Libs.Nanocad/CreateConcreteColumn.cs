using BIMStructureMgd.Common;
using BIMStructureMgd.DatabaseObjects;

using HostMgd.ApplicationServices;

using Teigha.DatabaseServices;
using Teigha.Geometry;

using System.Linq;
using System.Collections.Generic;

using NVP.API.Nodes;

using NVPXYZ = NVP.API.Geometry.XYZ;

namespace NVP_Libs.Nanocad
{
    [NodeInput("основа", typeof(NVPXYZ))]
    [NodeInput("высота", typeof(NVPXYZ))]
    [NodeInput("профиль", typeof(string))]
    public class CreateConcreteColumn : INode
    {
        public NodeResult Execute(INVPData context, List<NodeResult> inputs)
        {
            var basePoint = (NVPXYZ)inputs[0].Value;
            var height = (double)inputs[1].Value;
            var typeName = (string)inputs[2].Value;

            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            doc.Editor.WriteMessage("Создание колонны");

            // Запросить из базы объектов все профили Ж/Б объектов
            var request = LibraryRequest.CreateDatabaseRequest();
            request.AddCategoryCondition(LibraryObject.ConcreteProfileCategory);

            var profiles = request.Execute();

            // Выбрать из профилей (их было пять штук - прямоугольное, круглое, два тавровых и трапецивидное) прямоугольный.
            // Можно отфильтровать по первой букве имени. Если вдруг такой профиль не найден - взять первый попавшийся.
            var profile = profiles.FirstOrDefault(p => p.Name == typeName) ?? profiles.FirstOrDefault();

            if (profile == null)
            {
                throw new KeyNotFoundException("Профиль не найден");
            }

            var column = ConcreteColumn.Create(profile, null);

            var endPoint = basePoint.Offset(NVPXYZ.BasicZ, height);

            column.SetLocation(
                new Point3d(basePoint.X, basePoint.Y, basePoint.Z),
                new Point3d(endPoint.X, endPoint.Y, endPoint.Z),
                new Vector3d(NVPXYZ.BasicZ.X, NVPXYZ.BasicZ.Y, NVPXYZ.BasicZ.Z).GetNormal()
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
