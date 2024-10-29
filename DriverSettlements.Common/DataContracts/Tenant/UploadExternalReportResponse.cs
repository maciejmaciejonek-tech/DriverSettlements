using DriverSettlements.Common.Models;
using static DriverSettlements.Common.Models.ExternalReport;

namespace DriverSettlements.Common.DataContracts.Tenant
{
    public class UploadExternalReportResponse : BaseResponse
    {
        public ExternalReportMetadata? Report { get; set; }    
    }
}