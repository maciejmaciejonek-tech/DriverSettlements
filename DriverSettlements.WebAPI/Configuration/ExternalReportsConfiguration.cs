using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriverSettlements.WebAPI.Configuration
{
    public class ExternalReportsConfiguration
    {
        public const string SectionName = "ExternalReports";
        public string ReportsFolder { get; set; } = string.Empty;
        public string ExternalServicesAdapterFile { get; set; } = string.Empty;
    }
}
