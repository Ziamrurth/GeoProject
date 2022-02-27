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
        //public double Area { get; set; }
        public string Direction { get; set; }
        public string BufferRange { get; set; }
        //public double AreaPart { get; set; }
        public double AreaProportion { get; set; }
    }
}
