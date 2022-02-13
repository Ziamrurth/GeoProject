using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoProject.Helpers {
    public class JsonReader {
        public static Polygon LoadJson(string fileName)
        {
            using (StreamReader r = new StreamReader(fileName))
            {
                string json = r.ReadToEnd();
                var res = JsonConvert.DeserializeObject<Polygon>(json);
                Polygon polygon = JsonConvert.DeserializeObject<Polygon>(json);
                return polygon;
            }
        }
    }

    public class Polygon {
        public string type;
        public List<Feature> features;
    }

    public class Feature {
        public string type;
        public Props properties;
        public Geometry geometry;
    }

    public class Props {
        public string name;
        public float buffer;
    }

    public class Geometry {
        public string type;
        public List<List<List<double>>> coordinates { get; set; }
    }
}
