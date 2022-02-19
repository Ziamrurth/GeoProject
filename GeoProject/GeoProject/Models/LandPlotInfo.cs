using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoProject.Models
{
    public class LandPlotInfo
    {
        public Polygon LandPlot { get; set; }
        public List<LandPlotPartInfo> LandPlotPartsInfo { get; set; }

        public class LandPlotPartInfo
        {
            public Geometry LandPart { get; set; }
            public double BufferSize { get; set; }
            public double Area { get; set; }
            public double AreaProportion { get; set; }
        }
    }
}
