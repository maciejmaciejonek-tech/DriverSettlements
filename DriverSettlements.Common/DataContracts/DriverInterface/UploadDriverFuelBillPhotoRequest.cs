using DriverSettlements.Common.DataContracts;
using DriverSettlements.Common.Models;

namespace DriverSettlements.Common.DataContracts.DriverInterface
{
    public class UploadDriverFuelBillPhotoRequest : BaseRequest
    {
        public required int FuelBillId { get; set; }
        public required byte[] PhotoContent { get; set; } 
    }
}

