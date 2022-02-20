using System;
using System.Collections.Generic;

namespace GeoProject.Models.Json
{
    public class Osm3s
    {
        public DateTime timestamp_osm_base { get; set; }
        public string copyright { get; set; }
    }

    public class Bounds
    {
        public double minlat { get; set; }
        public double minlon { get; set; }
        public double maxlat { get; set; }
        public double maxlon { get; set; }
    }

    public class Geometry
    {
        public double lat { get; set; }
        public double lon { get; set; }
    }

    public class Tags
    {
        public string landuse { get; set; }
    }

    public class Element
    {
        public string type { get; set; }
        public int id { get; set; }
        public Bounds bounds { get; set; }
        public List<int> nodes { get; set; }
        public List<Geometry> geometry { get; set; }
        public Tags tags { get; set; }
    }

    public class WasteHeapOSM
    {
        public double version { get; set; }
        public string generator { get; set; }
        public Osm3s osm3s { get; set; }
        public List<Element> elements { get; set; }
    }


}
