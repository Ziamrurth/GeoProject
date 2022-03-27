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
using GeoProject.Models.Json;
using System.IO;

namespace GeoProject
{
    public partial class MainWindow : Window
    {
        public List<WasteHeapModel> WasteHeapsModels { get; set; }
        public List<LandPlotInfo> LandPlotsInfo { get; set; }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnWasteHeapFromOSM_Click(object sender, RoutedEventArgs e)
        {
            string wasteHeapJson = OverpassHelper.GetJsonById(WasteHeap.Text);
            var wasteHeap = JsonHelper.FromJson<WasteHeapOSM>(wasteHeapJson);
            var wasteHeapModel = new WasteHeapModel()
            {
                WasteHeap = GeometryHelper.GetPolygonFromModel(wasteHeap),
                Name = wasteHeap.elements[0].id.ToString()
            };
            WasteHeapsModels = new List<WasteHeapModel>();
            WasteHeapsModels.Add(wasteHeapModel);

            btnWasteHeapFromOSM.Background = Brushes.Green;
        }

        private void btnWasteHeapFromOSMMultiple_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                var fileName = openFileDialog.FileName;
                string[] wasteHeapIds = File.ReadAllLines(fileName);
                WasteHeapsModels = new List<WasteHeapModel>();
                foreach (string wasteHeapId in wasteHeapIds)
                {
                    string wasteHeapJson = OverpassHelper.GetJsonById(wasteHeapId);
                    var wasteHeap = JsonHelper.FromJson<WasteHeapOSM>(wasteHeapJson);
                    var wasteHeapModel = new WasteHeapModel()
                    {
                        WasteHeap = GeometryHelper.GetPolygonFromModel(wasteHeap),
                        Name = wasteHeap.elements[0].id.ToString()
                    };
                    WasteHeapsModels.Add(wasteHeapModel);
                }

