using System.Collections.Generic;

namespace GeoProject.Models
{
    public class LandPlots
    {
        public string type { get; set; }
        public List<Feature> features { get; set; }

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
