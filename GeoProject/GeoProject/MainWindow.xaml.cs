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
using System.Diagnostics;

namespace GeoProject
{
    public partial class MainWindow : Window
    {
        public List<WasteHeapModel> WasteHeapsModels { get; set; }
        public List<LandPlotInfo> LandPlotsInfo { get; set; }

        private readonly Settings _settings;

        public MainWindow()
        {
            InitializeComponent();

            if (!File.Exists("settings.json"))
            {
                _settings = new Settings
                {
                    BuffersSizes = "100;200;500;1000"
                };

                JsonHelper.SaveJson(_settings, "settings.json");
            }
            else
            {
                _settings = JsonHelper.LoadJson<Settings>("settings.json");
            }

            Buffers.Text = _settings.BuffersSizes;
        }

        private void btnWasteHeapFromOSM_Click(object sender, RoutedEventArgs e)
        {
            try
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
            catch (Exception ex)
            {
                LoggingHelper.LogError(ex);
                btnWasteHeapFromOSM.Background = Brushes.Red;
            }
        }

        private void btnWasteHeapFromOSMMultiple_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                try
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
                catch (Exception ex)
                {
                    LoggingHelper.LogError(ex);
                    btnWasteHeapFromOSMMultiple.Background = Brushes.Red;
                }
            }
        }

        private void btnWasteHeapFromJson_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "GeoJson files (*.geojson)|*.geojson|Text files (*.txt)|*.txt";
            if (openFileDialog.ShowDialog() == true)
            {
                try
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
                catch (Exception ex)
                {
                    LoggingHelper.LogError(ex);
                    btnWasteHeapFromJson.Background = Brushes.Red;
                }
            }
        }

        private void btnWasteHeapFromJsonMultiple_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "GeoJson files (*.geojson)|*.geojson|Text files (*.txt)|*.txt";
            openFileDialog.Multiselect = true;
            if (openFileDialog.ShowDialog() == true)
            {
                try
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
                catch (Exception ex)
                {
                    LoggingHelper.LogError(ex);
                    btnWasteHeapFromJsonMultiple.Background = Brushes.Red;
                }
            }
        }

        private void btnLoadLandsInfo_Click(object sender, RoutedEventArgs e)
        {
            try
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
            catch (Exception ex)
            {
                LoggingHelper.LogError(ex);
                btnLoadLandsInfo.Background = Brushes.Red;
            }
        }

        private void btnAddBuffers_Click(object sender, RoutedEventArgs e)
        {
            try
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
            catch (Exception ex)
            {
                LoggingHelper.LogError(ex);
                btnAddBuffers.Background = Brushes.Red;
            }
        }

        private void btnAddMultipleBuffers_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (var wasteHeapModel in WasteHeapsModels)
                {
                    double[] buffersSizeValues = default;
                    var inputDialog = new BufferInput(wasteHeapModel.Name, defaultAnswer: _settings.BuffersSizes);
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
            catch (Exception ex)
            {
                LoggingHelper.LogError(ex);
                btnAddMultipleBuffers.Background = Brushes.Red;
            }
        }

        private void btnProcess_Click(object sender, RoutedEventArgs e)
        {
            try
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
                            var areaProportion = landPlotIntersection.Area / landPlot.LandPlot.Area;
                            resultIntersection.Add(new LandPlotInfoCsv()
                            {
                                CadastralNumber = landPlot.CadastralNumber,
                                WasteHeap = intersection.WasteHeapModel.Name,
                                Direction1 = GetLandPlotDirection1(landPlot.LandPlot, intersection.WasteHeapModel.WasteHeap).ToString(),
                                Direction2 = GetLandPlotDirection2(landPlot.LandPlot, intersection.WasteHeapModel.WasteHeap).ToString(),
                                Direction3 = GetLandPlotDirection3(landPlot.LandPlot, intersection.WasteHeapModel.WasteHeap).ToString(),
                                BufferRange = $"{buffer.From * 100000} - {buffer.To * 100000}",
                                AreaProportion = areaProportion,
                                IntersectionIndex = intersection.IntersectionIndex,
                                Area = landPlot.Area,
                                AreaPart = landPlot.Area * areaProportion,
                                cad_cost = landPlot.cad_cost,
                                category_type = landPlot.category_type,
                                area_type = landPlot.area_type,
                                date_create = landPlot.date_create
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
                    Area = landPlotInfo.Area,
                    //Area = i.LandPlot.Area * 10000000000,
                    Direction1 = landPlotInfo.Direction1.ToString(),
                    Direction2 = landPlotInfo.Direction2.ToString(),
                    Direction3 = landPlotInfo.Direction3.ToString(),
                    BufferRange = $"{landPlotPartInfo.BufferInfo.From * 100000} - {landPlotPartInfo.BufferInfo.To * 100000}",
                    AreaPart = landPlotPartInfo.Area,
                    //AreaPart = p.Area * 10000000000,
                    AreaProportion = landPlotPartInfo.AreaProportion,
                    cad_cost = landPlotInfo.cad_cost,
                    category_type = landPlotInfo.category_type,
                    area_type = landPlotInfo.area_type,
                    date_create = landPlotInfo.date_create
                })).ToList();

                result.AddRange(resultIntersection);

                SaveFileDialog saveFileDialog = new SaveFileDialog();
                DateTime dateNow = DateTime.Now;
                saveFileDialog.Filter = "CSV file (.csv)|.csv| All Files (.)|.";
                saveFileDialog.FileName = $"result_{dateNow.ToString("dd.MM.yyyy_HH-mm")}";
                if (saveFileDialog.ShowDialog() == true)
                {

                    CsvSaveHelper.SaveToCsv(result, saveFileDialog.FileName);
                }

                btnProcess.Background = Brushes.Green;
            }
            catch (Exception ex)
            {
                LoggingHelper.LogError(ex);
                btnProcess.Background = Brushes.Red;
            }
        }

        private void btnSettings_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start("settings.json");
            }
            catch (Exception ex)
            {
                LoggingHelper.LogError(ex);
            }
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
                    var areaProportion = landPart.Area / landPlotInfo.LandPlot.Area;
                    landPlotPartsInfo.Add(
                        new LandPlotPartInfo()
                        {
                            LandPart = landPart,
                            BufferInfo = bufferInfo,
                            WasteHeap = wasteHeapModel.Name,
                            Area = landPlotInfo.Area * areaProportion,
                            AreaProportion = areaProportion
                        });
                }
            }


            landPlotInfo.Direction1 = GetLandPlotDirection1(landPlotInfo.LandPlot, wasteHeapModel.WasteHeap);
            landPlotInfo.Direction2 = GetLandPlotDirection2(landPlotInfo.LandPlot, wasteHeapModel.WasteHeap);
            landPlotInfo.Direction3 = GetLandPlotDirection3(landPlotInfo.LandPlot, wasteHeapModel.WasteHeap);
            landPlotInfo.LandPlotPartsInfo = landPlotPartsInfo;

            return landPlotInfo;
        }

        private Direction1 GetLandPlotDirection1(Polygon landPlot, Polygon wasteHeap)
        {
            var wasteHeapCenter = wasteHeap.Centroid;
            var landPlotCenter = landPlot.Centroid;

            var a = AngleUtility.Angle(wasteHeapCenter.Coordinate, landPlotCenter.Coordinate);
            a = a < 0
                ? 360 + a * (180 / Math.PI)
                : a * (180 / Math.PI);

            return (Direction1)(int)(a / 22.5 + 1);
        }

        private Direction2 GetLandPlotDirection2(Polygon landPlot, Polygon wasteHeap)
        {
            var wasteHeapCenter = wasteHeap.Centroid;
            var landPlotCenter = landPlot.Centroid;

            var a = AngleUtility.Angle(wasteHeapCenter.Coordinate, landPlotCenter.Coordinate);
            a = a < 0
                ? 360 + a * (180 / Math.PI)
                : a * (180 / Math.PI);

            return (Direction2)(int)(a / 45 + 1);
        }

        private double GetLandPlotDirection3(Polygon landPlot, Polygon wasteHeap)
        {
            var wasteHeapCenter = wasteHeap.Centroid;
            var landPlotCenter = landPlot.Centroid;

            var a = AngleUtility.Angle(wasteHeapCenter.Coordinate, landPlotCenter.Coordinate);
            return a < 0
                ? 360 + a * (180 / Math.PI)
                : a * (180 / Math.PI);
        }
    }
}
