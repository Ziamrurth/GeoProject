using Newtonsoft.Json;
using System.IO;

namespace GeoProject.Helpers
{
    public class JsonReader {
        public static T LoadJson<T>(string fileName)
        {
            using (StreamReader r = new StreamReader(fileName))
            {
                string json = r.ReadToEnd();
                return JsonConvert.DeserializeObject<T>(json);
            }
        }
    }
}
