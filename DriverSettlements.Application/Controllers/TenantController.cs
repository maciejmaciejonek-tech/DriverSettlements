using DriverSettlements.App.Controllers;
using DriverSettlements.App.Controllers.Tenant;
using DriverSettlements.App.Models;
using DriverSettlements.Common.DataContracts.Tenant;
using DriverSettlements.Common.Interfaces;
using DriverSettlements.Common.Interfaces.Tenant;
using DriverSettlements.Common.Models;

namespace DriverSettlements.App
{
    public class TenantController
    {
        public Tenant Tenant;
        public readonly WeekController WeekController;
        public readonly FuelBillsController FuelBillsController;
        public readonly DriversController DriversController;
        public readonly TariffPlansController TariffPlansController;
        public readonly TariffPlanAssignmentsController TariffPlanAssignmentsController;
        public readonly SettlementItemsController SettlementItemsController;
        public readonly SettlementTypesController SettlementTypesController;
        private readonly ApplicationController _applicationController;
        private readonly ITenantAsync _tenantService;

        public TenantController(ApplicationController applicationController)
        {
            WeekController = new WeekController();
            FuelBillsController = new FuelBillsController(applicationController);
            DriversController = new DriversController(applicationController);
            TariffPlansController = new TariffPlansController(applicationController);
            TariffPlanAssignmentsController = new TariffPlanAssignmentsController(applicationController);
            SettlementItemsController = new SettlementItemsController(applicationController);
            SettlementTypesController = new SettlementTypesController(applicationController);
            _applicationController = applicationController;
            _tenantService = ServiceHelper.GetService<ITenantAsync>();
        }

        public async Task<bool> EnsureTenant()
        {
            if (Tenant == null)
            {
                var response = await _tenantService.GetTenantMetadataAsync(new GetTenantMetadataRequest()
                {
                    ApplicationInstanceId = _applicationController.ApplicationProfile.Id
                });
                if (!response.Result)
                    throw new Exception(response.ErrorMessage);
                Tenant = response.Tenant;
            }
            return true;
        }

        public async Task<bool> SaveTenant()
        {
            var response = await _tenantService.SaveTenantMetadataAsync(new SaveTenantMetadataRequest()
            {
                ApplicationInstanceId = _applicationController.ApplicationProfile.Id,
                Tenant = Tenant
            });
            if (!response.Result)
                throw new Exception(response.ErrorMessage);
            return true;
        }

        public async Task<bool> CheckDownloadingReport(ExternalReportType reportType)
        {
            var report = await DownloadExternalReportForLastWeek(reportType, true);
            if (report != null)
                return true;
            return false;
        }

        public async Task<ExternalReportMetadata> DownloadExternalReportForLastWeek(ExternalReportType reportType, bool overwrite = false)
        {
            var date = DateTime.Now.AddDays(-7);
            int yearNumber = date.Year;
            int weekNumber = Common.Helpers.Date.GetWeekNumber(date);
            return await DownloadExternalReport(yearNumber, weekNumber, reportType, overwrite);
        }

        public async Task<ExternalReportMetadata> DownloadExternalReport(int yearNumber, int weekNumber, ExternalReportType reportType, bool overwrite = false)
        {
            var response = await _tenantService.DownloadExternalReportAsync(new DownloadExternalReportRequest
            {
                ApplicationInstanceId = _applicationController.ApplicationProfile.Id,
                ReportType = reportType,
                YearNumber = yearNumber,
                WeekNumber = weekNumber,
                Overwrite = overwrite
            });
            if (!response.Result)
                throw new Exception(response.ErrorMessage); 
            return response.Report;
        }

        public async Task<ExternalReportMetadata> UploadExternalReport(int yearNumber, int weekNumber, ExternalReportType reportType, byte[] reportFileContent)
        {
            var response = await _tenantService.UploadExternalReportAsync(new UploadExternalReportRequest
            {
                ApplicationInstanceId = _applicationController.ApplicationProfile.Id,
                ReportType = reportType,
                YearNumber = yearNumber,
                WeekNumber = weekNumber,
                ReportFileContent = reportFileContent
            });
            if (!response.Result)
                throw new Exception(response.ErrorMessage);
            return response.Report;

        }

        public async Task<bool> UploadExternalReport(TenantWeek tenantWeek, ExternalReportType reportType, byte[] reportFileContent)
        {
            var report = await _applicationController.TenantController.UploadExternalReport(
                tenantWeek.YearNumber, tenantWeek.WeekNumber, reportType, reportFileContent);
            var externalReport = tenantWeek.ExternalReports.FirstOrDefault(er =>
                er.YearNumber.Equals(tenantWeek.YearNumber) && er.WeekNumber.Equals(tenantWeek.WeekNumber) && er.ReportType == reportType);
            if (externalReport != null)
                tenantWeek.ExternalReports.Remove(externalReport);
            tenantWeek.ExternalReports.Add(report);
            return true;
        }

