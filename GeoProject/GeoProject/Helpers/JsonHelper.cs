using Newtonsoft.Json;
using System.IO;

namespace GeoProject.Helpers
{
    public class JsonHelper {
        public static T LoadJson<T>(string fileName)
        {
            using (StreamReader r = new StreamReader(fileName))
            {
                string json = r.ReadToEnd();
                return JsonConvert.DeserializeObject<T>(json);
            }
        }

        public static T FromJson<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        public static void SaveJson<T>(T objectToSave, string fileName)
        {
            var res = JsonConvert.SerializeObject(objectToSave);
            File.WriteAllText(fileName, res);
        }
    }
}
