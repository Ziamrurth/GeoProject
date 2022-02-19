using NetTopologySuite.Geometries;
using System.Collections.Generic;

namespace GeoProject.Models
{
    public class WasteHeapModel
    {
        public Polygon WasteHeap { get; set; }
        public List<Geometry> Buffers { get; set; }
    }
}
