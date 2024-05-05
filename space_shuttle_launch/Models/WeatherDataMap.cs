using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace space_shuttle_launch.Models
{
    public class WeatherDataMap : ClassMap<WeatherData>
    {
        public WeatherDataMap()
        {
            Map(m => m.Day).Name("Day");
            Map(m => m.Temperature).Name("Temperature");
            Map(m => m.Wind).Name("Wind");
            Map(m => m.Humidity).Name("Humidity");
            Map(m => m.Precipitation).Name("Precipitation");
            Map(m => m.Lightning).Name("Lightning");
            Map(m => m.Clouds).Name("Clouds");
        }
    }
}
