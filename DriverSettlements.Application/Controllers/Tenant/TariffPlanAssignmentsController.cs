using DriverSettlements.Common.DataContracts.Tenant;
using DriverSettlements.Common.Interfaces;
using DriverSettlements.Common.Models;

namespace DriverSettlements.App.Controllers.Tenant
{
    public class TariffPlanAssignmentsController : BaseListController<TariffPlanAssignment>
    {
        private int _yearNumber;
        private int _weekNumber;
        private readonly ApplicationController _applicationController;
        private readonly ITenantAsync _tenantService;
        public BillingWeekStatus BillingWeekStatus { get; set; } = BillingWeekStatus.NotExists;

        public TariffPlanAssignmentsController(ApplicationController applicationController) : base()
        {
            _applicationController = applicationController;
            _tenantService = ServiceHelper.GetService<ITenantAsync>();
        }

        public void SetYearAndWeek(int yearNumber, int weekNumber)
        {
            bool changed = false;
            if (_yearNumber != yearNumber)
            {
                _yearNumber = yearNumber;
                changed = true;
            }
            if (_weekNumber != weekNumber)
            {
                _weekNumber = weekNumber;
                changed = true;
            }
            if (changed)
                ForceDataRefresh = true;
        }
       
        public async Task<bool> CopyTariffPlanAssignmentsFromPreviousWeek()
        {
            var response = await _tenantService.CopyTariffPlanAssignmentsFromPreviousWeekAsync(new CopyTariffPlanAssignmentsFromPreviousWeekRequest()
            {
                ApplicationInstanceId = _applicationController.ApplicationProfile.Id,
                YearNumber = _yearNumber,
                WeekNumber = _weekNumber
            });
            if (!response.Result)
                throw new Exception(response.ErrorMessage);
            return response.Copied;
        }

        protected async override Task<List<TariffPlanAssignment>> GetDataFromService()
        {
            var response = await _tenantService.GetTariffPlanAssignmentsAsync(new GetTariffPlanAssignmentsRequest() { 
                ApplicationInstanceId = _applicationController.ApplicationProfile.Id,
                YearNumber = _yearNumber,
                WeekNumber = _weekNumber            
            });
            if (!response.Result)
                throw new Exception(response.ErrorMessage);
            BillingWeekStatus = response.BillingWeekStatus;
            return response.TariffPlanAssignments.OrderBy(tpa=>tpa.Driver.CompanyName).ToList();                
        }

        protected override async Task<TariffPlanAssignment> AddObjectToService(TariffPlanAssignment obj)
        {
            var response = await _tenantService.AddTariffPlanAssignmentAsync
            (
                new AddTariffPlanAssignmentRequest()
                {
                    ApplicationInstanceId = _applicationController.ApplicationProfile.Id,
                    TariffPlanAssignment = obj
                }
            );
            if (!response.Result)
                throw new Exception(response.ErrorMessage);
            obj.Id = response.TariffPlanAssignmentId.Value;
            return obj;
        }

        protected override async Task<TariffPlanAssignment> UpdateObjectInService(TariffPlanAssignment obj)
        {
            var response = await _tenantService.UpdateTariffPlanAssignmentAsync(
                new UpdateTariffPlanAssignmentRequest()
                {
                    ApplicationInstanceId = _applicationController.ApplicationProfile.Id,
                    TariffPlanAssignment = obj
                }
            );
            if (!response.Result)
                throw new Exception(response.ErrorMessage);
            return obj;
        }

        protected override async Task<TariffPlanAssignment> DeleteObjectInService(TariffPlanAssignment obj)
        {
            var response = await _tenantService.DeleteTariffPlanAssignmentAsync(
                new DeleteTariffPlanAssignmentRequest()
                {
                    ApplicationInstanceId = _applicationController.ApplicationProfile.Id,
                    TariffPlanAssignmentId  = obj.Id
                }
            );
            if (!response.Result)
                throw new Exception(response.ErrorMessage);
            return obj;
        }

        protected override List<TariffPlanAssignment> FilterData(string filerString)
        {
            if (string.IsNullOrWhiteSpace(filerString))
                return Objects.OrderBy(t => t.Driver.CompanyName).ToList();
            else
                return Objects.Where(t=> t.Driver.CompanyName.StartsWith(filerString,StringComparison.InvariantCultureIgnoreCase)
                    || t.Driver.CompanyName.StartsWith(filerString, StringComparison.InvariantCultureIgnoreCase) 
                    || t.Driver.CompanyName.Contains($" {filerString}",StringComparison.InvariantCultureIgnoreCase)).OrderBy(t => t.Driver.CompanyName).ToList();
        }
    }
}