                btnWasteHeapFromOSMMultiple.Background = Brushes.Green;
            }
        }

        private void btnWasteHeapFromJson_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "GeoJson files (*.geojson)|*.geojson|Text files (*.txt)|*.txt";
            if (openFileDialog.ShowDialog() == true)
            {
                var fileName = openFileDialog.FileName;
                var modelWasteHeap = JsonHelper.LoadJson<WasteHeapJson>(fileName);
                var geometryWasteHeap = GeometryHelper.GetPolygonFromModel(modelWasteHeap);
                var wasteHeapModel = new WasteHeapModel()
                {
                    WasteHeap = geometryWasteHeap,
                    Name = modelWasteHeap.features[0].properties.name
                };
                WasteHeapsModels = new List<WasteHeapModel>();
                WasteHeapsModels.Add(wasteHeapModel);

                btnWasteHeapFromJson.Background = Brushes.Green;
            }
        }

        private void btnWasteHeapFromJsonMultiple_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "GeoJson files (*.geojson)|*.geojson|Text files (*.txt)|*.txt";
            openFileDialog.Multiselect = true;
            if (openFileDialog.ShowDialog() == true)
            {
                WasteHeapsModels = new List<WasteHeapModel>();
                foreach (var fileName in openFileDialog.FileNames)
                {
                    var modelWasteHeap = JsonHelper.LoadJson<WasteHeapJson>(fileName);
                    var geometryWasteHeap = GeometryHelper.GetPolygonFromModel(modelWasteHeap);
                    var wasteHeapModel = new WasteHeapModel()
                    {
                        WasteHeap = geometryWasteHeap,
                        Name = modelWasteHeap.features[0].properties.name
                    };
                    WasteHeapsModels.Add(wasteHeapModel);
                }

                btnWasteHeapFromJsonMultiple.Background = Brushes.Green;
            }
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
                //buffersSizeValues[i] = double.Parse(buffersSize[i]);
            }
            buffersSizeValues = buffersSizeValues.Distinct().ToArray();
            Array.Sort(buffersSizeValues);

            foreach (var wasteHeapModel in WasteHeapsModels)
            {
                var buffersInfo = new List<BufferInfo>();
                for (int i = 0; i < buffersSizeValues.Length; i++)
                {
                    buffersInfo.Add(
                        new BufferInfo
                        {
                            Buffer = wasteHeapModel.WasteHeap.Buffer(buffersSizeValues[i]),
                            To = buffersSizeValues[i],
                            From = i == 0 ? 0 : buffersSizeValues[i - 1]
                        });
                }

                for (int i = buffersSizeValues.Length - 1; i > 0; i--)
                {
                    buffersInfo[i].Buffer = buffersInfo[i].Buffer.Difference(buffersInfo[i - 1].Buffer);
                }

                wasteHeapModel.BuffersInfo = buffersInfo;
            }

            //CsvSaveHelper.TestSave(WasteHeapsModels.FirstOrDefault().BuffersInfo.FirstOrDefault().Buffer.Coordinates);

            btnAddBuffers.Background = Brushes.Green;
        }

        private void btnAddMultipleBuffers_Click(object sender, RoutedEventArgs e)
        {
            foreach (var wasteHeapModel in WasteHeapsModels)
            {
                double[] buffersSizeValues = default;
                var inputDialog = new BufferInput(wasteHeapModel.Name);
                if (inputDialog.ShowDialog() == true)
                {
                    var bufferInput = inputDialog.Answer;
                    string[] buffersSize = bufferInput.Split(';');
                    buffersSizeValues = new double[buffersSize.Length];
                    for (int i = 0; i < buffersSize.Length; i++)
                    {
                        buffersSizeValues[i] = double.Parse(buffersSize[i]) * 0.00001;
                    }
                    buffersSizeValues = buffersSizeValues.Distinct().ToArray();
                    Array.Sort(buffersSizeValues);
                }

                var buffersInfo = new List<BufferInfo>();
                for (int i = 0; i < buffersSizeValues.Length; i++)
                {
                    buffersInfo.Add(
                        new BufferInfo
                        {
                            Buffer = wasteHeapModel.WasteHeap.Buffer(buffersSizeValues[i]),
                            To = buffersSizeValues[i],
                            From = i == 0 ? 0 : buffersSizeValues[i - 1]
                        });
                }

                for (int i = buffersSizeValues.Length - 1; i > 0; i--)
                {
                    buffersInfo[i].Buffer = buffersInfo[i].Buffer.Difference(buffersInfo[i - 1].Buffer);
                }

                wasteHeapModel.BuffersInfo = buffersInfo;
            }

            btnAddMultipleBuffers.Background = Brushes.Green;
        }

        private void btnProcess_Click(object sender, RoutedEventArgs e)
        {
            var landPlotsInfo = new List<LandPlotInfo>();

            // Разделение терриконов на пересекающиеся и одиночные
            var wasteHeapsIntersects = new List<WasteHeapModel>();
            for (int i = 0; i < WasteHeapsModels.Count - 1; i++)
            {
                for (int j = i + 1; j < WasteHeapsModels.Count; j++)
                {
                    var maxBufferI = WasteHeapsModels[i].WasteHeap.Buffer(WasteHeapsModels[i].BuffersInfo.LastOrDefault().To);
                    var maxBufferJ = WasteHeapsModels[j].WasteHeap.Buffer(WasteHeapsModels[j].BuffersInfo.LastOrDefault().To);
                    if (maxBufferI.Intersects(maxBufferJ))
                    {
                        wasteHeapsIntersects.Add(WasteHeapsModels[i]);
                        wasteHeapsIntersects.Add(WasteHeapsModels[j]);
                    }
                }
            }
            wasteHeapsIntersects = wasteHeapsIntersects.GroupBy(w => w)
                .SelectMany(x => x).ToList();
            var wasteHeapsSingle = WasteHeapsModels.Where(x => !wasteHeapsIntersects.Contains(x)).ToList();

            // ToDo: Обработка случая для пересекающихся терриконов

            //Поиск пересечений буферов
            var wasteHeapIntersections = new List<WasteHeapIntersection>();
            for (int i = 0; i < wasteHeapsIntersects.Count - 1; i++)
            {
                for (int j = i + 1; j < wasteHeapsIntersects.Count; j++)
                {
                    var bufferListI = wasteHeapsIntersects[i].BuffersInfo;
                    var bufferListJ = wasteHeapsIntersects[j].BuffersInfo;

                    foreach (var bufferI in bufferListI)
                    {
                        foreach (var bufferJ in bufferListJ)
                        {
                            if (bufferI.Buffer.Intersects(bufferJ.Buffer))
                            {
                                var intersection = bufferI.Buffer.Intersection(bufferJ.Buffer);
                                var intersectionIndex = wasteHeapIntersections.Count;
                                wasteHeapIntersections.Add(new WasteHeapIntersection()
                                {
                                    Segment = intersection,
                                    IntersectionIndex = intersectionIndex + 1,
                                    WasteHeapModel = wasteHeapsIntersects[i],
                                    BufferIndex = wasteHeapsIntersects[i].BuffersInfo.IndexOf(bufferI)
                                });
                                wasteHeapIntersections.Add(new WasteHeapIntersection()
                                {
                                    Segment = intersection,
                                    IntersectionIndex = intersectionIndex + 1,
                                    WasteHeapModel = wasteHeapsIntersects[j],
                                    BufferIndex = wasteHeapsIntersects[j].BuffersInfo.IndexOf(bufferJ)
                                });

                            }
                        }
                        // Убрать из всех буферов пересечения
                        var buffI = bufferI.Buffer
                            .Difference(wasteHeapsIntersects[j].WasteHeap
                                .Buffer(wasteHeapsIntersects[j].BuffersInfo.LastOrDefault().To));
                        wasteHeapIntersections.Add(new WasteHeapIntersection()
                        {
                            Segment = buffI,
                            IntersectionIndex = 0,
                            WasteHeapModel = wasteHeapsIntersects[i],
                            BufferIndex = wasteHeapsIntersects[i].BuffersInfo.IndexOf(bufferI)
                        });
                    }
                    foreach (var bufferJ in bufferListJ)
                    {
                        // Убрать из всех буферов пересечения
                        var buffJ = bufferJ.Buffer
                               .Difference(wasteHeapsIntersects[i].WasteHeap
                                   .Buffer(wasteHeapsIntersects[i].BuffersInfo.LastOrDefault().To));
                        wasteHeapIntersections.Add(new WasteHeapIntersection()
                        {
                            Segment = buffJ,
                            IntersectionIndex = 0,
                            WasteHeapModel = wasteHeapsIntersects[j],
                            BufferIndex = wasteHeapsIntersects[j].BuffersInfo.IndexOf(bufferJ)
                        });
                    }
                }
            }

            //JsonHelper.SaveJson(new LandPlots(wasteHeapIntersections), "WasteHeapIntersections.json");

            // Обработка участков, попавших в пересечение буферов
            List<LandPlotInfoCsv> resultIntersection = new List<LandPlotInfoCsv>();
            foreach (var landPlot in LandPlotsInfo)
            {
                foreach (var intersection in wasteHeapIntersections)
                {
                    if (landPlot.LandPlot.Intersects(intersection.Segment))
                    {
                        var buffer = intersection.WasteHeapModel.BuffersInfo[intersection.BufferIndex];
                        var landPlotIntersection = landPlot.LandPlot.Intersection(intersection.Segment);
                        resultIntersection.Add(new LandPlotInfoCsv()
                        {
                            CadastralNumber = landPlot.CadastralNumber,
                            WasteHeap = intersection.WasteHeapModel.Name,
                            Direction = GetLandPlotDirection(landPlot.LandPlot, intersection.WasteHeapModel.WasteHeap).ToString(),
                            BufferRange = $"{buffer.From * 100000} - {buffer.To * 100000}",
                            AreaProportion = landPlotIntersection.Area / landPlot.LandPlot.Area,
                            IntersectionIndex = intersection.IntersectionIndex
                        });
                    }
                }
            }

            // Обработка одиночных полигонов
            foreach (var wasteHeapModel in wasteHeapsSingle)
            {
                var lastBuffer = wasteHeapModel.WasteHeap.Buffer(wasteHeapModel.BuffersInfo.LastOrDefault().To);
                var landPlotsInRange = GetLandPlotsInsideBuffer(LandPlotsInfo, lastBuffer);

                foreach (var landPlot in landPlotsInRange)
                {
                    landPlotsInfo.Add(GetLandPlotInfo(landPlot, wasteHeapModel));
                }
            }

            List<LandPlotInfoCsv> result = landPlotsInfo.SelectMany(landPlotInfo => landPlotInfo.LandPlotPartsInfo
            .Select(landPlotPartInfo => new LandPlotInfoCsv()
            {
                CadastralNumber = landPlotInfo.CadastralNumber,
                WasteHeap = landPlotPartInfo.WasteHeap,
                //Area = i.LandPlot.Area * 10000000000,
                Direction = landPlotInfo.Direction.ToString(),
                BufferRange = $"{landPlotPartInfo.BufferInfo.From * 100000} - {landPlotPartInfo.BufferInfo.To * 100000}",
                //AreaPart = p.Area * 10000000000,
                AreaProportion = landPlotPartInfo.AreaProportion
            })).ToList();

            result.AddRange(resultIntersection);

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
                            WasteHeap = wasteHeapModel.Name,
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
