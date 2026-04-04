using DriverSettlements.Common.DataContracts.DriverInterface;
using DriverSettlements.Common.Helpers;
using DriverSettlements.Common.Interfaces;
using DriverSettlements.Common.Models;
using DriverSettlements.Database;
using DriverSettlements.WebAPI.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using BaseRequest = DriverSettlements.Common.DataContracts.BaseRequest;

namespace DriverSettlements.WebAPI.Services
{
    public class DriverService : IDriver
    {
        private readonly DatabaseContext _dbContext;
        private readonly ILogger<DriverService> _driverServiceLogger;
        private readonly PhotosConfiguration _photosConfiguration;

        public DriverService(DatabaseContext dbContext, ILogger<DriverService> driverServiceLogger, IOptions<PhotosConfiguration> photosConfiguration)
        {
            _dbContext = dbContext;
            _driverServiceLogger = driverServiceLogger;
            _photosConfiguration = photosConfiguration.Value;
        }

        public GetBillingWeekResponse GetBillingWeek(GetBillingWeekRequest request)
        {
            try
            {
                var driver = CheckPriviledgesAndGetApplicationInstance(request);
                var response = new GetBillingWeekResponse()
                {
                    Result = true,
                    BillingWeek = new BillingWeek()
                };
                var billingWeek = _dbContext.BillingWeeks.FirstOrDefault(w=>
                    w.TenantId.Equals(driver.TenantId) && w.YearNumber.Equals(request.YearNumber) && w.WeekNumber.Equals(request.WeekNumber));
                if (billingWeek!=null)
                    response.BillingWeek.Status = billingWeek.Status;
                var externalReports = _dbContext.ExternalReports.Where(r =>
                    r.TenantId.Equals(driver.TenantId) && r.YearNumber.Equals(request.YearNumber) && r.WeekNumber.Equals(request.WeekNumber)
                    && r.Status==ExternalReportStatus.Generated
                );
                var driverNames = driver.GetDriverNames();
                var reportsValues = new Dictionary<ExternalReportType, List<double>>();
                foreach (var externalReport in externalReports)
                {
                    var reportBody = ExternalReportBody.Deserialize(externalReport.ReportBody);
                    if (reportBody == null)
                        throw new Exception($"Invalid report body in {externalReport.ReportType} report for week {request.YearNumber}.{request.WeekNumber}");
                    ReportTemplatesHelper.SetReportBodyGlobalTemplates(_dbContext, reportBody, externalReport.ReportType, externalReport.DownloadDate);
                    ReportTemplatesHelper.SetReportBodyBasicReportTemplate(_dbContext, reportBody, externalReport.ReportType, externalReport.DownloadDate);
                    var values = reportBody.GetValues();
                    if (values != null)
                    {
                        var driverValues = values.Where(v => driverNames.Contains(v.Key)).ToList();
                        if (driverValues.Count > 1)
                        {
                            _driverServiceLogger.LogWarning($"Multiple values for driver names {string.Join(',', driverNames.Select(n => Strings.FirstCharToUpper(n, false, StringSplitOptions.None)))}");
                            List<double> tempValues = new List<double>();
                            for(int i=0; i< driverValues.First().Value.Count;i++)
                                tempValues.Add(driverValues.Sum(v => v.Value[i]));
                            reportsValues.Add(externalReport.ReportType, tempValues);
                        }
                        else if (driverValues.Count == 1)
                            reportsValues.Add(externalReport.ReportType, driverValues.First().Value);
                    }
                }
                response.BillingWeek.DriverReportsValues = reportsValues;
                response.BillingWeek.WeeklySettlementComponents = new Dictionary<string, double>();

                var defReportItem = ReportTemplatesHelper.GetReportTemplate(_dbContext, "Basic report", DateTime.Now);
                if (defReportItem != null && defReportItem.DefReportColumns != null)
                {
                    var columnNames = defReportItem.DefReportColumns.Skip(1).Select(c => c.ColumnName).ToList();
                    response.BillingWeek.DriverReportsValueNames = columnNames;
                    AddReportValues(response.BillingWeek.WeeklySettlementComponents, columnNames,reportsValues);
                }
                AddCarRentalCosts(response.BillingWeek.WeeklySettlementComponents, driver.Id, request.YearNumber, request.WeekNumber);
                AddFuelCosts(response.BillingWeek.WeeklySettlementComponents, driver.Id, request.YearNumber, request.WeekNumber);
                AddOtherSettlementItems(response.BillingWeek.WeeklySettlementComponents, driver.Id, request.YearNumber, request.WeekNumber);
                AddApplicationFee(response.BillingWeek.WeeklySettlementComponents, driver, request.YearNumber, request.WeekNumber);
                return response;
            }
            catch (Exception ex)
            {
                _driverServiceLogger.LogError(ex, ex.Message);
                return new GetBillingWeekResponse()
                {
                    Result = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        private void AddReportValues(Dictionary<string, double> weeklySettlementComponents, List<string> columnNames, Dictionary<ExternalReportType, List<double>> reportsValues)
        {
            if (reportsValues == null || reportsValues.Count < 1)
                return;
            var settlementType1 = _dbContext.SettlementTypes.FirstOrDefault(st => st.Id == 10);
            var settlementType2 = _dbContext.SettlementTypes.FirstOrDefault(st => st.Id == 9);
            if (settlementType1 == null || settlementType2 ==null)
                return;
            var columnIdx = columnNames.IndexOf("Konto spółki");
            if (columnIdx > -1)
            {
                var value = reportsValues.Sum(v => v.Value[columnIdx]);
                if (settlementType1.Debit)
                    value = -1.0 * value;
                weeklySettlementComponents.Add(settlementType1.TypeName,  value * settlementType1.Factor);
            }
            columnIdx = columnNames.IndexOf("Utarg brutto");
            if (columnIdx > -1)
            {
                var value = reportsValues.Sum(v => v.Value[columnIdx]);
                if (settlementType2.Debit)
                    value = -1.0 * value;
                weeklySettlementComponents.Add(settlementType2.TypeName, value * settlementType2.Factor);
            }
        }

        private void AddCarRentalCosts(Dictionary<string, double> weeklySettlementComponents, int driverId, int yearNumber, int weekNumber)
        {
            var settlementType = _dbContext.SettlementTypes.FirstOrDefault(st => st.Id == 1);
            if (settlementType == null)
                return;
            var tariffPlanAssignments = _dbContext.TariffPlanAssignments.Where(fb => fb.DriverId.Equals(driverId) && fb.YearNumber.Equals(yearNumber) && fb.WeekNumber.Equals(weekNumber)).Include(fb=>fb.TariffPlan);
            foreach(var tariffPlanAssignment in tariffPlanAssignments)
            {
                var carRentalCost = tariffPlanAssignment.TariffPlan?.WeeklyRate;
                if (settlementType.Debit)
                    carRentalCost = -1 * carRentalCost;
                if (!carRentalCost.HasValue)
                    throw new Exception("Tariff plan assignment without tariff plan");
                var numberOfDays = 0;
                if (tariffPlanAssignment.Monday)
                    numberOfDays++;
                if (tariffPlanAssignment.Tuesday)
                    numberOfDays++;
                if (tariffPlanAssignment.Wednesday)
                    numberOfDays++;
                if (tariffPlanAssignment.Thursday)
                    numberOfDays++;
                if (tariffPlanAssignment.Friday)
                    numberOfDays++;
                if (tariffPlanAssignment.Saturday)
                    numberOfDays++;
                if (tariffPlanAssignment.Sunday)
                    numberOfDays++;
                weeklySettlementComponents.Add($"{settlementType.TypeName}|({tariffPlanAssignment.TariffPlan?.Name})|{tariffPlanAssignment.Id}", carRentalCost.Value * settlementType.Factor * numberOfDays/7);
            }
            var settlementItems = _dbContext.SettlementItems.Where(si => si.DriverId.Equals(driverId) && si.YearNumber.Equals(yearNumber) && si.WeekNumber.Equals(weekNumber)
                && si.SettlementTypeId.Equals(settlementType.Id));
            if (settlementItems.Any())
            {
                var carRentalCost = settlementItems.Sum(si => si.Value);
                if (settlementType.Debit)
                    carRentalCost = -1 * carRentalCost;
                weeklySettlementComponents.Add($"{settlementType.TypeName}|(others)", carRentalCost * settlementType.Factor);
            }
        }

        private void AddFuelCosts(Dictionary<string,double> weeklySettlementComponents, int driverId, int yearNumber, int weekNumber)
        {
            var settlements = _dbContext.SettlementTypes.Where(st => st.Id == 2 || st.Id == 25).ToDictionary(st => st.TypeName, st => st);
            if (settlements == null || settlements.Count < 1)
                throw new Exception("Missing fuel settlement definitions");
            var defReportItem = ReportTemplatesHelper.GetReportTemplate(_dbContext, "Fuel report", Date.GetFirstDateOfWeek(yearNumber, weekNumber));
            if (defReportItem == null)
                throw new Exception("Missing fuel report definition");
            var defValues = defReportItem?.DefReportColumns?.Where(drc => drc.DefValue != null && !settlements.ContainsKey(drc.DefValue));
            if (defValues != null && defValues.Count() > 0)
            {
                var missingSettlementTypes = string.Join(", ", defValues.Select(dv => dv.DefValue));
                throw new Exception($"Missing settlement types for fuel costs: {missingSettlementTypes}");
            }
            var fuelBillTypes = defReportItem?.DefReportColumns?.Skip(1).ToDictionary(drc => drc.ColumnName, drc => settlements[drc.DefValue]);
            var defaultFuelBillType = settlements[defReportItem?.DefReportColumns?.First().DefValue];
            var settlementValues = settlements.Keys.ToDictionary(k => k, k => 0.0);

            var fuelBills = _dbContext.FuelBills.Where(fb => fb.DriverId.Equals(driverId) && fb.YearNumber.Equals(yearNumber) && fb.WeekNumber.Equals(weekNumber)
                    && fb.Status == FuelBillStatus.Acccepted);
            if (fuelBills != null && fuelBills.Count() > 0)
            {
                foreach (var fuelBill in fuelBills)
                {
                    var settlementType = GetSettlementType(fuelBill.ReferenceNumber);
                    if (settlementType != null)
                    {
                        var fuelCost = fuelBill.Cost * settlementType.Factor;
                        if (settlementType.Debit)
                            fuelCost = -1 * fuelCost;
                        settlementValues[settlementType.TypeName] += fuelCost;
                    }
                }
                foreach (var settlementValueKvp in settlementValues)
                {
                    if (!settlementValueKvp.Value.Equals(0.0))
                        weeklySettlementComponents.Add(settlementValueKvp.Key, settlementValueKvp.Value);
                }
            }

            SettlementType? GetSettlementType(string referenceNumber)
            {
                var fuelBillTypeCode = referenceNumber.Substring(0, 1).ToUpper();
                if (fuelBillTypes != null && fuelBillTypes.TryGetValue(fuelBillTypeCode, out SettlementType? fuelBillType))
                    return fuelBillType;
                return defaultFuelBillType;
            }
        }

        private void AddOtherSettlementItems(Dictionary<string, double> weeklySettlementComponents, int driverId, int yearNumber, int weekNumber)
        {
            var settlementItems = _dbContext.SettlementItems.Where(si => si.DriverId.Equals(driverId)
                && si.YearNumber.Equals(yearNumber) && si.WeekNumber.Equals(weekNumber)
                && si.SettlementType!=null && si.SettlementType.Id!=1 && si.SettlementType.Id!=2
                && si.SettlementType.Id != 9 && si.SettlementType.Id != 10).Include(si=>si.SettlementType);
            if (settlementItems == null || settlementItems.Count() == 0)
                return;
            foreach(var settlementItem in settlementItems)
            {
                if (settlementItem.SettlementType == null) continue;
                var value = settlementItem.Value;
                if (settlementItem.SettlementType.Debit)
                    value = -1 * value;
                weeklySettlementComponents.Add(settlementItem.SettlementType.TypeName, value * settlementItem.SettlementType.Factor);
            }
        }

        private void AddApplicationFee(Dictionary<string, double> weeklySettlementComponents, Driver driver, int yearNumber, int weekNumber)
        {
            var settlementType = _dbContext.SettlementTypes.FirstOrDefault(st => st.Id == 8);
            if (settlementType == null)
                return;
            var tenant = _dbContext.Tenants.FirstOrDefault(t => t.Id == driver.TenantId);
            if (tenant == null)
                return;
            var billingWeek = _dbContext.BillingWeeks.FirstOrDefault(bw => bw.TenantId.Equals(tenant.Id)
                && bw.YearNumber.Equals(yearNumber) && bw.WeekNumber.Equals(weekNumber));
            if (billingWeek == null || billingWeek.Status != BillingWeekStatus.Closed || !billingWeek.ApplicationFeeUnitPrice.HasValue || !billingWeek.NumberOfDrivers.HasValue)
                return;
            if (tenant.ApplicationFeeChargeDriver && driver.ApplicationFeeChargeDriver && weeklySettlementComponents.Any(wsc => !wsc.Value.Equals(0.0)))
                weeklySettlementComponents.Add(settlementType.TypeName, billingWeek.ApplicationFeeUnitPrice.Value * (settlementType.Debit ? -1.0 : 1.0) * settlementType.Factor);
        }


        public GetDriverFuelBillsResponse GetDriverFuelBills(GetDriverFuelBillsRequest request)
        {
            try
            {
                var driver = CheckPriviledgesAndGetApplicationInstance(request);
                var billingWeek = _dbContext.BillingWeeks.FirstOrDefault(bw => bw.TenantId.Equals(driver.TenantId)
                    && bw.YearNumber.Equals(request.YearNumber) && bw.WeekNumber.Equals(request.WeekNumber));
                return new GetDriverFuelBillsResponse()
                {
                    Result = true,
                    BillingWeekStatus = billingWeek == null ? BillingWeekStatus.NotExists : billingWeek.Status,
                    FuelBills = _dbContext.FuelBills.Where(b => b.DriverId.Equals(driver.Id) 
                        && b.YearNumber.Equals(request.YearNumber) && b.WeekNumber.Equals(request.WeekNumber)).ToList()
                };
            }
            catch (Exception ex)
            {
                _driverServiceLogger.LogError(ex, ex.Message);
                return new GetDriverFuelBillsResponse()
                {
                    Result = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        public AddDriverFuelBillResponse AddDriverFuelBill(AddDriverFuelBillRequest request)
        {
            try
            {
                var driver = CheckPriviledgesAndGetApplicationInstance(request);
                request.FuelBill.DriverId = driver.Id;
                request.FuelBill.TenantId =driver.TenantId;
                _dbContext.FuelBills.Add(request.FuelBill);
                _dbContext.SaveChanges();
                return new AddDriverFuelBillResponse()
                {
                    Result = true,
                    FuelBillId = request.FuelBill.Id
                };
            }
            catch (Exception ex)
            {
                _driverServiceLogger.LogError(ex, ex.Message);
                return new AddDriverFuelBillResponse()
                {
                    Result = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        public UpdateDriverFuelBillResponse UpdateDriverFuelBill(UpdateDriverFuelBillRequest request)
        {
            try
            {
                var driver = CheckPriviledgesAndGetApplicationInstance(request);
                var fuelBill = _dbContext.FuelBills.FirstOrDefault(b => b.Id.Equals(request.FuelBill.Id));
                if (fuelBill == null)
                    throw new Exception($"Fuel bill with ID={request.FuelBill.Id} does not exist");
                if (!fuelBill.DriverId.Equals(driver.Id))
                    throw new Exception($"Appeared problem with ownership of fuel bill with ID={request.FuelBill.Id}");
                fuelBill.Date = request.FuelBill.Date;
                fuelBill.ReferenceNumber = request.FuelBill.ReferenceNumber;
                fuelBill.Cost = request.FuelBill.Cost;
                fuelBill.Status = request.FuelBill.Status;
                _dbContext.SaveChanges();
                return new UpdateDriverFuelBillResponse()
                {
                    Result = true,
                };
            }
            catch (Exception ex)
            {
                _driverServiceLogger.LogError(ex, ex.Message);
                return new UpdateDriverFuelBillResponse()
                {
                    Result = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        public DeleteDriverFuelBillResponse DeleteDriverFuelBill(DeleteDriverFuelBillRequest request)
        {
            try
            {
                var driver = CheckPriviledgesAndGetApplicationInstance(request);
                var fuelBill = _dbContext.FuelBills.FirstOrDefault(b => b.Id.Equals(request.FuelBillId));
                if (fuelBill == null)
                    throw new Exception($"Fuel bill with ID={request.FuelBillId} does not exist");
                if (!fuelBill.DriverId.Equals(driver.Id))
                    throw new Exception($"Appeared problem with ownership of fuel bill with ID={request.FuelBillId}");
                fuelBill.Status = FuelBillStatus.Deleted;
                _dbContext.SaveChanges();
                return new DeleteDriverFuelBillResponse()
                {
                    Result = true,
                };
            }
            catch (Exception ex)
            {
                _driverServiceLogger.LogError(ex, ex.Message);
                return new DeleteDriverFuelBillResponse()
                {
                    Result = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        public UploadDriverFuelBillPhotoResponse UploadDriverFuelBillPhoto(UploadDriverFuelBillPhotoRequest request)
        {
            try
            {
                var driver = CheckPriviledgesAndGetApplicationInstance(request);
                var fuelBill = _dbContext.FuelBills.FirstOrDefault(b => b.Id.Equals(request.FuelBillId));
                if (fuelBill == null)
                    throw new Exception($"Fuel bill with ID={request.FuelBillId} does not exist");
                if (!fuelBill.DriverId.Equals(driver.Id))
                    throw new Exception($"Appeared problem with ownership of fuel bill with ID={request.FuelBillId}");
                var photosFolder = Path.Combine(
                        _photosConfiguration.PhotosFolder,
                        $"{driver.TenantId.ToString("#000")}",
                        $"{fuelBill.YearNumber.ToString("#0000")}.{fuelBill.WeekNumber.ToString("#00")}");
                var photoFilePath = Path.Combine(
                    photosFolder,$"{fuelBill.Id}.jpeg");
                if (!Directory.Exists(photosFolder))
                    Directory.CreateDirectory(photosFolder);
                using (var fileStream = new FileStream(photoFilePath, FileMode.Create, FileAccess.Write))
                    fileStream.Write(request.PhotoContent, 0, request.PhotoContent.Length);
                return new UploadDriverFuelBillPhotoResponse()
                {
                    Result = true,
                };
            }
            catch (Exception ex)
            {
                _driverServiceLogger.LogError(ex, ex.Message);
                return new UploadDriverFuelBillPhotoResponse()
                {
                    Result = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        public DownloadDriverFuelBillPhotoResponse DownloadDriverFuelBillPhoto(DownloadDriverFuelBillPhotoRequest request)
        {
            try
            {
                var driver = CheckPriviledgesAndGetApplicationInstance(request);
                var fuelBill = _dbContext.FuelBills.FirstOrDefault(fb => fb.Id == request.FuelBillId);
                if (fuelBill == null)
                    throw new Exception($"Fuel bill with ID={request.FuelBillId} does not exist");
                if (!fuelBill.DriverId.Equals(driver.Id))
                        throw new Exception($"Appeared problem with ownership of fuel bill with ID={request.FuelBillId}");
                var photosFolder = Path.Combine(
                        _photosConfiguration.PhotosFolder,
                        $"{fuelBill.TenantId.ToString("#000")}",
                        $"{fuelBill.YearNumber.ToString("#0000")}.{fuelBill.WeekNumber.ToString("#00")}");
                var photoFilePath = Path.Combine(
                    photosFolder, $"{fuelBill.Id}.jpeg");
                if (!File.Exists(photoFilePath))
                    throw new Exception($"Image file of fuel bill with ID={request.FuelBillId} does not exist");
                using (Image image = Image.Load(photoFilePath))
                {
                    if (image.Width < image.Height && request.RequestedImageWidth > request.RequestedImageHeight
                        || image.Width > image.Height && request.RequestedImageWidth < request.RequestedImageHeight)
                    {
                        image.Mutate(ctx => ctx.Rotate(RotateMode.Rotate90));
                    }
                    using (var ms = new MemoryStream())
                    {
                        image.SaveAsJpeg(ms);
                        return new DownloadDriverFuelBillPhotoResponse()
                        {
                            PhotoContent = ms.ToArray(),
                            Result = true
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                _driverServiceLogger.LogError(ex, ex.Message);
                return new DownloadDriverFuelBillPhotoResponse()
                {
                    Result = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        public string Ping(string ping)
        {
            throw new NotImplementedException();
        }

        public string Test()
        {
            throw new NotImplementedException();
        }

        private Driver CheckPriviledgesAndGetApplicationInstance(BaseRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.ApplicationInstanceId))
                throw new Exception("Invalid request");
            var applicationInstance = _dbContext.ApplicationInstances.FirstOrDefault(ai => request.ApplicationInstanceId.Equals(ai.Id));
            if (applicationInstance == null)
                throw new Exception($"ApplicationInstance {request.ApplicationInstanceId} does not exist");
            if (applicationInstance.DriverId == null || !applicationInstance.DriverId.HasValue)
                throw new Exception("Missing priviledges to Driver interface");
            var driver = _dbContext.Drivers.FirstOrDefault(t => t.Id.Equals(applicationInstance.DriverId.Value));
            if (driver == null)
                throw new Exception($"Driver related to {request.ApplicationInstanceId} does not exist");
            if (driver.Blocked)
                throw new Exception($"Driver {driver.CompanyName} is blocked");
            if (driver.Deleted)
                throw new Exception($"Driver {driver.CompanyName} does not exist");
            return driver;
        }
    }
}
