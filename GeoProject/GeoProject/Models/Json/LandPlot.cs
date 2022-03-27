using System.Collections.Generic;

namespace GeoProject.Models
{
    public class LandPlots
    {
        public string type { get; set; }
        public List<Feature> features { get; set; }

        public LandPlots(List<WasteHeapIntersection> wasteHeapIntersections)
        {
            if (wasteHeapIntersections == null)
                return;
            type = "FeatureCollection";
            features = new List<Feature>();
            foreach (var wasteHeapIntersection in wasteHeapIntersections)
            {
                var coordinates = new List<List<List<List<double>>>>()
                {
                    new List<List<List<double>>>()
                    {
                        new List<List<double>>()
                    }
                };

                foreach (var coord in wasteHeapIntersection.Segment.Coordinates)
                {
                    var coords = new List<double>() { coord.Y, coord.X };
                    coordinates[0][0].Add(coords);
                }

                features.Add(new Feature()
                {
                    type = "Feature",
                    geometry = new Geometry()
                    {
                        type = "MultiPolygon",
                        coordinates = coordinates
                    }
                });
            }
        }

        public class Feature
        {
            public string type { get; set; }
            public Property properties { get; set; }
            public Geometry geometry { get; set; }
        }

        public class Geometry
        {
            public string type { get; set; }
            public List<List<List<List<double>>>> coordinates { get; set; }
        }

        public class Property
        {
            public string cn { get; set; }
        }
    }
}
