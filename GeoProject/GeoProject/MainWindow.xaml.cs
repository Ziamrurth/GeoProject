using GeoProject.Helpers;
using GeoProject.Models;
using Microsoft.Win32;
using NetTopologySuite.Geometries;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using static GeoProject.Models.LandPlotInfo;

namespace GeoProject
{
    public partial class MainWindow : Window
    {
        public WasteHeapModel WasteHeapModel { get; set; }
        public List<Polygon> LandPlots { get; set; }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnOpenFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                var fileName = openFileDialog.FileName;
                var modelWasteHeap = JsonReader.LoadJson<WasteHeap>(fileName);
                var geometryWasteHeap = GeometryHelper.GetPolygonFromModel(modelWasteHeap);
                WasteHeapModel = new WasteHeapModel()
                {
                    WasteHeap = geometryWasteHeap
                };
            }
        }

        private void btnLoadLandsInfo_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                var fileName = openFileDialog.FileName;
                var modelLandPlot = JsonReader.LoadJson<LandPlots>(fileName);
                var geometryLandPlots = GeometryHelper.GetPolygonFromModel(modelLandPlot);
                LandPlots = geometryLandPlots;
            }
        }

        private void btnAddBuffers_Click(object sender, RoutedEventArgs e)
        {
            WasteHeapModel.Buffers = new List<Geometry>();

            string[] buffers = Buffers.Text.Split(';');
            foreach (var buffer in buffers)
            {
                var bufferSize = double.Parse(buffer)*0.00001;
                WasteHeapModel.Buffers.Add(
                    WasteHeapModel.WasteHeap.Buffer(bufferSize));
            }
        }

        private void btnProcess_Click(object sender, RoutedEventArgs e)
        {
            var lastBuffer = WasteHeapModel.Buffers.LastOrDefault();
            var landPlotsInRange = GetLandPlotsInsideBuffer(LandPlots, lastBuffer);

            var landPlotsInfo = new List<LandPlotInfo>();
            foreach (var landPlot in landPlotsInRange)
            {
                landPlotsInfo.Add(GetLandPlotInfo(landPlot, WasteHeapModel.Buffers));
            }
        }

        private List<Polygon> GetLandPlotsInsideBuffer(List<Polygon> landPlots, Geometry buffer)
        {
            var result = new List<Polygon>();

            foreach (var landPlot in landPlots)
            {
                if (buffer.Intersects(landPlot))
                    result.Add(landPlot);
            }

            return result;
        }

        private LandPlotInfo GetLandPlotInfo(Polygon landPlot, List<Geometry> buffers)
        {
            var landPlotPartsInfo = new List<LandPlotPartInfo>();

            foreach (var buffer in buffers)
            {
                if (buffer.Contains(landPlot))
                {
                    // ToDo: check, if its contains in smaller buffer
                    // ToDo: save buffer size
                    landPlotPartsInfo.Add(
                        new LandPlotPartInfo()
                        {
                            LandPart = landPlot,
                            Area = landPlot.Area,
                            AreaProportion = 1
                        });
                }else if (buffer.Intersects(landPlot))
                {
                    var landPart = landPlot.Difference(buffer);
                    landPlotPartsInfo.Add(
                        new LandPlotPartInfo()
                        {
                            LandPart = landPart,
                            Area = landPart.Area,
                            AreaProportion = landPart.Area/landPlot.Area
                        });
                }
            }

            return new LandPlotInfo()
            {
                LandPlot = landPlot,
                LandPlotPartsInfo = landPlotPartsInfo
            };
        }
    }
}
