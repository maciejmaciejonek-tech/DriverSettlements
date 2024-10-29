using DriverSettlements.Common.DataContracts.Tenant;
using DriverSettlements.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriverSettlements.App.Models
{
    public class TenantWeek: Week
    {
        private List<ExternalReportMetadata>? _externalReports;
        public List<string>? TenantReportsValueNames { get; set; }
        public Dictionary<ExternalReportType, List<double>>? TenantReportsValues { get; set; }
        public Dictionary<string, double>? WeeklySettlementComponents { get; set; }

        public WeeklySettlementsInfo? WeeklySettlementsInfo { get; set; }

        public List<ExternalReportMetadata>? ExternalReports
        {
            get => _externalReports;
            set
            {
                _externalReports = value;
            }
        }
    }
}
