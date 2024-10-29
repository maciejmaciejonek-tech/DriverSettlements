using DriverSettlements.Common.DataContracts;
using DriverSettlements.Common.DataContracts.MasterAdmin;
using DriverSettlements.Common.DataContracts.Tenant;
using DriverSettlements.Common.Interfaces;
using DriverSettlements.Common.Models;
using DriverSettlements.Database;
using DriverSettlements.WebAPI.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace TenantSettlements.WebAPI.Services
{
    public class MasterAdminService:IMasterAdmin
    {
        private readonly DatabaseContext _dbContext;
        private readonly ILogger<MasterAdminService> _logger;
        private readonly ExternalReportsConfiguration _externalReportsConfiguration;

        public MasterAdminService(DatabaseContext dbContext, ILogger<MasterAdminService> logger,  IOptions<ExternalReportsConfiguration> externalReportsConfiguration)
        {
            _dbContext = dbContext;
            _logger = logger;
            _externalReportsConfiguration = externalReportsConfiguration.Value;
        }

        public GetTenantsResponse GetTenants(GetTenantsRequest request)
        {
            try
            {
                var applicationInstance = CheckPriviledgesAndGetApplicationInstance(request);
                return new GetTenantsResponse()
                {
                    Result = true,
                    Tenants = request.IncludeDeleted
                                ? _dbContext.Tenants.Cast<LegalEntity>().ToList()
                                : _dbContext.Tenants.Where(t => !t.Deleted).Cast<LegalEntity>().ToList()
                };
            }
            catch (Exception ex)
            {
                return new GetTenantsResponse()
                {
                    Result = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        public AddTenantResponse AddTenant(AddTenantRequest request)
        {
            try
            {
                var applicationInstance = CheckPriviledgesAndGetApplicationInstance(request);
                var tenant = new Tenant()
                {
                    FriendlyName = request.Tenant.FriendlyName,
                    CompanyName = request.Tenant.CompanyName,
                    EmailAddress = request.Tenant.EmailAddress,
                    Blocked = request.Tenant.Blocked,
                    Deleted = request.Tenant.Deleted
                };
                _dbContext.Tenants.Add(tenant);
                _dbContext.SaveChanges();
                return new AddTenantResponse()
                {
                    Result = true,
                    TenantId = tenant.Id
                };
            }
            catch (Exception ex)
            {
                return new AddTenantResponse()
                {
                    Result = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        public UpdateTenantResponse UpdateTenant(UpdateTenantRequest request)
        {
            try
            {
                var applicationInstance = CheckPriviledgesAndGetApplicationInstance(request);
                var tenant = _dbContext.Tenants.FirstOrDefault(t => t.Id == request.Tenant.Id);
                if (tenant == null)
                    return new UpdateTenantResponse()
                    {
                        Result = false,
                        ErrorMessage = $"Tenant with key {request.Tenant.Id} does not exist"
                    };
                tenant.FriendlyName = request.Tenant.FriendlyName;
                tenant.CompanyName = request.Tenant.CompanyName;
                tenant.EmailAddress = request.Tenant.EmailAddress;
                tenant.ApplicationFeeChargeDriver = request.Tenant.ApplicationFeeChargeDriver;
                tenant.ApplicationFeeBaseValue = request.Tenant.ApplicationFeeBaseValue;
                tenant.Blocked = request.Tenant.Blocked;
                tenant.Deleted = request.Tenant.Deleted;
                _dbContext.SaveChanges();
                return new UpdateTenantResponse()
                {
                    Result = true
                };
            }
            catch (Exception ex)
            {
                return new UpdateTenantResponse()
                {
                    Result = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        public DeleteTenantResponse DeleteTenant(DeleteTenantRequest request)
        {
            try
            {
                var applicationInstance = CheckPriviledgesAndGetApplicationInstance(request);
                var tenant = _dbContext.Tenants.FirstOrDefault(t => t.Id == request.TenantId);
                if (tenant == null)
                    return new DeleteTenantResponse()
                    {
                        Result = false,
                        ErrorMessage = $"Tenant with key {request.TenantId} does not exist"
                    };
                _dbContext.Tenants.Remove(tenant);
                _dbContext.SaveChanges();
                return new DeleteTenantResponse()
                {
                    Result = true
                };
            }
            catch (Exception ex)
            {
                return new DeleteTenantResponse()
                {
                    Result = false,
                    ErrorMessage = ex.Message
                };
            }

        }

        public SetTenantInvitationKeyResponse SetTenantInvitationKey(SetTenantInvitationKeyRequest request)
        {
            try
            {
                var applicationInstance = CheckPriviledgesAndGetApplicationInstance(request);
                var tenant = _dbContext.Tenants.FirstOrDefault(t => t.Id == request.TenantId);
                if (tenant == null)
                    return new SetTenantInvitationKeyResponse()
                    {
                        Result = false,
                        ErrorMessage = $"Tenant with key {request.TenantId} does not exist"
                    };

                Random generator = new Random();
                var invitationKey = generator.Next(0, 100000000).ToString("D8");
                while (_dbContext.Tenants.Count(t => invitationKey.Equals(t.InvitationKey)) != 0
                    || _dbContext.Tenants.Count(t => invitationKey.Equals(t.InvitationKey)) != 0)
                    invitationKey = generator.Next(0, 100000000).ToString("D8");
                tenant.InvitationKey = invitationKey;
                _dbContext.SaveChanges();
                return new SetTenantInvitationKeyResponse()
                {
                    Result = true,
                    InvitationKey = invitationKey
                };
            }
            catch (Exception ex)
            {
                return new SetTenantInvitationKeyResponse()
                {
                    Result = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        public AddDefaultReportDefinitionsResponse AddDefaultReportDefinitions(AddDefaultReportDefinitionsRequest request)
        {
            try
            {
                var applicationInstance = CheckPriviledgesAndGetApplicationInstance(request);
                if (!applicationInstance.MasterAdmin)
                    throw new Exception("Missing priviledges to MasterAdmin interface");
                var defReports = _dbContext.DefReports.Include(r=>r.DefReportColumns);
                foreach (var defReport in defReports)
                    _dbContext.DefReports.Remove(defReport);
                _dbContext.SaveChanges();
                var defReport0 = new DefReport()
                {
                    ReportName = "Global templates",
                    DefReportColumns = new List<DefReportColumn>()
                    {
                        new DefReportColumn(){Lp=0,ColumnName="DriverName",
                            DefBoltValue="{Kierowca}",
                            DefUberValue="{Imię kierowcy} {Nazwisko kierowcy}",
                            DefFreenowValue="{Driver}"}
                    }
                };
                _dbContext.DefReports.Add(defReport0);
                var defReport1 = new DefReport()
                {
                    ReportName = "Basic report",
                    DefReportColumns = new List<DefReportColumn>()
                    {
                        new DefReportColumn(){Lp=0,ColumnName="Kierowca",
                            DefValue="{DriverName}"},
                        new DefReportColumn(){Lp=1,ColumnName="Gotówka",
                            DefBoltValue="(-1)*{Przejazdy gotówkowe (przyjęta gotówka)}",
                            DefUberValue="(-1)*{Wypłacono Ci : Bilans przejazdu : Wypłaty : Odebrana gotówka}",
                            DefFreenowValue="{Non-app payments}"},
                        new DefReportColumn(){Lp=2,ColumnName="Konto spółki",
                            DefBoltValue="{Tygodniowy bilans}",
                            DefUberValue="{Wypłacono Ci}",
                            DefFreenowValue="{FREENOW balance}"},
                        new DefReportColumn(){Lp=3,ColumnName="Utarg brutto",
                            DefBoltValue="{Wartość brutto}",
                            DefUberValue="{Wypłacono Ci : Twój przychód}",
                            DefFreenowValue="{Earnings before commission}"}                    }
                };
                _dbContext.DefReports.Add(defReport1);
                _dbContext.SaveChanges();
                return new AddDefaultReportDefinitionsResponse()
                {
                    Result = true
                };
            }
            catch (Exception ex)
            {
                return new AddDefaultReportDefinitionsResponse()
                {
                    Result = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        public AddSettlementTypesResponse AddSettlementTypes(AddSettlementTypesRequest request)
        {
            try
            {
                var applicationInstance = CheckPriviledgesAndGetApplicationInstance(request);
                if (!applicationInstance.MasterAdmin)
                    throw new Exception("Missing priviledges to MasterAdmin interface");
                _dbContext.SettlementTypes.RemoveRange(_dbContext.SettlementTypes);
                _dbContext.SettlementTypes.Add(new SettlementType() { Id = 1, TypeName = "Car rental", Debit = true, Factor = 1, Enabled = true });
                _dbContext.SettlementTypes.Add(new SettlementType() { Id = 2, TypeName = "1/2 TAX on fuel", Debit = false, Factor = 0.0934959349593496, Enabled = true });
                _dbContext.SettlementTypes.Add(new SettlementType() { Id = 3, TypeName = "Mandate contract", Debit = false, Factor = 1, Enabled = true });
                _dbContext.SettlementTypes.Add(new SettlementType() { Id = 4, TypeName = "Extra money", Debit = false, Factor = 1, Enabled = true });
                _dbContext.SettlementTypes.Add(new SettlementType() { Id = 5, TypeName = "Deducted money", Debit = true, Factor = 1, Enabled = true });
                _dbContext.SettlementTypes.Add(new SettlementType() { Id = 6, TypeName = "Insurance increasing", Debit = true, Factor = 1, Enabled = true });
                _dbContext.SaveChanges();
                return new AddSettlementTypesResponse()
                {
                    Result = true
                };
            }
            catch (Exception ex)
            {
                return new AddSettlementTypesResponse()
                {
                    Result = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        public string Ping(string ping)
        {
            return $"OK - {ping}";
        }

        public string Test()
        {
            return $"Tenants: {_dbContext.Tenants.Count()} Tenants:{_dbContext.Tenants.Count()}";
        }

        private ApplicationInstance CheckPriviledgesAndGetApplicationInstance(BaseRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.ApplicationInstanceId))
                throw new Exception("Invalid request");
            var applicationInstance = _dbContext.ApplicationInstances.FirstOrDefault(ai => request.ApplicationInstanceId.Equals(ai.Id));
            if (applicationInstance == null)
                throw new Exception($"ApplicationInstance {request.ApplicationInstanceId} does not exist");
            if (applicationInstance == null || !applicationInstance.MasterAdmin)
                throw new Exception("Missing priviledges to MasterAdmin interface");
            return applicationInstance;
        }
    }
}
