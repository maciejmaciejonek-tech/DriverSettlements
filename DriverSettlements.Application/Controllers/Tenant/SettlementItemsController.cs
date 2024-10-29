using DriverSettlements.App.Helpers;
using DriverSettlements.Common.DataContracts.Tenant;
using DriverSettlements.Common.Interfaces;
using DriverSettlements.Common.Models;

namespace DriverSettlements.App.Controllers.Tenant
{
    public class SettlementItemsController : BaseListController<SettlementItem>
    {
        public BillingWeekStatus BillingWeekStatus { get; private set; } = BillingWeekStatus.NotExists;
        private int _yearNumber;
        private int _weekNumber;
        private readonly ApplicationController _applicationController;
        private readonly ITenantAsync _tenantService;

        public SettlementItemsController(ApplicationController applicationController) : base()
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
       
        public async Task<bool> CopySettlementItemsFromPreviousWeek()
        {
            var response = await _tenantService.CopySettlementItemsFromPreviousWeekAsync(new CopySettlementItemsFromPreviousWeekRequest()
            {
                ApplicationInstanceId = _applicationController.ApplicationProfile.Id,
                YearNumber = _yearNumber,
                WeekNumber = _weekNumber
            });
            if (!response.Result)
                throw new Exception(response.ErrorMessage);
            return response.Copied;
        }

        protected async override Task<List<SettlementItem>> GetDataFromService()
        {
            var response = await _tenantService.GetSettlementItemsAsync(new GetSettlementItemsRequest() { 
                ApplicationInstanceId = _applicationController.ApplicationProfile.Id,
                YearNumber = _yearNumber,
                WeekNumber = _weekNumber            
            });
            if (!response.Result)
                throw new Exception(response.ErrorMessage);
            BillingWeekStatus = response.BillingWeekStatus;
            return response.SettlementItems.Select(Translate.TranslateSettlementItem).OrderBy(tpa=>tpa.Driver.CompanyName).ToList();                
        }

        protected override async Task<SettlementItem> AddObjectToService(SettlementItem obj)
        {
            var response = await _tenantService.AddSettlementItemAsync
            (
                new AddSettlementItemRequest()
                {
                    ApplicationInstanceId = _applicationController.ApplicationProfile.Id,
                    SettlementItem = obj
                }
            );
            if (!response.Result)
                throw new Exception(response.ErrorMessage);
            obj.Id = response.SettlementItemId.Value;
            return obj;
        }

        protected override async Task<SettlementItem> UpdateObjectInService(SettlementItem obj)
        {
            var response = await _tenantService.UpdateSettlementItemAsync(
                new UpdateSettlementItemRequest()
                {
                    ApplicationInstanceId = _applicationController.ApplicationProfile.Id,
                    SettlementItem = obj
                }
            );
            if (!response.Result)
                throw new Exception(response.ErrorMessage);
            return obj;
        }

        protected override async Task<SettlementItem> DeleteObjectInService(SettlementItem obj)
        {
            var response = await _tenantService.DeleteSettlementItemAsync(
                new DeleteSettlementItemRequest()
                {
                    ApplicationInstanceId = _applicationController.ApplicationProfile.Id,
                    SettlementItemId = obj.Id
                }
            );
            if (!response.Result)
                throw new Exception(response.ErrorMessage);
            return obj;
        }

        protected override List<SettlementItem> FilterData(string filerString)
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
