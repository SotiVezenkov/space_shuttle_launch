using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace space_shuttle_launch.Models
{
    public class Spaceport
    {
        public string Name { get; set; }
        public double Latitude { get; set; }
        public List<WeatherData> WeatherForecast { get; set; }
    }
}
