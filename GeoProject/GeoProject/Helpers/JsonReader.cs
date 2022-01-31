using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoProject.Helpers {
    class JsonReader {
        public void LoadJson(string fileName)
        {
            using (StreamReader r = new StreamReader(fileName))
            {
                string json = r.ReadToEnd();
                List<Polygon> polygons = JsonConvert.DeserializeObject<List<Polygon>>(json);
            }
        }
    }

    public class Polygon {
        public string type;
        public Features features;
    }

    public class Features {
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
        public Coordinates coordinates;
    }

    public class Coordinates {
        public float x;
        public float y;
    }
}
