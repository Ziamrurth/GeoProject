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
        public string CadastralNumber { get; set; }
        public Direction1 Direction1 { get; set; }
        public Direction2 Direction2 { get; set; }
        public double Direction3 { get; set; }
        public List<LandPlotPartInfo> LandPlotPartsInfo { get; set; }
        public double Area { get; set; }
        public double cad_cost { get; set; }
        public string category_type { get; set; }
        public string area_type { get; set; }
        public string date_create { get; set; }

        public class LandPlotPartInfo
        {
            public Geometry LandPart { get; set; }
            public BufferInfo BufferInfo { get; set; }
            public string WasteHeap { get; set; }
            public Point WasteHeapCenter { get; set; }
            public double Area { get; set; }
            public double AreaProportion { get; set; }
        }
    }

    public enum Direction1
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

    public enum Direction2
    {
        N = 1,
        NE = 2,
        E = 3,
        SE = 4,
        S = 5,
        SW = 6,
        W = 7,
        NW = 8
    }
}
