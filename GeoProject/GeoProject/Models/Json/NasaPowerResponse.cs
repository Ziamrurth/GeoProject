using Newtonsoft.Json;
using System.Collections.Generic;

namespace GeoProject.Models.Json
{
    public class NasaPowerResponse
    {
        public string type { get; set; }
        public Geometry geometry { get; set; }
        public Properties properties { get; set; }
        public Header header { get; set; }
        public List<Message> messages { get; set; }
        public Parameters parameters { get; set; }
        public Times times { get; set; }



        public class Direction
        {
            public double WD_PCT { get; set; }
            public double WD_AVG { get; set; }
            public CLASSES CLASSES { get; set; }
            public string name { get; set; }
            public string range { get; set; }
        }

        public class ALL
        {
            public double CLASS_1 { get; set; }
            public double CLASS_2 { get; set; }
            public double CLASS_3 { get; set; }
            public double CLASS_4 { get; set; }
            public double CLASS_5 { get; set; }
            public double CLASS_6 { get; set; }
            public double CLASS_7 { get; set; }
            public double CLASS_8 { get; set; }
            public double CLASS_9 { get; set; }
        }

        public class Api
        {
            public string version { get; set; }
            public string name { get; set; }
        }

        public class CLASSES
        {
            public double CLASS_1 { get; set; }
            public double CLASS_2 { get; set; }
            public double CLASS_3 { get; set; }
            public double CLASS_4 { get; set; }
            public double CLASS_5 { get; set; }
            public double CLASS_6 { get; set; }
            public double CLASS_7 { get; set; }
            public double CLASS_8 { get; set; }
            public double CLASS_9 { get; set; }
            public double CLASS_10 { get; set; }
            public string Comment { get; set; }
            public WR10M WR10M { get; set; }
            public WR50M WR50M { get; set; }
        }

        public class DIRECTION
        {
            public string Comment { get; set; }

            [JsonProperty("0.0")]
            public Direction _00 { get; set; }

            [JsonProperty("22.5")]
            public Direction _225 { get; set; }

            [JsonProperty("45.0")]
            public Direction _450 { get; set; }

            [JsonProperty("67.5")]
            public Direction _675 { get; set; }

            [JsonProperty("90.0")]
            public Direction _900 { get; set; }

            [JsonProperty("112.5")]
            public Direction _1125 { get; set; }

            [JsonProperty("135.0")]
            public Direction _1350 { get; set; }

            [JsonProperty("157.5")]
            public Direction _1575 { get; set; }

            [JsonProperty("180.0")]
            public Direction _1800 { get; set; }

            [JsonProperty("202.5")]
            public Direction _2025 { get; set; }

            [JsonProperty("225.0")]
            public Direction _2250 { get; set; }

            [JsonProperty("247.5")]
            public Direction _2475 { get; set; }

            [JsonProperty("270.0")]
            public Direction _2700 { get; set; }

            [JsonProperty("292.5")]
            public Direction _2925 { get; set; }

            [JsonProperty("315.0")]
            public Direction _3150 { get; set; }

            [JsonProperty("337.5")]
            public Direction _3375 { get; set; }
        }

        public class Geometry
        {
            public string type { get; set; }
            public List<double> coordinates { get; set; }
        }

        public class Header
        {
            public string title { get; set; }
            public Api api { get; set; }
            public double fill_value { get; set; }
        }

        public class Message
        {
            public CLASSES CLASSES { get; set; }
            public DIRECTION DIRECTION { get; set; }
        }

        public class Parameter
        {
            public WR10M WR10M { get; set; }
            public WR50M WR50M { get; set; }
        }

        public class Parameters
        {
            public WR10M WR10M { get; set; }
            public WR50M WR50M { get; set; }
            public WDPCT WD_PCT { get; set; }
            public WDAVG WD_AVG { get; set; }
        }

        public class Properties
        {
            public Parameter parameter { get; set; }
        }

