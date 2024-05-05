using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace space_shuttle_launch.Models
{
    public class WeatherData
    {
        public int Day { get; set; }
        public double Temperature { get; set; }
        public double Wind { get; set; }
        public double Humidity { get; set; }
        public bool Precipitation { get; set; }
        public bool Lightning { get; set; }
        public string Clouds { get; set; }
    }

}
