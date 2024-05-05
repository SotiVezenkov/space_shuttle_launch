using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace space_shuttle_launch
{
    public class ReportDataMap : ClassMap<ReportData>
    {
        public ReportDataMap()
        {
            Map(m => m.SpaceportName);
            Map(m => m.BestLaunchDay);
            Map(m => m.Latitude).Ignore();
        }
    }
}

