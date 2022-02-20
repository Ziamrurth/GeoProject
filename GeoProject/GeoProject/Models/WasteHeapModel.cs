using NetTopologySuite.Geometries;
using System.Collections.Generic;

namespace GeoProject.Models
{
    public class WasteHeapModel
    {
        public Polygon WasteHeap { get; set; }
        public List<BufferInfo> BuffersInfo { get; set; }

        public class BufferInfo
        {
            public Geometry Buffer { get; set; }
            public double To { get; set; }
            public double From { get; set; }
        }
    }
}
