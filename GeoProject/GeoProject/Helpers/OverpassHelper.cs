using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GeoProject.Helpers
{
    public class OverpassHelper
    {
        public static string GetJsonById(string osmId)
        {
            string url = $"http://overpass-api.de//api/interpreter?data=[out:json];way({osmId});out geom;";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.AutomaticDecompression = DecompressionMethods.GZip;

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }

            //var wasteHeap = JsonHelper.FromJson<WasteHeapOSM>(httpResult);
            //WasteHeapModel = new WasteHeapModel()
            //{
            //    WasteHeap = GeometryHelper.GetPolygonFromModel(wasteHeap)
            //};
        }
    }
}
