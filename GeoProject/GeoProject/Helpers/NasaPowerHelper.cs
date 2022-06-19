using System;
using System.Globalization;
using System.IO;
using System.Net;

namespace GeoProject.Helpers
{
    class NasaPowerHelper
    {
        public static string GetJsonByCoords(double lon, double lat)
        {
            DateTime dateEnd = DateTime.Now;
            DateTime dateStart = dateEnd.AddYears(-10);

            string dateStartString = dateStart.ToString("yyyyMMdd");
            string dateEndString = dateEnd.ToString("yyyyMMdd");

            string url = $"https://power.larc.nasa.gov/api/application/windrose/point?Longitude={lon.ToString(CultureInfo.InvariantCulture)}&latitude={lat.ToString(CultureInfo.InvariantCulture)}&start={dateStartString}&end={dateEndString}&format=JSON";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.AutomaticDecompression = DecompressionMethods.GZip;

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