        public class Times
        {
            public double data { get; set; }
            public double process { get; set; }
        }

        public class WDAVG
        {
            public string longname { get; set; }
            public string units { get; set; }
            public string note { get; set; }
        }

        public class WDPCT
        {
            public string longname { get; set; }
            public string units { get; set; }
            public string note { get; set; }
        }

        public class WR10M
        {
            [JsonProperty("0.0")]
            public Direction _00 { get; set; }

            [JsonProperty("22.5")]
            public Direction _225 { get; set; }

            [JsonProperty("45.0")]
            public Direction _450 { get; set; }

            [JsonProperty("67.5")]
            public Direction _675 { get; set; }

            [JsonProperty("90.0")]
            public Direction _900 { get; set; }

            [JsonProperty("112.5")]
            public Direction _1125 { get; set; }

            [JsonProperty("135.0")]
            public Direction _1350 { get; set; }

            [JsonProperty("157.5")]
            public Direction _1575 { get; set; }

            [JsonProperty("180.0")]
            public Direction _1800 { get; set; }

            [JsonProperty("202.5")]
            public Direction _2025 { get; set; }

            [JsonProperty("225.0")]
            public Direction _2250 { get; set; }

            [JsonProperty("247.5")]
            public Direction _2475 { get; set; }

            [JsonProperty("270.0")]
            public Direction _2700 { get; set; }

            [JsonProperty("292.5")]
            public Direction _2925 { get; set; }

            [JsonProperty("315.0")]
            public Direction _3150 { get; set; }

            [JsonProperty("337.5")]
            public Direction _3375 { get; set; }
            public ALL ALL { get; set; }
            public string CLASS_1 { get; set; }
            public string CLASS_2 { get; set; }
            public string CLASS_3 { get; set; }
            public string CLASS_4 { get; set; }
            public string CLASS_5 { get; set; }
            public string CLASS_6 { get; set; }
            public string CLASS_7 { get; set; }
            public string CLASS_8 { get; set; }
            public string CLASS_9 { get; set; }
            public string CLASS_10 { get; set; }
            public string longname { get; set; }
            public string units { get; set; }
            public string note { get; set; }
        }

        public class WR50M
        {
            [JsonProperty("0.0")]
            public Direction _00 { get; set; }

            [JsonProperty("22.5")]
            public Direction _225 { get; set; }

            [JsonProperty("45.0")]
            public Direction _450 { get; set; }

            [JsonProperty("67.5")]
            public Direction _675 { get; set; }

            [JsonProperty("90.0")]
            public Direction _900 { get; set; }

            [JsonProperty("112.5")]
            public Direction _1125 { get; set; }

            [JsonProperty("135.0")]
            public Direction _1350 { get; set; }

            [JsonProperty("157.5")]
            public Direction _1575 { get; set; }

            [JsonProperty("180.0")]
            public Direction _1800 { get; set; }

            [JsonProperty("202.5")]
            public Direction _2025 { get; set; }

            [JsonProperty("225.0")]
            public Direction _2250 { get; set; }

            [JsonProperty("247.5")]
            public Direction _2475 { get; set; }

            [JsonProperty("270.0")]
            public Direction _2700 { get; set; }

            [JsonProperty("292.5")]
            public Direction _2925 { get; set; }

            [JsonProperty("315.0")]
            public Direction _3150 { get; set; }

            [JsonProperty("337.5")]
            public Direction _3375 { get; set; }
            public ALL ALL { get; set; }
            public string CLASS_1 { get; set; }
            public string CLASS_2 { get; set; }
            public string CLASS_3 { get; set; }
            public string CLASS_4 { get; set; }
            public string CLASS_5 { get; set; }
            public string CLASS_6 { get; set; }
            public string CLASS_7 { get; set; }
            public string CLASS_8 { get; set; }
            public string CLASS_9 { get; set; }
            public string CLASS_10 { get; set; }
            public string longname { get; set; }
            public string units { get; set; }
            public string note { get; set; }
        }
    }

}
