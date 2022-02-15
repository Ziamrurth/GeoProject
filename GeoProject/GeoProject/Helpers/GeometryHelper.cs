using GeoProject.Models;
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

        public static Polygon GetPolygonFromModel(WasteHeap model)
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

        public static List<Polygon> GetPolygonFromModel(LandPlots model)
        {
            var geometryFactory = NetTopologySuite.NtsGeometryServices.Instance.CreateGeometryFactory(4326);

            var polygonsList = new List<Polygon>();

            foreach (var modelItem in model.features)
            {
                var modelPointsList = modelItem.geometry.coordinates.First().First();

                var geometryCoordinatesList = new List<Coordinate>();

                foreach (var modelPoint in modelPointsList)
                {
                    var geometryCoordinate = new Coordinate(modelPoint[0], modelPoint[1]);
                    geometryCoordinatesList.Add(geometryCoordinate);
                }

                var polygonGeometry = geometryFactory.CreatePolygon(geometryCoordinatesList.ToArray());
                polygonsList.Add(polygonGeometry);
            }

            return polygonsList;
        }

        private static Coordinate EPSGConvert(double longitude, double latitude)
        {
            double coordX = (longitude * 180) / X;
            double coordY = latitude / (X / 180);
            double exp = (Math.PI / 180) * coordY;
            coordY = Math.Atan(Math.Pow(E, exp));
            coordY = coordY / (Math.PI / 360);
            coordY = coordY - 90;

            return new Coordinate(coordX, coordY);
        }
    }
}
