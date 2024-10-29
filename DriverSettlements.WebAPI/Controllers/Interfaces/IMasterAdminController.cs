using DriverSettlements.Common.DataContracts.MasterAdmin;
using DriverSettlements.Common.DataContracts.Tenant;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriverSettlements.WebAPI.Controllers.Interfaces
{
    public interface IMasterAdminController
    {

        IActionResult GetTenants(GetTenantsRequest request);
        IActionResult AddTenant(AddTenantRequest request);
        IActionResult UpdateTenant(UpdateTenantRequest request);
        IActionResult DeleteTenant(DeleteTenantRequest request);
        IActionResult SetTenantInvitationKey(SetTenantInvitationKeyRequest request);
        IActionResult AddDefaultReportDefinitions(AddDefaultReportDefinitionsRequest request);
        IActionResult AddSettlementTypes(AddSettlementTypesRequest request);
        IActionResult Ping(string ping);
        IActionResult Test();
    }
}
