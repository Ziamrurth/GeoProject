using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GeoProject.Models.WasteHeapModel;
using MIConvexHull;

namespace GeoProject.Models
{
    public class WindRoseModel
    {
        public double Coef { get; set; }
        public Dictionary<Direction2, double> Directions { get; set; }

        public WindRoseModel(MainWindow mw)
        {
            if (string.IsNullOrEmpty(mw.N.Text))
            {
                return;
            }

            Directions = new Dictionary<Direction2, double>()
            {
                { Direction2.N, double.Parse(mw.N.Text) },
                { Direction2.NE, double.Parse(mw.NE.Text) },
                { Direction2.E, double.Parse(mw.E.Text) },
                { Direction2.SE, double.Parse(mw.SE.Text) },
                { Direction2.S, double.Parse(mw.S.Text) },
                { Direction2.SW, double.Parse(mw.SW.Text) },
                { Direction2.W, double.Parse(mw.W.Text) },
                { Direction2.NW, double.Parse(mw.NW.Text) }
            };
            Coef = double.Parse(mw.Coef.Text);
        }

        public List<Vertex2d> GetWindRosePoints(Coordinate center)
        {
            List<Vertex2d> coords = new List<Vertex2d>();

            foreach (var direction in Directions)
            {
                coords.Add(GetCoord(center, GetAzimut(direction.Key), direction.Value * Coef));
            }

            return coords;
        }

        public BufferInfo GetBufferInfo(Polygon wasteHeap)
        {
            List<Vertex2d> vertexList = new List<Vertex2d>();

            foreach (var point in wasteHeap.Coordinates)
            {
                vertexList.AddRange(GetWindRosePoints(point));
            }

            var convexHull = ConvexHull.Create2D(vertexList);

            var coords = convexHull.Result.Select(vertex => new Coordinate(vertex.X, vertex.Y)).ToList();
            coords.Add(coords[0]);

            var geometryFactory = NetTopologySuite.NtsGeometryServices.Instance.CreateGeometryFactory(4326);
            var windRosePolygon = geometryFactory.CreatePolygon(coords.ToArray());

            return new BufferInfo { Buffer = windRosePolygon, From = -1, To = Coef };
        }

        private Vertex2d GetCoord(Coordinate center, double azimut, double length)
        {
            var centerLon = center.X;
            var centerLat = center.Y;

            double lat = centerLat + length * Math.Cos(azimut * Math.PI / 180) / (6371000 * Math.PI / 180);
            double lon = centerLon + length * Math.Sin(azimut * Math.PI / 180) / Math.Cos(centerLat * Math.PI / 180) / (6371000 * Math.PI / 180);

            return new Vertex2d() { X = lon, Y = lat };
        }

        private double GetAzimut(Direction2 dir)
        {
            switch (dir)
            {
                case Direction2.N:
                    return 0;
                case Direction2.NE:
                    return 45;
                case Direction2.E:
                    return 90;
                case Direction2.SE:
                    return 135;
                case Direction2.S:
                    return 180;
                case Direction2.SW:
                    return 225;
                case Direction2.W:
                    return 270;
                case Direction2.NW:
                    return 315;
                default:
                    return 0;
            }
        }
    }

    public class Vertex2d : IVertex2D
    {
        public double X { get; set; }

        public double Y { get; set; }
    }
}
