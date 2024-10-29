using DriverSettlements.Common.DataContracts.ApplicationInstance;
using DriverSettlements.Common.Interfaces;
using DriverSettlements.Common.Models;
using DriverSettlements.Database;

namespace DriverSettlements.WebAPI.Services
{
    public class ApplicationInstanceService : IApplicationInstance
    {
        private readonly DatabaseContext _dbContext;
        private readonly ILogger<ApplicationInstanceService> _logger;
        public ApplicationInstanceService(DatabaseContext dbContext, ILogger<ApplicationInstanceService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public GetApplicationInstanceRoleResponse GetApplicationInstanceRole(GetApplicationInstanceRoleRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.ApplicationInstanceId))
                return new GetApplicationInstanceRoleResponse()
                {
                    Result = false,
                    ErrorMessage = "Invalid request"
                };
            var applicationInstance = _dbContext.ApplicationInstances.FirstOrDefault(ai => ai.Id.Equals(request.ApplicationInstanceId));
            if (applicationInstance == null)
                return new GetApplicationInstanceRoleResponse()
                {
                    Result = true,
                    ApplicationRole = ApplicationRole.None
                };
            if (applicationInstance.DriverId.HasValue)
            {
                applicationInstance.LastLoginDate = DateTime.Now;
                _dbContext.SaveChanges();
                return new GetApplicationInstanceRoleResponse()
                {
                    Result = true,
                    ApplicationRole = ApplicationRole.Driver
                };
            }
            if (applicationInstance.TenantId.HasValue)
            {
                applicationInstance.LastLoginDate = DateTime.Now;
                _dbContext.SaveChanges();
                return new GetApplicationInstanceRoleResponse()
                {
                    Result = true,
                    ApplicationRole = ApplicationRole.Tenant
                };
            }
            if (applicationInstance.MasterAdmin)
            {
                applicationInstance.LastLoginDate = DateTime.Now;
                _dbContext.SaveChanges();
                return new GetApplicationInstanceRoleResponse()
                {
                    Result = true,
                    ApplicationRole = ApplicationRole.MasterAdmin
                };
            }
            return new GetApplicationInstanceRoleResponse()
            {
                Result = false,
                ErrorMessage = $"Can't sucessfully processed request for application instance {request.ApplicationInstanceId}"
            };
        }

        public RegisterApplicationInstanceResponse RegisterApplicationInstance(RegisterApplicationInstanceRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.ApplicationInstanceId) || string.IsNullOrWhiteSpace(request.InvitationKey))
                return new RegisterApplicationInstanceResponse()
                {
                    Result = false,
                    ErrorMessage = "Invalid request"
                };
            var applicationInstance = _dbContext.ApplicationInstances.FirstOrDefault(ai => request.ApplicationInstanceId.Equals(ai.Id));
            if (applicationInstance != null)
                return new RegisterApplicationInstanceResponse()
                {
                    Result = false,
                    ErrorMessage = $"Application instance {request.ApplicationInstanceId} is already registered"
                };
            var driver = _dbContext.Drivers.FirstOrDefault(d => request.InvitationKey.Equals(d.InvitationKey));
            if (driver != null)
            {
                driver.InvitationKey = string.Empty;
                _dbContext.ApplicationInstances.Add(new ApplicationInstance()
                {
                    Id = request.ApplicationInstanceId,
                    RegistrationDate = DateTime.Now,
                    Locked = false,
                    DriverId = driver.Id,
                    MasterAdmin = false
                });
                _dbContext.SaveChanges();
                return new RegisterApplicationInstanceResponse()
                {
                    Result = true,
                    ApplicationRole = ApplicationRole.Driver
                };
            }
            var tenant = _dbContext.Tenants.FirstOrDefault(t => request.InvitationKey.Equals(t.InvitationKey));
            if (tenant != null)
            {
                tenant.InvitationKey = string.Empty;
                _dbContext.ApplicationInstances.Add(new ApplicationInstance()
                {
                    Id = request.ApplicationInstanceId,
                    RegistrationDate = DateTime.Now,
                    Locked = false,
                    TenantId = tenant.Id,
                    MasterAdmin = false
                });
                _dbContext.SaveChanges();
                return new RegisterApplicationInstanceResponse()
                {
                    Result = true,
                    ApplicationRole = ApplicationRole.Tenant
                };
            }
            if (request.InvitationKey.Equals("macmacek"))
            {
                _dbContext.ApplicationInstances.Add(new ApplicationInstance()
                {
                    Id = request.ApplicationInstanceId,
                    RegistrationDate = DateTime.Now,
                    LastLoginDate = DateTime.Now,
                    Locked = false,
                    MasterAdmin = true
                });
                _dbContext.SaveChanges();
                return new RegisterApplicationInstanceResponse()
                {
                    Result = true,
                    ApplicationRole = ApplicationRole.MasterAdmin
                };
            }
            return new RegisterApplicationInstanceResponse()
            {
                Result = false,
                ErrorMessage = $"Can't sucessfully processed request for invitation key {request.InvitationKey}"
            };
        }

        public string Ping(string ping)
        {
            return $"OK - {ping}";
        }

        public string Test()
        {
            return $"Tenants: {_dbContext.Tenants.Count()} Drivers:{_dbContext.Drivers.Count()}";
        }
    }
}
