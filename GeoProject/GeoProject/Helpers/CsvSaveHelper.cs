using NetTopologySuite.Geometries;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using static GeoProject.Models.WasteHeapModel;

namespace GeoProject.Helpers
{
    class CsvSaveHelper
    {
        public static void SaveToCsv<T>(IEnumerable<T> data, string path)
        {
            var lines = new List<string>();
            IEnumerable<PropertyDescriptor> props = TypeDescriptor.GetProperties(typeof(T)).OfType<PropertyDescriptor>();
            var header = string.Join(";", props.ToList().Select(x => x.Name));
            lines.Add(header);
            var valueLines = data.Select(
                row => string.Join(";", header.Split(';').Select(
                    a => row.GetType().GetProperty(a).GetValue(row, null))));
            lines.AddRange(valueLines);
            File.WriteAllLines(path, lines.ToArray(), Encoding.GetEncoding("Windows-1251"));
        }

        public static void TestSave(Coordinate[] bufferInfo)
        {
            string res = "";
            foreach (var coord in bufferInfo)
            {
                res += $"[{coord.X.ToString().Replace(',', '.')},{coord.Y.ToString().Replace(',', '.')}],";
            }
            File.WriteAllText("result.txt", res, Encoding.GetEncoding("Windows-1251"));
        }
    }
}
