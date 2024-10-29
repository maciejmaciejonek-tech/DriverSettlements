using DriverSettlements.Common.Models;
using static DriverSettlements.Common.Models.ExternalReport;

namespace DriverSettlements.Common.DataContracts.Tenant
{
    public class DownloadTenantFuelBillPhotoResponse : BaseResponse
    {
        public byte[]? PhotoContent { get; set; }
    }
}