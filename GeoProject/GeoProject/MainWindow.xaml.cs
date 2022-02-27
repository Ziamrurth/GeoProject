using GeoProject.Helpers;
using GeoProject.Models;
using GeoProject.Models.CSV;
using Microsoft.Win32;
using NetTopologySuite.Algorithm;
using NetTopologySuite.Geometries;
using Geometry = NetTopologySuite.Geometries.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using static GeoProject.Models.LandPlotInfo;
using static GeoProject.Models.WasteHeapModel;
using System.Net;
using System.IO;
using GeoProject.Models.Json;

namespace GeoProject
{
    public partial class MainWindow : Window
    {
        public WasteHeapModel WasteHeapModel { get; set; }
        public List<LandPlotInfo> LandPlotsInfo { get; set; }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnWasteHeapFromOSM_Click(object sender, RoutedEventArgs e)
        {
            string wasteHeapJson = OverpassHelper.GetJsonById(WasteHeap.Text);
            var wasteHeap = JsonHelper.FromJson<WasteHeapOSM>(wasteHeapJson);
            WasteHeapModel = new WasteHeapModel()
            {
                WasteHeap = GeometryHelper.GetPolygonFromModel(wasteHeap)
            };
            btnWasteHeapFromOSM.Background = Brushes.Green;
        }

        private void btnWasteHeapFromJson_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                var fileName = openFileDialog.FileName;
                var modelWasteHeap = JsonHelper.LoadJson<WasteHeapJson>(fileName);
                var geometryWasteHeap = GeometryHelper.GetPolygonFromModel(modelWasteHeap);
                WasteHeapModel = new WasteHeapModel()
                {
                    WasteHeap = geometryWasteHeap
                };
            }
            btnWasteHeapFromJson.Background = Brushes.Green;
        }

        private void btnLoadLandsInfo_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                var fileName = openFileDialog.FileName;
                var modelLandPlot = JsonHelper.LoadJson<LandPlots>(fileName);
                var geometryLandPlots = GeometryHelper.GetLandPlotsInfoFromModel(modelLandPlot);
                LandPlotsInfo = geometryLandPlots;
            }
            btnLoadLandsInfo.Background = Brushes.Green;
        }

        private void btnAddBuffers_Click(object sender, RoutedEventArgs e)
        {
            string[] buffersSize = Buffers.Text.Split(';');
            double[] buffersSizeValues = new double[buffersSize.Length];

            for (int i = 0; i < buffersSize.Length; i++)
            {
                buffersSizeValues[i] = double.Parse(buffersSize[i]) * 0.00001;
            }
            buffersSizeValues = buffersSizeValues.Distinct().ToArray();
            Array.Sort(buffersSizeValues);

            var buffersInfo = new List<BufferInfo>();
            for (int i = 0; i < buffersSizeValues.Length; i++)
            {
                buffersInfo.Add(
                    new BufferInfo
                    {
                        Buffer = WasteHeapModel.WasteHeap.Buffer(buffersSizeValues[i]),
                        To = buffersSizeValues[i],
                        From = i == 0 ? 0 : buffersSizeValues[i - 1]
                    });
            }

            for (int i = buffersSizeValues.Length - 1; i > 0; i--)
            {
                buffersInfo[i].Buffer = buffersInfo[i].Buffer.Difference(buffersInfo[i - 1].Buffer);
            }

            WasteHeapModel.BuffersInfo = buffersInfo;
            btnAddBuffers.Background = Brushes.Green;
        }

        private void btnProcess_Click(object sender, RoutedEventArgs e)
        {
            var lastBuffer = WasteHeapModel.WasteHeap.Buffer(WasteHeapModel.BuffersInfo.LastOrDefault().To);
            var landPlotsInRange = GetLandPlotsInsideBuffer(LandPlotsInfo, lastBuffer);

            var landPlotsInfo = new List<LandPlotInfo>();
            foreach (var landPlot in landPlotsInRange)
            {
                landPlotsInfo.Add(GetLandPlotInfo(landPlot, WasteHeapModel));
            }

            IEnumerable<LandPlotInfoCsv> result = landPlotsInfo.SelectMany(i => i.LandPlotPartsInfo
            .Select(p => new LandPlotInfoCsv()
            {
                CadastralNumber = i.cadastralNumber,
                //Area = i.LandPlot.Area * 10000000000,
                Direction = i.Direction.ToString(),
                BufferRange = $"{p.BufferInfo.From * 100000} - {p.BufferInfo.To * 100000}",
                //AreaPart = p.Area * 10000000000,
                AreaProportion = p.AreaProportion
            }));

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "CSV file (.csv)|.csv| All Files (.)|.";
            saveFileDialog.FileName = "result";
            if (saveFileDialog.ShowDialog() == true)
            {
                
                CsvSaveHelper.SaveToCsv(result, saveFileDialog.FileName);
            }

            btnProcess.Background = Brushes.Green;
        }

        private List<LandPlotInfo> GetLandPlotsInsideBuffer(List<LandPlotInfo> landPlotsInfo, Geometry buffer)
        {
            var result = new List<LandPlotInfo>();

            foreach (var landPlot in landPlotsInfo)
            {
                if (buffer.Intersects(landPlot.LandPlot))
                    result.Add(landPlot);
            }

            return result;
        }

        private LandPlotInfo GetLandPlotInfo(LandPlotInfo landPlotInfo, WasteHeapModel wasteHeapModel)
        {
            var landPlotPartsInfo = new List<LandPlotPartInfo>();

            foreach (var bufferInfo in wasteHeapModel.BuffersInfo)
            {
                if (bufferInfo.Buffer.Intersects(landPlotInfo.LandPlot))
                {
                    var landPart = landPlotInfo.LandPlot.Intersection(bufferInfo.Buffer);
                    landPlotPartsInfo.Add(
                        new LandPlotPartInfo()
                        {
                            LandPart = landPart,
                            BufferInfo = bufferInfo,
                            Area = landPart.Area,
                            AreaProportion = landPart.Area / landPlotInfo.LandPlot.Area
                        });
                }
            }


            landPlotInfo.Direction = GetLandPlotDirection(landPlotInfo.LandPlot, wasteHeapModel.WasteHeap);
            landPlotInfo.LandPlotPartsInfo = landPlotPartsInfo;

            return landPlotInfo;
        }

        private Direction GetLandPlotDirection(Polygon landPlot, Polygon wasteHeap)
        {
            var wasteHeapCenter = wasteHeap.Centroid;
            var landPlotCenter = landPlot.Centroid;

            var a = AngleUtility.Angle(wasteHeapCenter.Coordinate, landPlotCenter.Coordinate);
            a = a < 0
                ? 360 + a * (180 / Math.PI)
                : a * (180 / Math.PI);

            return (Direction)(int)(a / 22.1 + 1);
        }
    }
}
