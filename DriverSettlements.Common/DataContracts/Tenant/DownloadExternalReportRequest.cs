using DriverSettlements.Common.DataContracts;
using DriverSettlements.Common.Models;

namespace DriverSettlements.Common.DataContracts.Tenant
{
    public class DownloadExternalReportRequest : BaseRequest
    {
        public ExternalReportType ReportType { get; set; } = ExternalReportType.Unknown;
        public int YearNumber { get; set; }
        public int WeekNumber { get; set; }
        public bool Overwrite { get; set; } = false;
    }
}

