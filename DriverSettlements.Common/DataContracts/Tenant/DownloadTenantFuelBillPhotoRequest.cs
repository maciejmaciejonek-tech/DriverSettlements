using DriverSettlements.Common.Models;

namespace DriverSettlements.Common.DataContracts.Tenant
{
    public class DownloadTenantFuelBillPhotoRequest : BaseRequest
    {
        public required int FuelBillId { get; set; }
        public required double RequestedImageWidth { get; set; }
        public required double RequestedImageHeight { get; set; }

    }
}

