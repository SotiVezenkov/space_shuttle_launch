using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace space_shuttle_launch.Models
{
    public class WeatherCriteria
    {
        public double MinTemperature { get; set; }
        public double MaxTemperature { get; set; }
        public double MaxWindSpeed { get; set; }
        public double MaxHumidity { get; set; }
        public bool AllowPrecipitation { get; set; }
        public bool AllowLightning { get; set; }
        public List<string> DisallowedCloudTypes { get; set; }
    }
}
