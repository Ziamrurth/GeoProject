using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoProject.Models.CSV
{
    class LandPlotInfoCsv
    {
        public string CadastralNumber { get; set; }
        public string WasteHeap { get; set; }
        public string WHC_lon { get; set; }
        public string WHC_lat { get; set; }
        public double Area { get; set; }
        public string Direction1 { get; set; }
        public string Direction2 { get; set; }
        public string Direction3 { get; set; }
        public string BufferRange { get; set; }
        public double AreaPart { get; set; }
        public double AreaProportion { get; set; }
        public int? IntersectionIndex { get; set; }
        public double cad_cost { get; set; }
        public string category_type { get; set; }
        public string area_type { get; set; }
        public string date_create { get; set; }
    }
}
