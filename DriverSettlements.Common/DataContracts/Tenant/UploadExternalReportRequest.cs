using DriverSettlements.Common.DataContracts;
using DriverSettlements.Common.Models;

namespace DriverSettlements.Common.DataContracts.Tenant
{
    public class UploadExternalReportRequest : BaseRequest
    {
        public int YearNumber { get; set; }
        public int WeekNumber { get; set; }
        public ExternalReportType ReportType { get; set; } = ExternalReportType.Unknown;
        public string FileName { get; set; } = string.Empty;
        public byte[] ReportFileContent { get; set; } = new byte[0];
    }
}

