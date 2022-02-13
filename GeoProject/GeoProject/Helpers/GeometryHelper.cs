using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoProject.Helpers
{
    public class GeometryHelper
    {
        private const double E = 2.7182818284;
        private const double X = 20037508.34;

        public static Polygon GetPolygonFromModel(Models.Polygon model)
        {
            var geometryFactory = NetTopologySuite.NtsGeometryServices.Instance.CreateGeometryFactory(4326);

            var modelPointsList = model.features.First()
                .geometry.coordinates.First();

            var geometryCoordinatesList = new List<Coordinate>();
            foreach (var modelPoint in modelPointsList)
            {
                var geometryCoordinate = EPSGConvert(modelPoint[0], modelPoint[1]);
                geometryCoordinatesList.Add(geometryCoordinate);
            }

            return geometryFactory.CreatePolygon(geometryCoordinatesList.ToArray());
        }

        private static Coordinate EPSGConvert(double longitude, double latitude)
        {
            double coordX = (longitude * 180) / X;
            //double coordY = ((Math.Atan(Math.Pow(E, ((Math.PI / 180) * latitude)))) / (Math.PI / 360)) - 90;

            double coordY1 = latitude / (X / 180);
            double exp = (Math.PI / 180) * coordY1;
            double coordY2 = Math.Atan(Math.Pow(E, exp));
            double coordY3 = coordY2 / (Math.PI / 360);
            double coordY4 = coordY3 - 90;

            return new Coordinate(coordX, coordY4);
        }
    }
}
