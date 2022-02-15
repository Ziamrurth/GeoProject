using System.Collections.Generic;

namespace GeoProject.Models
{
    public class WasteHeap
    {
        public string type;
        public List<Feature> features;

        public class Feature
        {
            public string type;
            public Props properties;
            public Geometry geometry;
        }

        public class Props
        {
            public string name;
            public float buffer;
        }

        public class Geometry
        {
            public string type;
            public List<List<List<double>>> coordinates { get; set; }
        }
    }
}
