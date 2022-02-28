using GeoProject.Models;
using GeoProject.Models.Json;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeoProject.Helpers
{
    public class GeometryHelper
    {
        private const double E = 2.7182818284;
        private const double X = 20037508.34;

        public static Polygon GetPolygonFromModel(WasteHeapJson model)
        {
            var geometryFactory = NetTopologySuite.NtsGeometryServices.Instance.CreateGeometryFactory(3857);

            var modelPointsList = model.features.First()
                .geometry.coordinates.First();

            var geometryCoordinatesList = new List<Coordinate>();
            foreach (var modelPoint in modelPointsList)
            {
                var coordinates = EpsgConvert(modelPoint[0], modelPoint[1]);
                var geometryCoordinate = new Coordinate(coordinates.y, coordinates.x);
                //var geometryCoordinate = new Coordinate(modelPoint[0], modelPoint[1]);
                geometryCoordinatesList.Add(geometryCoordinate);

                //var geometryCoordinate = EPSGConvert(modelPoint[0], modelPoint[1]);
                //geometryCoordinatesList.Add(geometryCoordinate);

                //geometryCoordinatesList.Add(new Coordinate(modelPoint[1], modelPoint[0]));
            }

            return geometryFactory.CreatePolygon(geometryCoordinatesList.ToArray());
        }

        public static Polygon GetPolygonFromModel(WasteHeapOSM model)
        {
            var geometryFactory = NetTopologySuite.NtsGeometryServices.Instance.CreateGeometryFactory(4326);

            var modelPointsList = model.elements.FirstOrDefault().geometry;

            var geometryCoordinatesList = new List<Coordinate>();
            foreach (var modelPoint in modelPointsList)
            {
                geometryCoordinatesList.Add(new Coordinate(modelPoint.lat, modelPoint.lon));
            }

            return geometryFactory.CreatePolygon(geometryCoordinatesList.ToArray());
        }

        public static List<LandPlotInfo> GetLandPlotsInfoFromModel(LandPlots model)
        {
            var geometryFactory = NetTopologySuite.NtsGeometryServices.Instance.CreateGeometryFactory(4326);

            var landPlotsInfo = new List<LandPlotInfo>();

            foreach (var modelItem in model.features)
            {
                var modelPointsList = modelItem.geometry.coordinates.First().First();

                var geometryCoordinatesList = new List<Coordinate>();

                foreach (var modelPoint in modelPointsList)
                {
                    var geometryCoordinate = new Coordinate(modelPoint[1], modelPoint[0]);
                    geometryCoordinatesList.Add(geometryCoordinate);
                }

                var polygonGeometry = geometryFactory.CreatePolygon(geometryCoordinatesList.ToArray());
                landPlotsInfo.Add(new LandPlotInfo()
                {
                    LandPlot = polygonGeometry,
                    cadastralNumber = modelItem.properties.cn
                }); ;
            }

            return landPlotsInfo;
        }

        private static (double x, double y) EpsgConvert(double x, double y)
        {
            var epsg3857ProjectedCoordinateSystem = ProjNet.CoordinateSystems.ProjectedCoordinateSystem.WebMercator;
            var epsg4326GeographicCoordinateSystem = ProjNet.CoordinateSystems.GeographicCoordinateSystem.WGS84;

            var coordinateTransformationFactory = new ProjNet.CoordinateSystems.Transformations.CoordinateTransformationFactory();
            var coordinateTransformation = coordinateTransformationFactory.CreateFromCoordinateSystems(epsg3857ProjectedCoordinateSystem, epsg4326GeographicCoordinateSystem);

            return coordinateTransformation.MathTransform.Transform(x, y);
        }

        //private static Coordinate EpsgConvert(double longitude, double latitude)
        //{
        //    double coordX = (longitude * 180) / X;
        //    double coordY = latitude / (X / 180);
        //    double exp = (Math.PI / 180) * coordY;
        //    coordY = Math.Atan(Math.Pow(E, exp));
        //    coordY = coordY / (Math.PI / 360);
        //    coordY = coordY - 90;

        //    return new Coordinate(coordX, coordY);
        //}
    }
}
