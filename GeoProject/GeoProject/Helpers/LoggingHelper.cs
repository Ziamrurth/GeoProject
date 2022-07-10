using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoProject.Helpers
{
    public static class LoggingHelper
    {
        public static void LogError(Exception ex)
        {
            string error = ex.ToString();
            DateTime dateNow = DateTime.Now;
            string fileName = $@"Errors\error_{dateNow.ToString("dd.MM.yyyy_HH-mm")}.txt";

            if (!Directory.Exists("Errors"))
                Directory.CreateDirectory("Errors");

            File.WriteAllText(fileName, error, Encoding.GetEncoding("Windows-1251"));

            Process.Start($@"Errors");
        }
    }
}