        public async Task<bool> LoadWeeklyExternalReportsMetadata(TenantWeek week, Driver filerDriver)
        {
            var request = new GetWeeklyExternalReportsMetadataRequest()
            {
                ApplicationInstanceId = _applicationController.ApplicationProfile.Id,
                YearNumber = week.YearNumber,
                WeekNumber = week.WeekNumber
            };
            if (filerDriver!=null)
                request.DriverId = filerDriver.Id;
            var response = await _tenantService.GetWeeklyExternalReportsMetadataAsync(request);
            if (!response.Result)
                throw new Exception(response.ErrorMessage);
            week.ExternalReports = response.Reports;
            week.TenantReportsValues = response.TenantReportsValues;
            week.TenantReportsValueNames = response.TenantReportsValueNames;
            week.WeeklySettlementComponents = response.WeeklySettlementComponents;
            week.BillingWeekStatus = response.BillingWeekStatus;
            week.WeeklySettlementsInfo = response.WeeklySettlementsInfo;
            return true;
        }

        public async Task<Report> GetBasicReport(int yearNumber, int weekNumber)
        {
            var response = await _tenantService.GetBasicReportAsync(new GetBasicReportRequest()
            {
                ApplicationInstanceId = _applicationController.ApplicationProfile.Id,
                YearNumber = yearNumber,
                WeekNumber = weekNumber
            });
            if (!response.Result)
                throw new Exception(response.ErrorMessage);
            return response.Report;
        }

        public async Task<Report> GetCustomReport(List<Week> weeks)
        {
            var response = await _tenantService.GetCustomReportAsync(new GetCustomReportRequest()
            {
                ApplicationInstanceId = _applicationController.ApplicationProfile.Id,
                Weeks = weeks
            });
            if (!response.Result)
                throw new Exception(response.ErrorMessage);
            return response.Report;
        }

        public async Task<List<FuelBill>> GetTenantFuelBills(int yearNumber, int weekNumber,int? driverId=null)
        {
            var response = await _tenantService.GetTenantFuelBillsAsync(new GetTenantFuelBillsRequest()
            {
                ApplicationInstanceId = _applicationController.ApplicationProfile.Id,
                YearNumber = yearNumber,
                WeekNumber = weekNumber,
                DriverId = driverId
            });
            if (!response.Result)
                throw new Exception(response.ErrorMessage);
            return response.FuelBills;
        }

        public async Task<bool> UpdateTenantFuelBills(FuelBill fuelBill)
        {
            var response = await _tenantService.UpdateTenantFuelBillAsync(new UpdateTenantFuelBillRequest()
            {
                ApplicationInstanceId = _applicationController.ApplicationProfile.Id,
                FuelBill = fuelBill
            });
            if (!response.Result)
                throw new Exception(response.ErrorMessage);
            return response.Result;
        }

        public async Task<GetDriverFuelBillsInfosResponse> GetDriverFuelBillsInfos(int yearNumber, int weekNumber)
        {
            var response = await _tenantService.GetDriverFuelBillsInfosAsync(new GetDriverFuelBillsInfosRequest()
            {
                ApplicationInstanceId = _applicationController.ApplicationProfile.Id,
                YearNumber = yearNumber,
                WeekNumber = weekNumber
            });
            if (!response.Result)
                throw new Exception(response.ErrorMessage);
            return response;
        }

        public async Task<byte[]> DownloadTenantFuelBillPhoto(int fuelBillId, double requestedImageWidth, double requestedImageHeight)
        {
            var response = await _tenantService.DownloadTenantFuelBillPhotoAsync(new DownloadTenantFuelBillPhotoRequest()
            {
                ApplicationInstanceId = _applicationController.ApplicationProfile.Id,
                FuelBillId = fuelBillId,
                RequestedImageWidth = requestedImageWidth,
                RequestedImageHeight = requestedImageHeight
            });
            if (!response.Result)
                throw new Exception(response.ErrorMessage);
            return response.PhotoContent;
        }

        public async Task<bool> ConfirmBillingWeek(int yearNumber, int weekNumber)
        {
            var response = await _tenantService.ConfirmBillingWeekAsync(new ConfirmBillingWeekRequest()
            {
                ApplicationInstanceId = _applicationController.ApplicationProfile.Id,
                YearNumber = yearNumber,
                WeekNumber = weekNumber
            });
            if (!response.Result)
                throw new Exception(response.ErrorMessage);
            return response.Result;
        }

        public async Task<bool> ReopenBillingWeek(int yearNumber, int weekNumber)
        {
            var response = await _tenantService.ReopenBillingWeekAsync(new ReopenBillingWeekRequest()
            {
                ApplicationInstanceId = _applicationController.ApplicationProfile.Id,
                YearNumber = yearNumber,
                WeekNumber = weekNumber
            });
            if (!response.Result)
                throw new Exception(response.ErrorMessage);
            return response.Result;
        }

        public async Task<double> GetDriverDepositValue(int driverId)
        {
            var response = await _tenantService.GetDriverDepositValueAsync(new GetDriverDepositValueRequest()
            {
                ApplicationInstanceId = _applicationController.ApplicationProfile.Id,
                DriverId = driverId
            });
            if (!response.Result)
                throw new Exception(response.ErrorMessage);
            return response.DepositValue.Value;
        }

        public void ClearControllers()
        {
            Tenant = null;
            DriversController.ClearController();
            FuelBillsController.ClearController();
            WeekController.ClearController();
            TariffPlansController.ClearController();
            TariffPlanAssignmentsController.ClearController();
            SettlementItemsController.ClearController();
            SettlementTypesController.ClearController();
        }
    }
}
