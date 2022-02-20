using GeoProject.Helpers;
using GeoProject.Models;
using Microsoft.Win32;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using static GeoProject.Models.LandPlotInfo;
using static GeoProject.Models.WasteHeapModel;

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
        }

        private void btnProcess_Click(object sender, RoutedEventArgs e)
        {
            var lastBuffer = WasteHeapModel.WasteHeap.Buffer(WasteHeapModel.BuffersInfo.LastOrDefault().To);
            var landPlotsInRange = GetLandPlotsInsideBuffer(LandPlots, lastBuffer);

            var landPlotsInfo = new List<LandPlotInfo>();
            foreach (var landPlot in landPlotsInRange)
            {
                landPlotsInfo.Add(GetLandPlotInfo(landPlot, WasteHeapModel.BuffersInfo));
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

        private LandPlotInfo GetLandPlotInfo(Polygon landPlot, List<BufferInfo> buffersInfo)
        {
            var landPlotPartsInfo = new List<LandPlotPartInfo>();

            foreach (var bufferInfo in buffersInfo)
            {
                if (bufferInfo.Buffer.Intersects(landPlot))
                {
                    var landPart = landPlot.Intersection(bufferInfo.Buffer);
                    landPlotPartsInfo.Add(
                        new LandPlotPartInfo()
                        {
                            LandPart = landPart,
                            BufferInfo = bufferInfo,
                            Area = landPart.Area,
                            AreaProportion = landPart.Area / landPlot.Area
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
