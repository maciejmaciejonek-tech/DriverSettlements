using DriverSettlements.Common.Models;
using static DriverSettlements.Common.Models.ExternalReport;

namespace DriverSettlements.Common.DataContracts.DriverInterface
{
    public class DownloadDriverFuelBillPhotoResponse : BaseResponse
    {
        public byte[]? PhotoContent { get; set; }
    }
}