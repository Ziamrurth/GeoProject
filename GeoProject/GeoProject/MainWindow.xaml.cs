using GeoProject.Helpers;
using GeoProject.Models;
using Microsoft.Win32;
using NetTopologySuite.Geometries;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
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
    }
}
