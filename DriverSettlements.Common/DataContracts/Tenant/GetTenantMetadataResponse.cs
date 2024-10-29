using DriverSettlements.Common.DataContracts;
using DriverSettlements.Common.Models;

namespace DriverSettlements.Common.Interfaces.Tenant
{
    public class GetTenantMetadataResponse:BaseResponse
    {
        public Models.Tenant? Tenant { get; set; }
    }

}

