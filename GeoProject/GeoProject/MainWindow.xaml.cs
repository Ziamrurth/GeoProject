using GeoProject.Helpers;
using GeoProject.Models;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GeoProject {
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class OpenFile : Window
    {
        public OpenFile()
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
            }
        }
    }
}
