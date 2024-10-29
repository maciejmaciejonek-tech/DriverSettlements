using DriverSettlements.Common.DataContracts;
using DriverSettlements.Common.Models;

namespace DriverSettlements.Common.DataContracts.Tenant
{
    public class SaveTenantMetadataRequest : BaseRequest
    {
        public required Models.Tenant Tenant { get; set; }
    }
}
