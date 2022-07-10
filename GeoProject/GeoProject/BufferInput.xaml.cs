using GeoProject.Helpers;
using GeoProject.Models.Json;
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
using System.Windows.Shapes;

namespace GeoProject
{
    public partial class BufferInput : Window
    {
        private NetTopologySuite.Geometries.Point _center;

        public BufferInput(string wasteHeapName, NetTopologySuite.Geometries.Point center, string defaultAnswer = "")
        {
            InitializeComponent();
			lblQuestion.Content = $"Буферные зоны для террикона {wasteHeapName}";
			txtAnswer.Text = defaultAnswer;
            _center = center;
		}

		private void btnDialogOk_Click(object sender, RoutedEventArgs e)
		{
			this.DialogResult = true;
		}

        private void btnRoseWind_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                GetWindRoseBuffer(_center);
                btnRoseWind.Background = Brushes.Green;
            }
            catch (Exception ex)
            {
                LoggingHelper.LogError(ex);
                btnRoseWind.Background = Brushes.Red;
            }

        }

        private void Window_ContentRendered(object sender, EventArgs e)
		{
			txtAnswer.SelectAll();
			txtAnswer.Focus();
		}

        private void GetWindRoseBuffer(NetTopologySuite.Geometries.Point center)
        {
            string responseString = NasaPowerHelper.GetJsonByCoords(center.X, center.Y);
            var response = JsonHelper.FromJson<NasaPowerResponse>(responseString);
            var winds = response.properties.parameter.WR10M;

            N.Text = winds._00.WD_AVG.ToString();
            NE.Text = winds._450.WD_AVG.ToString();
            E.Text = winds._900.WD_AVG.ToString();
            SE.Text = winds._1350.WD_AVG.ToString();
            S.Text = winds._1800.WD_AVG.ToString();
            SW.Text = winds._2250.WD_AVG.ToString();
            W.Text = winds._2700.WD_AVG.ToString();
            NW.Text = winds._3150.WD_AVG.ToString();
        }

        public string Answer
		{
			get { return txtAnswer.Text; }
		}
	}
}
