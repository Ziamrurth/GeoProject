using NetTopologySuite.Geometries;

namespace GeoProject.Models
{
    public class WasteHeapIntersection
    {
        public Geometry Segment { get; set; }
        public int IntersectionIndex { get; set; }
        public WasteHeapModel WasteHeapModel { get; set; }
        public int BufferIndex { get; set; }
    }
}
