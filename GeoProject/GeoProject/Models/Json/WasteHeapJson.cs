using GeoProject.Helpers;
using System.Collections.Generic;

namespace GeoProject.Models
{
    public class WasteHeapJson
    {
        public string type;
        public List<Feature> features;

        public WasteHeapJson(List<NetTopologySuite.Geometries.Geometry> geometries, string name)
        {
            if (geometries == null)
                return;

            type = "FeatureCollection";
            features = new List<Feature>();

            foreach (var geometry in geometries)
            {
                var coordinates = new List<List<List<double>>>()
                {
                    new List<List<double>>()
                };

                foreach (var coord in geometry.Coordinates)
                {
                    var coordsConvert = GeometryHelper.EpsgConvertTo3857(coord.Y, coord.X);
                    var coords = new List<double>() { coordsConvert.x, coordsConvert.y };
                    coordinates[0].Add(coords);
                }

                features.Add(new Feature()
                {
                    properties = new Props()
                    {
                        name = name
                    },
                    type = "Feature",
                    geometry = new Geometry()
                    {
                        type = "Polygon",
                        coordinates = coordinates
                    }
                });
            }
        }

        public class Feature
        {
            public string type;
            public Props properties;
            public Geometry geometry;
        }

        public class Props
        {
            public string name;
            public float? buffer;
        }

        public class Geometry
        {
            public string type;
            public List<List<List<double>>> coordinates { get; set; }
        }
    }
}
