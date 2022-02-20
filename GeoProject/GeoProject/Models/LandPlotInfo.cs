using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GeoProject.Models.WasteHeapModel;

namespace GeoProject.Models
{
    public class LandPlotInfo
    {
        public Polygon LandPlot { get; set; }
        public string cadastralNumber { get; set; }
        public Direction Direction { get; set; }
        public List<LandPlotPartInfo> LandPlotPartsInfo { get; set; }

        public class LandPlotPartInfo
        {
            public Geometry LandPart { get; set; }
            public BufferInfo BufferInfo { get; set; }
            public double Area { get; set; }
            public double AreaProportion { get; set; }
        }
    }
    public enum Direction
    {
        N = 1,
        NNE = 2,
        NE = 3,
        ENE = 4,
        E = 5,
        ESE = 6,
        SE = 7,
        SSE = 8,
        S = 9,
        SSW = 10,
        SW = 11,
        WSW = 12,
        W = 13,
        WNW = 14,
        NW = 15,
        NNW = 16
    }
}
