using DriverSettlements.Common.DataContracts.DriverInterface;
using DriverSettlements.Common.DataContracts.Tenant;
using DriverSettlements.Common.Helpers;
using DriverSettlements.Common.Interfaces;
using DriverSettlements.Common.Interfaces.Tenant;
using DriverSettlements.Common.Models;
using DriverSettlements.Database;
using DriverSettlements.WebAPI.Configuration;
using DriverSettlements.WebAPI.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Reflection.PortableExecutable;
using BaseRequest = DriverSettlements.Common.DataContracts.BaseRequest;

namespace DriverSettlements.WebAPI.Services
{
    public class TenantService : ITenant
    {
        private readonly DatabaseContext _dbContext;
        private readonly ILogger<TenantService> _tenantServiceLogger;
        private readonly ExternalReportsConfiguration _externalReportsConfiguration;
        private readonly PhotosConfiguration _photosConfiguration;

        public TenantService(DatabaseContext dbContext, ILogger<TenantService> tenantServiceLogger, IOptions<ExternalReportsConfiguration> externalReportsConfiguration, IOptions<PhotosConfiguration> photosConfiguration)
        {
            _dbContext = dbContext;
            _tenantServiceLogger = tenantServiceLogger;
            _externalReportsConfiguration = externalReportsConfiguration.Value;   
            _photosConfiguration = photosConfiguration.Value;
        }

        public GetDriversResponse GetDrivers(GetDriversRequest request)
        {
            try
            {
                var tenant = CheckPriviledgesAndGetApplicationInstance(request);
                return new GetDriversResponse()
                {
                    Result = true,
                    Drivers = request.IncludeDeleted
                                ? _dbContext.Drivers.Where(d => d.TenantId.Equals(tenant.Id)).ToList()
                                : _dbContext.Drivers.Where(d => d.TenantId.Equals(tenant.Id) && !d.Deleted).ToList()
                };
            }
            catch (Exception ex)
            {
                _tenantServiceLogger.LogError(ex, ex.Message);
                return new GetDriversResponse()
                {
                    Result = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        public AddDriverResponse AddDriver(AddDriverRequest request)
        {
            try
            {
                var tenant = CheckPriviledgesAndGetApplicationInstance(request);
                request.Driver.TenantId = tenant.Id;
                _dbContext.Drivers.Add(request.Driver);
                _dbContext.SaveChanges();
                return new AddDriverResponse()
                {
                    Result = true,
                    DriverId = request.Driver.Id
                };
            }
            catch (Exception ex)
            {
                _tenantServiceLogger.LogError(ex, ex.Message);
                return new AddDriverResponse()
                {
                    Result = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        public UpdateDriverResponse UpdateDriver(UpdateDriverRequest request)
        {
            try
            {
                var tenant = CheckPriviledgesAndGetApplicationInstance(request);
                var driver = _dbContext.Drivers.FirstOrDefault(d => d.TenantId.Equals(tenant.Id) && d.Id.Equals(request.Driver.Id));
                if (driver == null)
                    return new UpdateDriverResponse()
                    {
                        Result = false,
                        ErrorMessage = $"Driver with key {request.Driver.Id} does not exist"
                    };
                driver.FriendlyName = request.Driver.FriendlyName;
                driver.CompanyName = request.Driver.CompanyName;
                driver.EmailAddress = request.Driver.EmailAddress;
                driver.ApplicationFeeChargeDriver = request.Driver.ApplicationFeeChargeDriver;
                driver.Blocked = request.Driver.Blocked;
                driver.Deleted = request.Driver.Deleted;
                driver.Confirmed = request.Driver.Confirmed;
                driver.BoltName = request.Driver.BoltName;
                driver.UberName = request.Driver.UberName;
                driver.FreenowName = request.Driver.FreenowName;
                driver.Deposit = request.Driver.Deposit;
                if (request.Driver.PartnershipFee.HasValue)
                    driver.PartnershipFee = request.Driver.PartnershipFee;
                else
                    driver.PartnershipFee = null;
                _dbContext.SaveChanges();
                return new UpdateDriverResponse()
                {
                    Result = true
                };
            }
            catch (Exception ex)
            {
                _tenantServiceLogger.LogError(ex, ex.Message);
                return new UpdateDriverResponse()
                {
                    Result = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        public DeleteDriverResponse DeleteDriver(DeleteDriverRequest request)
        {
            try
            {
                var tenant = CheckPriviledgesAndGetApplicationInstance(request);
                var driver = _dbContext.Drivers.FirstOrDefault(d => d.TenantId.Equals(tenant.Id) && d.Id.Equals(request.DriverId));
                if (driver == null)
                    return new DeleteDriverResponse()
                    {
                        Result = false,
                        ErrorMessage = $"Driver with key {request.DriverId} does not exist"
                    };
                _dbContext.Drivers.Remove(driver);
                _dbContext.SaveChanges();
                return new DeleteDriverResponse()
                {
                    Result = true
                };
            }
            catch (Exception ex)
            {
                _tenantServiceLogger.LogError(ex, ex.Message);
                return new DeleteDriverResponse()
                {
                    Result = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        public SetDriverInvitationKeyResponse SetDriverInvitationKey(SetDriverInvitationKeyRequest request)
        {
            try
            {
                var tenant = CheckPriviledgesAndGetApplicationInstance(request);
                var driver = _dbContext.Drivers.FirstOrDefault(d => d.TenantId.Equals(tenant.Id) && d.Id.Equals(request.DriverId));
                if (driver == null)
                    return new SetDriverInvitationKeyResponse()
                    {
                        Result = false,
                        ErrorMessage = $"Driver with key {request.DriverId} does not exist"
                    };
                Random generator = new Random();
                var invitationKey = generator.Next(0, 100000000).ToString("D8");
                while (_dbContext.Tenants.Count(t => invitationKey.Equals(t.InvitationKey)) != 0
                    || _dbContext.Drivers.Count(t => invitationKey.Equals(t.InvitationKey)) != 0)
                    invitationKey = generator.Next(0, 100000000).ToString("D8");
                driver.InvitationKey = invitationKey;
                _dbContext.SaveChanges();
                return new SetDriverInvitationKeyResponse()
                {
                    Result = true,
                    InvitationKey = invitationKey
                };
            }
            catch (Exception ex)
            {
                _tenantServiceLogger.LogError(ex, ex.Message);
                return new SetDriverInvitationKeyResponse()
                {
                    Result = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        public GetTenantMetadataResponse GetTenantMetadata(GetTenantMetadataRequest request)
        {
            try
            {
                var tenant = CheckPriviledgesAndGetApplicationInstance(request);
                return new GetTenantMetadataResponse()
                {
                    Result = true,
                    Tenant = tenant
                };
            }
            catch (Exception ex)
            {
                _tenantServiceLogger.LogError(ex, ex.Message);
                return new GetTenantMetadataResponse()
                {
                    Result = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        public SaveTenantMetadataResponse SaveTenantMetadata(SaveTenantMetadataRequest request)
        {
            try
            {
                var tenant = CheckPriviledgesAndGetApplicationInstance(request);
                tenant.FriendlyName = request.Tenant.FriendlyName;
                tenant.CompanyName = request.Tenant.CompanyName;
                tenant.EmailAddress = request.Tenant.EmailAddress;
                tenant.BoltCompanyId = request.Tenant.BoltCompanyId;
                tenant.BoltUserName = request.Tenant.BoltUserName;
                tenant.BoltPassword = request.Tenant.BoltPassword;
                tenant.UberCompanyId = request.Tenant.UberCompanyId;
                tenant.UberUserName = request.Tenant.UberUserName;
                tenant.UberPassword = request.Tenant.UberPassword;
                tenant.FreenowUserName = request.Tenant.FreenowUserName;
                tenant.FreenowPassword = request.Tenant.FreenowPassword;
                _dbContext.SaveChanges();
                return new SaveTenantMetadataResponse()
                {
                    Result = true
                };
            }
            catch (Exception ex)
            {
                _tenantServiceLogger.LogError(ex, ex.Message);
                return new SaveTenantMetadataResponse()
                {
                    Result = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        public DownloadExternalReportResponse DownloadExternalReport(DownloadExternalReportRequest request)
        {
            try
            {
                var requestedDate = DateTime.Now;
                var tenant = CheckPriviledgesAndGetApplicationInstance(request);
                var firstDateOfWeek = Common.Helpers.Date.GetFirstDateOfWeek(request.YearNumber, request.WeekNumber);
                var reportFolder = Path.Combine(
                        _externalReportsConfiguration.ReportsFolder,
                        $"{tenant.Id.ToString("#000")}",
                        request.YearNumber.ToString("#0000"),
                        $"Week {request.WeekNumber.ToString("#00")} ({firstDateOfWeek.ToString("yyyy.MM.dd")}-{firstDateOfWeek.AddDays(6).ToString("yyyy.MM.dd")})");
                var reportFilePath = Path.Combine(
                    reportFolder,
                    $"{request.YearNumber.ToString("#0000")}.{request.WeekNumber.ToString("#00")} ({firstDateOfWeek.ToString("yyyy.MM.dd")}-{firstDateOfWeek.AddDays(6).ToString("yyyy.MM.dd")}) - {request.ReportType}.csv"
                    );
                if (request.ReportType == ExternalReportType.Freenow)
                    reportFilePath = reportFilePath.Replace(".csv", ".zip");
                var externalReport = _dbContext.ExternalReports.FirstOrDefault(er => er.TenantId.Equals(tenant.Id)
                    && er.YearNumber.Equals(request.YearNumber) && er.WeekNumber.Equals(request.WeekNumber) && er.ReportType == request.ReportType);
                if (!request.Overwrite && externalReport!=null && externalReport.Status==ExternalReportStatus.Generated && !string.IsNullOrWhiteSpace(externalReport.ReportBody))
                    return new DownloadExternalReportResponse()
                    {
                        Result = true,
                        Report = externalReport
                    };
                if (!Directory.Exists(reportFolder))
                    Directory.CreateDirectory(reportFolder);
                ExternalServicesWrapper? wrapper = null;
                switch (request.ReportType)
                {
                    case ExternalReportType.Bolt:
                        wrapper = new ExternalServicesWrapper(_externalReportsConfiguration.ExternalServicesAdapterFile, request.ReportType,
                            tenant.BoltCompanyId, tenant.BoltUserName, tenant.BoltPassword,
                            firstDateOfWeek, firstDateOfWeek.AddDays(6),
                            reportFilePath);
                        break;
                    case ExternalReportType.Uber:
                        wrapper = new ExternalServicesWrapper(_externalReportsConfiguration.ExternalServicesAdapterFile, request.ReportType,
                            tenant.UberCompanyId, tenant.UberUserName, tenant.UberPassword,
                            firstDateOfWeek, firstDateOfWeek.AddDays(6),
                            reportFilePath);
                        break;
                    case ExternalReportType.Freenow:
                        wrapper = new ExternalServicesWrapper(_externalReportsConfiguration.ExternalServicesAdapterFile, request.ReportType,
                            "", tenant.FreenowUserName, tenant.FreenowPassword,
                            firstDateOfWeek, firstDateOfWeek.AddDays(6),
                            reportFilePath);
                        break;
                    default:
                        throw new Exception($"Unsupported report type [{request.ReportType}]");
                }
                if (wrapper == null)
                    throw new Exception($"Unsupported report type [{request.ReportType}]");
                var task = wrapper.Execute();
                task.Wait();

                var reportParser = new ExternalReportParser();
                if (!reportParser.Parse(reportFilePath, request.ReportType))
                    throw new Exception($"Appeared problem with parsing {request.ReportType} report [year: {request.YearNumber}, week: {request.WeekNumber.ToString("#00")}]");
                externalReport = _dbContext.ExternalReports.FirstOrDefault(er => er.TenantId.Equals(tenant.Id)
                    && er.YearNumber.Equals(request.YearNumber) && er.WeekNumber.Equals(request.WeekNumber) && er.ReportType == request.ReportType);
                if (externalReport==null)
                {
                    externalReport = new ExternalReport()
                    {
                        TenantId = tenant.Id,
                        ReportType = request.ReportType,
                        YearNumber = request.YearNumber,
                        WeekNumber = request.WeekNumber,
                        DateFrom = firstDateOfWeek,
                        DateTo = firstDateOfWeek.AddDays(6),
                        RequestDate = requestedDate,
                        Status = ExternalReportStatus.Unknown
                    };
                    _dbContext.ExternalReports.Add(externalReport);
                }
                externalReport.RequestDate = requestedDate;
                if (!task.Result) 
                {
                    externalReport.Status = ExternalReportStatus.Unknown;
                    string errorMessage = string.Empty;
                    if (wrapper.ReceivedData.Count>0)
                        errorMessage = wrapper.ReceivedData.First();
                    else if (wrapper.ReceivedErrorData.Count > 0)
                        errorMessage = wrapper.ReceivedErrorData.First();                
                    externalReport.Status = ExternalReportStatus.Failed;
                    externalReport.ErrorMessage = errorMessage;
                    externalReport.DownloadDate = DateTime.Now;
                    externalReport.ReportBody = string.Empty; 
                    _dbContext.SaveChanges();   
                    throw new Exception(errorMessage);
                }
                externalReport.Status = ExternalReportStatus.Generated;
                externalReport.ErrorMessage = string.Empty;
                externalReport.DownloadDate = DateTime.Now;
                externalReport.ReportBody = reportParser.Serialize();
                _dbContext.SaveChanges();
                CollectExternalReportMetadata(tenant.Id, externalReport);
                return new DownloadExternalReportResponse()
                {
                    Result = true,
                    Report = externalReport
                };
            }
            catch (Exception ex)
            {
                _tenantServiceLogger.LogError(ex, ex.Message);
                return new DownloadExternalReportResponse()
                {
                    Result = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        public UploadExternalReportResponse UploadExternalReport(UploadExternalReportRequest request)
        {
            try
            {
                var tenant = CheckPriviledgesAndGetApplicationInstance(request);
                var firstDateOfWeek = Common.Helpers.Date.GetFirstDateOfWeek(request.YearNumber, request.WeekNumber);
                var reportFolder = Path.Combine(
                        _externalReportsConfiguration.ReportsFolder,
                        $"{tenant.Id.ToString("#000")}",
                        request.YearNumber.ToString("#0000"),
                        $"Week {request.WeekNumber.ToString("#00")} ({firstDateOfWeek.ToString("yyyy.MM.dd")}-{firstDateOfWeek.AddDays(6).ToString("yyyy.MM.dd")})");
                var reportFilePath = Path.Combine(
                    reportFolder,
                    $"{request.YearNumber.ToString("#0000")}.{request.WeekNumber.ToString("#00")} ({firstDateOfWeek.ToString("yyyy.MM.dd")}-{firstDateOfWeek.AddDays(6).ToString("yyyy.MM.dd")}) - {request.ReportType}.csv"
                );
                if (request.ReportType == ExternalReportType.Freenow)
                    reportFilePath = reportFilePath.Replace(".csv", ".zip");
                if (!Directory.Exists(reportFolder))
                    Directory.CreateDirectory(reportFolder);
                using (var fileStream = new FileStream(reportFilePath, FileMode.Create, FileAccess.Write))
                    fileStream.Write(request.ReportFileContent, 0, request.ReportFileContent.Length);

                var reportParser = new ExternalReportParser();
                if (!reportParser.Parse(reportFilePath, request.ReportType))
                    throw new Exception($"Appeared problem with parsing {request.ReportType} report [year: {request.YearNumber}, week: {request.WeekNumber.ToString("#00")}]");
                var externalReport = _dbContext.ExternalReports.FirstOrDefault(er => er.TenantId.Equals(tenant.Id)
                    && er.YearNumber.Equals(request.YearNumber) && er.WeekNumber.Equals(request.WeekNumber) && er.ReportType == request.ReportType);
                if (externalReport == null)
                {
                    externalReport = new ExternalReport()
                    {
                        TenantId = tenant.Id,
                        ReportType = request.ReportType,
                        YearNumber = request.YearNumber,
                        WeekNumber = request.WeekNumber,
                        DateFrom = firstDateOfWeek,
                        DateTo = firstDateOfWeek.AddDays(6),
                        RequestDate = DateTime.Now,
                        DownloadDate = DateTime.Now,
                        Status = ExternalReportStatus.Unknown
                    };
                    _dbContext.ExternalReports.Add(externalReport);
                }
                externalReport.RequestDate = DateTime.Now;
                externalReport.Status = ExternalReportStatus.Generated;
                externalReport.ErrorMessage = string.Empty;
                externalReport.ReportBody = reportParser.Serialize();
                _dbContext.SaveChanges();
                CollectExternalReportMetadata(tenant.Id, externalReport);
                return new UploadExternalReportResponse()
                {
                    Result = true,
                    Report = externalReport
                };
            }
            catch (Exception ex)
            {
                _tenantServiceLogger.LogError(ex, ex.Message);
                return new UploadExternalReportResponse()
                {
                    Result = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        public GetWeeklyExternalReportsMetadataResponse GetWeeklyExternalReportsMetadata(GetWeeklyExternalReportsMetadataRequest request)
        {
            try
            {
                var tenant = CheckPriviledgesAndGetApplicationInstance(request);
                var reports = new List<ExternalReportMetadata>();
                var weekReports = _dbContext.ExternalReports.Where(er => er.TenantId.Equals(tenant.Id)
                    && er.YearNumber.Equals(request.YearNumber) && er.WeekNumber.Equals(request.WeekNumber)).ToList();
                var defReportItem = ReportTemplatesHelper.GetReportTemplate(_dbContext, "Basic report", DateTime.Now);

                if (defReportItem == null || defReportItem.DefReportColumns == null)
                    throw new Exception("Appeared problem with basic report definition");
                List<string> columnNames = defReportItem.DefReportColumns.Skip(1).Select(c => c.ColumnName).ToList();

                var externalReports = new List<ExternalReportMetadata>();
                var reportsValues = new Dictionary<ExternalReportType, List<double>>();
                var weeklySettlementComponents = new Dictionary<string, double>();

                var valuesPerDriver = new Dictionary<Driver, List<double>>();

                if (weekReports != null && weekReports.Count > 0)
                {
                    var reportDriversDict = new Dictionary<ExternalReportType, List<string>>();
                    foreach (var externalReport in weekReports)
                    {
                        defReportItem = ReportTemplatesHelper.GetReportTemplate(_dbContext, "Basic report", externalReport.DownloadDate);
                        if (externalReport.Status == ExternalReportStatus.Generated)
                        {
                            var reportBody = ExternalReportBody.Deserialize(externalReport.ReportBody);
                            if (reportBody == null)
                                throw new Exception($"Invalid report body in {externalReport.ReportType} report for week {request.YearNumber}.{request.WeekNumber}");
                            ReportTemplatesHelper.SetReportBodyGlobalTemplates(_dbContext, reportBody, externalReport.ReportType, externalReport.DownloadDate);
                            ReportTemplatesHelper.SetReportBodyBasicReportTemplate(_dbContext, reportBody, externalReport.ReportType, externalReport.DownloadDate);
                            var reportDrivers = reportBody.GetDrivers()?.Select(d => d.ToLower());
                            var databaseDrivers = GetDatabaseDriverNamesDict(tenant.Id, externalReport.ReportType);
                            if (reportDrivers != null)
                            {
                                reportDriversDict.Add(externalReport.ReportType, reportDrivers.ToList());
                                //add not confirmed drivers
                                var unmatchedDrivers = reportDrivers.Where(rd => !databaseDrivers.ContainsKey(rd)).ToList();
                                if (unmatchedDrivers.Count>0)
                                {
                                    databaseDrivers = GetDatabaseDriverNamesDict(tenant.Id, ExternalReportType.Unknown);
                                    var automaticallyMatchedDrivers = databaseDrivers.Where(d => unmatchedDrivers.Contains(d.Key.ToLower())).ToDictionary(d=>d.Key,d=>d.Value);
                                    if (automaticallyMatchedDrivers.Count>0)
                                    {
                                        foreach(var driverKvp in automaticallyMatchedDrivers)
                                        {
                                            switch(externalReport.ReportType)
                                            {
                                                case ExternalReportType.Bolt:
                                                    driverKvp.Value.BoltName = Strings.StringsToString(driverKvp.Value.BoltName, Strings.FirstCharToUpper(driverKvp.Key, false,StringSplitOptions.None));
                                                    break;
                                                case ExternalReportType.Uber:
                                                    driverKvp.Value.UberName = Strings.StringsToString(driverKvp.Value.UberName, Strings.FirstCharToUpper(driverKvp.Key, false, StringSplitOptions.None));
                                                    break;
                                                case ExternalReportType.Freenow:
                                                    driverKvp.Value.FreenowName = Strings.StringsToString(driverKvp.Value.FreenowName, Strings.FirstCharToUpper(driverKvp.Key, false, StringSplitOptions.None));
                                                    break;
                                            }
                                            if (!string.IsNullOrWhiteSpace(driverKvp.Value.BoltName) && !string.IsNullOrWhiteSpace(driverKvp.Value.UberName) && !string.IsNullOrWhiteSpace(driverKvp.Value.FreenowName))
                                                driverKvp.Value.Confirmed = true;
                                        }
                                    }
                                    var leftUnmatchedDrivers = unmatchedDrivers.Where(d => !automaticallyMatchedDrivers.ContainsKey(d)).ToList();
                                    if (leftUnmatchedDrivers.Count>0)
                                    {
                                        foreach(var driverName in leftUnmatchedDrivers)
                                        {
                                            var driver = new Driver()
                                            {
                                                TenantId = tenant.Id,
                                                CompanyName = Strings.FirstCharToUpper(driverName, false),
                                                Confirmed = false                                                    
                                            };
                                            switch (externalReport.ReportType)
                                            {
                                                case ExternalReportType.Bolt:
                                                    driver.BoltName = Strings.FirstCharToUpper(driverName, false, StringSplitOptions.None);
                                                    break;
                                                case ExternalReportType.Uber:
                                                    driver.UberName = Strings.FirstCharToUpper(driverName, false, StringSplitOptions.None);
                                                    break;
                                                case ExternalReportType.Freenow:
                                                    driver.FreenowName = Strings.FirstCharToUpper(driverName, false, StringSplitOptions.None);
                                                    break;
                                            }
                                            _dbContext.Drivers.Add(driver);                                                
                                        }
                                    }
                                    _dbContext.SaveChanges();
                                }
                            }
                            var values = reportBody.GetValues();
                            if (values != null)
                            {
                                if (request.DriverId.HasValue)
                                {
                                    var driver = _dbContext.Drivers.FirstOrDefault(d => d.Id.Equals(request.DriverId.Value));
                                    if (driver == null)
                                        throw new Exception($"Driver with ID={request.DriverId.Value} does not exist");
                                    if (driver.TenantId!=tenant.Id)
                                        throw new Exception($"Appeared problem with ownership of driver with ID={request.DriverId.Value}");
                                    var driverNames = driver.GetDriverNames();
                                    var driverValues = values.Where(v => driverNames.Contains(v.Key)).ToList();
                                    if (driverValues.Count > 1)
                                    {
                                        _tenantServiceLogger.LogWarning($"Multiple values for driver names {string.Join(',', driverNames.Select(n => Strings.FirstCharToUpper(n, false, StringSplitOptions.None)))}");
                                        List<double> tempValues = new List<double>();
                                        for (int i = 0; i < driverValues.First().Value.Count; i++)
                                            tempValues.Add(driverValues.Sum(v => v.Value[i]));
                                        reportsValues.Add(externalReport.ReportType, tempValues);
                                    }
                                    else if (driverValues.Count == 1)
                                        reportsValues.Add(externalReport.ReportType, driverValues.First().Value);
                                }
                                else
                                {
                                    var doubles = new List<double>();
                                    for (int i = 0; i < columnNames.Count; i++)
                                        doubles.Add(values.Sum(v => v.Value[i]));
                                    reportsValues.Add(externalReport.ReportType, doubles);
                                }
                                //collect inforamtion about reportValuesPerDriver
                                foreach (var valuesKvp in values)
                                {
                                    if (databaseDrivers.TryGetValue(valuesKvp.Key.ToLower(), out Driver driver))
                                    {
                                        if (valuesPerDriver.TryGetValue(driver,out List<double> driverValues))
                                        {
                                            for (int i = 0; i < columnNames.Count; i++)
                                                driverValues[i] += valuesKvp.Value[i];
                                        }
                                        else
                                        {
                                            driverValues = new List<double>();
                                            for (int i = 0; i < columnNames.Count; i++)
                                                driverValues.Add(valuesKvp.Value[i]);
                                            valuesPerDriver.Add(driver, driverValues);
                                        }
                                    }
                                }
                            }
                        }
                        externalReports.Add(externalReport);
                    }
                    //calculate number of unmatched drivers
                    foreach (var report in externalReports)
                    {
                        if (reportDriversDict.TryGetValue(report.ReportType, out var driverNames))
                        {
                            var databaseDrivers = GetDatabaseDriverNamesDict(tenant.Id, report.ReportType);
                            report.NotComfirmedDriversIdentifiers = databaseDrivers.Where(d => !d.Value.Confirmed && driverNames.Contains(d.Key)).Select(d => d.Value.Id).ToHashSet();
                        }
                    }
                    AddReportValues(weeklySettlementComponents, columnNames, reportsValues);
                }
                AddCarRentalCosts(weeklySettlementComponents, tenant.Id, request.DriverId, request.YearNumber, request.WeekNumber);
                AddFuelCosts(weeklySettlementComponents, tenant.Id, request.DriverId, request.YearNumber, request.WeekNumber);
                AddOtherSettlementItems(weeklySettlementComponents, tenant.Id, request.DriverId, request.YearNumber, request.WeekNumber);
                AddPartnershipFee(weeklySettlementComponents, tenant, request.DriverId, request.YearNumber, request.WeekNumber, columnNames, valuesPerDriver);
                AddApplicationFee(weeklySettlementComponents, tenant, request.DriverId, request.YearNumber, request.WeekNumber);

                var billingWeek = _dbContext.BillingWeeks.FirstOrDefault(bw=>bw.TenantId.Equals(tenant.Id) 
                    && bw.YearNumber.Equals(request.YearNumber) && bw.WeekNumber.Equals(request.WeekNumber));
                var billingWeekStatus = (billingWeek == null ? BillingWeekStatus.NotExists : billingWeek.Status);
                var weeklySettlementsInfo = (billingWeekStatus == BillingWeekStatus.Closed) ? null : GetWeeklySettlementsInfo(tenant.Id, request.YearNumber, request.WeekNumber);
                return new GetWeeklyExternalReportsMetadataResponse()
                {
                    Result = true,
                    Reports = externalReports,
                    TenantReportsValueNames = columnNames,
                    TenantReportsValues = reportsValues,
                    WeeklySettlementComponents = weeklySettlementComponents,
                    BillingWeekStatus = (billingWeek == null ? BillingWeekStatus.NotExists : billingWeek.Status),
                    WeeklySettlementsInfo = weeklySettlementsInfo
                };
            }
            catch (Exception ex)
            {
                _tenantServiceLogger.LogError(ex, ex.Message);
                return new GetWeeklyExternalReportsMetadataResponse()
                {
                    Result = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        private WeeklySettlementsInfo GetWeeklySettlementsInfo(int tenantId, int yearNumber, int weekNumber)
        {
            return new WeeklySettlementsInfo()
            {
                NumberOfTariffPlanAssignments = _dbContext.TariffPlanAssignments.Count(tpa => tpa.TenantId.Equals(tenantId) && tpa.YearNumber.Equals(yearNumber) && tpa.WeekNumber.Equals(weekNumber)),
                NumberOfSettlementItems = _dbContext.SettlementItems.Count(si => si.TenantId.Equals(tenantId) && si.YearNumber.Equals(yearNumber) && si.WeekNumber.Equals(weekNumber)),
                NumberOfRequestedFuelBills = _dbContext.FuelBills.Count(fb => fb.TenantId.Equals(tenantId) && fb.YearNumber.Equals(yearNumber) && fb.WeekNumber.Equals(weekNumber) && fb.Status == FuelBillStatus.Requested),
                NumberOfAcceptedFuelBills = _dbContext.FuelBills.Count(fb => fb.TenantId.Equals(tenantId) && fb.YearNumber.Equals(yearNumber) && fb.WeekNumber.Equals(weekNumber) && fb.Status == FuelBillStatus.Acccepted),
                NumberOfRejectedFuelBills = _dbContext.FuelBills.Count(fb => fb.TenantId.Equals(tenantId) && fb.YearNumber.Equals(yearNumber) && fb.WeekNumber.Equals(weekNumber) && fb.Status == FuelBillStatus.Rejected)
            };
        }

        private void AddReportValues(Dictionary<string, double> weeklySettlementComponents, List<string> columnNames, Dictionary<ExternalReportType, List<double>> reportsValues)
        {
            if (reportsValues == null || reportsValues.Count < 1)
                return;
            var settlementType1 = _dbContext.SettlementTypes.FirstOrDefault(st => st.Id == 10);
            var settlementType2 = _dbContext.SettlementTypes.FirstOrDefault(st => st.Id == 9);
            if (settlementType1 == null || settlementType2 == null)
                return;
            var columnIdx = columnNames.IndexOf("Konto spółki");
            if (columnIdx > -1)
            {
                var value = reportsValues.Sum(v => v.Value[columnIdx]);
                if (settlementType1.Debit)
                    value = -1.0 * value;
                weeklySettlementComponents.Add(settlementType1.TypeName, value * settlementType1.Factor);
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

        private void AddCarRentalCosts(Dictionary<string, double> weeklySettlementComponents, int tenantId, int? driverId, int yearNumber, int weekNumber)
        {
            var settlementType = _dbContext.SettlementTypes.FirstOrDefault(st => st.Id == 1);
            if (settlementType == null)
                return;
            var tariffPlanAssignments = _dbContext.TariffPlanAssignments.Where(fb => fb.TenantId.Equals(tenantId) && fb.YearNumber.Equals(yearNumber) && fb.WeekNumber.Equals(weekNumber)).Include(fb => fb.TariffPlan).ToList();
            if (driverId.HasValue)
                tariffPlanAssignments = tariffPlanAssignments.Where(tpa => tpa.DriverId.Equals(driverId.Value)).ToList();
            var value = 0.0;
            if (tariffPlanAssignments != null && tariffPlanAssignments.Count > 0)
            {
                foreach (var tariffPlanAssignment in tariffPlanAssignments)
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
                    value += carRentalCost.Value * settlementType.Factor * numberOfDays / 7.0;
                }
            }
            var settlementItems = _dbContext.SettlementItems.Where(si => si.TenantId.Equals(tenantId) && si.YearNumber.Equals(yearNumber) && si.WeekNumber.Equals(weekNumber)
                && si.SettlementTypeId.Equals(settlementType.Id));
            if (driverId.HasValue)
                settlementItems = settlementItems.Where(si => si.DriverId.Equals(driverId.Value));
            if (settlementItems.Any())
            {
                var carRentalCost = settlementItems.Sum(si => si.Value);
                if (settlementType.Debit)
                    carRentalCost = -1 * carRentalCost;
                value += carRentalCost * settlementType.Factor;
            }
            if (value != 0.0)
                weeklySettlementComponents.Add(settlementType.TypeName, value);
        }

        private void AddFuelCosts(Dictionary<string, double> weeklySettlementComponents, int tenantId, int? driverId, int yearNumber, int weekNumber)
        {
            var settlementType = _dbContext.SettlementTypes.FirstOrDefault(st => st.Id == 2);
            if (settlementType == null)
                return;
            var fuelBills = _dbContext.FuelBills.Where(fb => fb.TenantId.Equals(tenantId) && fb.YearNumber.Equals(yearNumber) && fb.WeekNumber.Equals(weekNumber)
                && fb.Status == FuelBillStatus.Acccepted);
            if (driverId.HasValue)
                fuelBills = fuelBills.Where(fb => fb.DriverId.Equals(driverId.Value));
            var value = 0.0;
            if (fuelBills != null && fuelBills.Count() > 0)
            {
                var fuelCost = fuelBills.Sum(fb => fb.Cost);
                if (settlementType.Debit)
                    fuelCost = -1 * fuelCost;
                value += fuelCost * settlementType.Factor;
            }
            var settlementItems = _dbContext.SettlementItems.Where(si => si.TenantId.Equals(tenantId) && si.YearNumber.Equals(yearNumber) && si.WeekNumber.Equals(weekNumber)
                && si.SettlementTypeId.Equals(settlementType.Id));
            if (driverId.HasValue)
                settlementItems = settlementItems.Where(si => si.DriverId.Equals(driverId.Value));
            if (settlementItems.Any())
            {
                var fuelCost = settlementItems.Sum(si => si.Value);
                if (settlementType.Debit)
                    fuelCost = -1 * fuelCost;
                value += fuelCost * settlementType.Factor;
            }
            if (value!=0.0)
                weeklySettlementComponents.Add(settlementType.TypeName, value);
        }

        private void AddOtherSettlementItems(Dictionary<string, double> weeklySettlementComponents, int tenantId, int? driverId, int yearNumber, int weekNumber)
        {
            var settlementItems = _dbContext.SettlementItems.Where(si => si.TenantId.Equals(tenantId)
                && si.YearNumber.Equals(yearNumber) && si.WeekNumber.Equals(weekNumber)
                && si.SettlementType != null && si.SettlementType.Id != 1 && si.SettlementType.Id != 2
                && si.SettlementType.Id != 9 && si.SettlementType.Id != 10).Include(si => si.SettlementType).ToList();
            if (driverId.HasValue)
                settlementItems = settlementItems.Where(si => si.DriverId.Equals(driverId.Value)).ToList();
            if (settlementItems == null || settlementItems.Count() == 0)
                return;
            var nsloopup = settlementItems.ToLookup(si => si.SettlementTypeId);
            foreach(var settlementItemsGroup in nsloopup)
            {
                var settlementType = settlementItemsGroup.First().SettlementType;
                if (settlementType == null) continue;
                var value = settlementItemsGroup.Sum(sig => sig.Value) * settlementType.Factor;
                if (settlementType.Debit)
                    value = -1 * value;
                if (value != 0.0)
                    weeklySettlementComponents.Add(settlementType.TypeName, value);
            }
        }

        private void AddPartnershipFee(Dictionary<string, double> weeklySettlementComponents, Tenant tenant, int? driverId, int yearNumber, int weekNumber, List<string> columnNames, Dictionary<Driver, List<double>> valuesPerDriver)
        {
            var settlementType = _dbContext.SettlementTypes.FirstOrDefault(st => st.Id == 19);
            if (settlementType == null)
                return;
            var billingWeek = _dbContext.BillingWeeks.FirstOrDefault(bw => bw.TenantId.Equals(tenant.Id)
                && bw.YearNumber.Equals(yearNumber) && bw.WeekNumber.Equals(weekNumber));
            if (billingWeek == null || billingWeek.Status != BillingWeekStatus.Closed || !billingWeek.NumberOfDrivers.HasValue)
                return;
            var tempValues = valuesPerDriver.Where(vpd => vpd.Key.PartnershipFee.HasValue);
            if (driverId.HasValue)
                tempValues = tempValues.Where(vpd => vpd.Key.Id == driverId.Value);
            if (tempValues.Count() < 1)
                return;
            var columnIdx = columnNames.IndexOf("Utarg brutto");
            if (columnIdx < 0)
                return;
            var sum = tempValues.Sum(vpd => vpd.Key.PartnershipFee * vpd.Value[columnIdx] / 100.0);
            if (!sum.HasValue)
                return;
            weeklySettlementComponents.Add(settlementType.TypeName, sum.Value * (settlementType.Debit ? -1.0 : 1.0) * settlementType.Factor);
        }

        private void AddApplicationFee(Dictionary<string, double> weeklySettlementComponents, Tenant tenant, int? driverId, int yearNumber, int weekNumber)
        {
            var settlementType = _dbContext.SettlementTypes.FirstOrDefault(st => st.Id == 8);
            if (settlementType == null)
                return;
            var billingWeek = _dbContext.BillingWeeks.FirstOrDefault(bw => bw.TenantId.Equals(tenant.Id)
                && bw.YearNumber.Equals(yearNumber) && bw.WeekNumber.Equals(weekNumber));
            if (billingWeek == null || billingWeek.Status != BillingWeekStatus.Closed || !billingWeek.ApplicationFeeUnitPrice.HasValue || !billingWeek.NumberOfDrivers.HasValue)
                return;
            if (driverId.HasValue)
            {
                var driver = _dbContext.Drivers.FirstOrDefault(d => d.Id.Equals(driverId.Value));
                if (driver != null && tenant.ApplicationFeeChargeDriver && driver.ApplicationFeeChargeDriver && weeklySettlementComponents.Any(wsc => !wsc.Value.Equals(0.0)))
                    weeklySettlementComponents.Add(settlementType.TypeName, billingWeek.ApplicationFeeUnitPrice.Value * (settlementType.Debit ? -1.0 : 1.0) * settlementType.Factor);
                else
                    weeklySettlementComponents.Add(settlementType.TypeName, 0.0);
            }
            else
                weeklySettlementComponents.Add(settlementType.TypeName, billingWeek.NumberOfDrivers.Value * billingWeek.ApplicationFeeUnitPrice.Value * (settlementType.Debit ? -1.0 : 1.0) * settlementType.Factor);
        }

        public GetBasicReportResponse GetBasicReport(GetBasicReportRequest request)
        {
            try
            {
                var internalBasicReportResponse = GetInternalBasicReport(request);
                var report = new Report()
                {
                    Header = ["DriverName", .. internalBasicReportResponse.ColumnNames],
                    Lines = new List<List<string>>()
                };
                foreach(var driverValuesKvp in internalBasicReportResponse.DriversWithValues)
                {
                    var strValues = new List<string> { driverValuesKvp.Key };
                    strValues.AddRange(driverValuesKvp.Value.Select(v => v.ToString("#0.00", CultureInfo.InvariantCulture)));
                    report.Lines.Add(strValues);
                }
                return new GetBasicReportResponse()
                {
                    Result = true,
                    Report = report
                };
            }
            catch (Exception ex)
            {
                _tenantServiceLogger.LogError(ex, ex.Message);
                return new GetBasicReportResponse()
                {
                    Result = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        public GetCustomReportResponse GetCustomReport(GetCustomReportRequest request)
        {
            try
            {
                List<string> header = null;
                var driversWithValues = new SortedDictionary<string, List<double>>();
                foreach (var week in request.Weeks)
                {
                    var internalBasicReportResponse = GetInternalBasicReport(new GetBasicReportRequest()
                    {
                        ApplicationInstanceId = request.ApplicationInstanceId,
                        YearNumber = week.YearNumber,
                        WeekNumber = week.WeekNumber
                    });
                    if (header == null)
                        header = ["DriverName", .. internalBasicReportResponse.ColumnNames];
                    foreach (var driverWithValuesKvp in internalBasicReportResponse.DriversWithValues)
                    {
                        if (driversWithValues.TryGetValue(driverWithValuesKvp.Key, out var values))
                        {
                            for (int i = 0; i < values.Count; i++)
                                values[i] += driverWithValuesKvp.Value[i];
                        }
                        else
                            driversWithValues.Add(driverWithValuesKvp.Key, driverWithValuesKvp.Value);
                    }
                }
                var report = new Report()
                {
                    Header = header,
                    Lines = new List<List<string>>()
                };
                foreach (var driverValuesKvp in driversWithValues)
                {
                    var strValues = new List<string> { driverValuesKvp.Key };
                    strValues.AddRange(driverValuesKvp.Value.Select(v => v.ToString("#0.00", CultureInfo.InvariantCulture)));
                    report.Lines.Add(strValues);
                }
                return new GetCustomReportResponse()
                {
                    Result = true,
                    Report = report
                };
            }
            catch(Exception ex)
            {
                _tenantServiceLogger.LogError(ex, ex.Message);
                return new GetCustomReportResponse()
                {
                    Result = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        #region Internal methods used in Basic and Custom reports
        private GetInternalBasicReportResponse GetInternalBasicReport(GetBasicReportRequest request)
        {
            try
            {
                var tenant = CheckPriviledgesAndGetApplicationInstance(request);
                var reportTemplate = ReportTemplatesHelper.GetReportTemplate(_dbContext, "Basic report", DateTime.Now);
                var reportColumnsTemplates = reportTemplate.DefReportColumns?.Where(c => string.IsNullOrWhiteSpace(c.DefValue)).ToList();
                if (reportColumnsTemplates == null || reportColumnsTemplates.Count < 1)
                    throw new Exception("Missing report columns definitions in Basic report definition");

                var reportsValues = GetReportValues(tenant.Id, request.YearNumber, request.WeekNumber);
                var driverReportItems = GetDriverReportItems(tenant.Id, reportColumnsTemplates, reportsValues);
                var driverReportHeaders = new List<string[]>();
                driverReportHeaders.Add(reportColumnsTemplates.Select(c => $"Bolt - {c.ColumnName}").ToArray());
                driverReportHeaders.Add(reportColumnsTemplates.Select(c => $"Uber - {c.ColumnName}").ToArray());
                driverReportHeaders.Add(reportColumnsTemplates.Select(c => $"Freenow - {c.ColumnName}").ToArray());
                driverReportHeaders.Add(reportColumnsTemplates.Select(c => c.ColumnName).ToArray());
                AddReportValues(driverReportHeaders, driverReportItems);
                AddCarRentalCosts(driverReportHeaders, driverReportItems, tenant.Id, request.YearNumber, request.WeekNumber);
                AddFuelCosts(driverReportHeaders, driverReportItems, tenant.Id, request.YearNumber, request.WeekNumber);
                AddOtherSettlementItems(driverReportHeaders, driverReportItems, tenant.Id, request.YearNumber, request.WeekNumber);
                AddPartnershipFee(driverReportHeaders, driverReportItems, tenant, request.YearNumber, request.WeekNumber);
                AddApplicationFee(driverReportHeaders, driverReportItems, tenant, request.YearNumber, request.WeekNumber);
                AddTotal(driverReportHeaders, driverReportItems);
                var columnNames = new List<string>();
                var driversWithValues = new Dictionary<string, List<double>>();
                var driverNames = new List<string>();
                var driverValues = new List<List<double>>();
                foreach (var items in driverReportHeaders)
                    columnNames.AddRange(items);
                foreach (var driverReportItemKvp in driverReportItems.ToImmutableSortedDictionary(dri => dri.Key.CompanyName, dri => dri.Value))
                {
                    var values = new List<double>();
                    foreach (var items in driverReportItemKvp.Value)
                        values.AddRange(items);
                    if (values.Any(v => v != 0.0))
                        driversWithValues.Add(driverReportItemKvp.Key, values);
                }
                return new GetInternalBasicReportResponse()
                {
                    ColumnNames = columnNames,
                    DriversWithValues = driversWithValues
                };
            }
            catch (Exception ex)
            {
                _tenantServiceLogger.LogError(ex, ex.Message);
                throw;
            }
        }

        private class GetInternalBasicReportResponse
        {
            public List<string> ColumnNames { get; set; } = new List<string>();
            public Dictionary<string, List<double>> DriversWithValues { get; set; } = new Dictionary<string, List<double>>();
        }

        private Dictionary<ExternalReportType, Dictionary<string, List<double>>> GetReportValues(int tenantId, int yearNumber, int weekNumber)
        {
            var reportsValues = new Dictionary<ExternalReportType, Dictionary<string, List<double>>>();
            var weekReports = _dbContext.ExternalReports.Where(er => er.TenantId.Equals(tenantId)
                && er.YearNumber.Equals(yearNumber) && er.WeekNumber.Equals(weekNumber) && er.Status == ExternalReportStatus.Generated).ToList();
            if (weekReports == null || weekReports.Count != 3)
                throw new Exception($"Invalid number of external reports for week {yearNumber}.{weekNumber}");
            foreach (var weekReport in weekReports)
            {
                var reportBody = ExternalReportBody.Deserialize(weekReport.ReportBody);
                if (reportBody == null)
                    throw new Exception($"Invalid report body in {weekReport.ReportType} report for week {yearNumber}.{weekNumber}");
                ReportTemplatesHelper.SetReportBodyGlobalTemplates(_dbContext, reportBody, weekReport.ReportType, weekReport.DownloadDate);
                ReportTemplatesHelper.SetReportBodyBasicReportTemplate(_dbContext, reportBody, weekReport.ReportType, weekReport.DownloadDate);
                var values = reportBody.GetValues();
                if (values != null)
                    reportsValues.Add(weekReport.ReportType, values);
            }
            return reportsValues;
        }

        private Dictionary<Driver, List<double[]>> GetDriverReportItems(int tenantId, List<DefReportColumn> reportColumnsTemplates, Dictionary<ExternalReportType, Dictionary<string, List<double>>> reportsValues)
        {
            var drivers = _dbContext.Drivers.Where(d => d.TenantId.Equals(tenantId) && d.Confirmed);
            var driverReportItems = drivers.ToDictionary(d => d, d => new List<double[]>() {
                    new double[reportColumnsTemplates.Count], new double[reportColumnsTemplates.Count], new double[reportColumnsTemplates.Count], new double[reportColumnsTemplates.Count]});
            foreach (var reportValuesKvp in reportsValues)
            {
                var reportType = reportValuesKvp.Key;
                var databaseDrivers = GetDatabaseDriverNamesDict(tenantId, reportType);
                foreach (var driverValues in reportValuesKvp.Value)
                {
                    var reportDriverName = driverValues.Key.Trim().ToLower();
                    if (!databaseDrivers.TryGetValue(reportDriverName, out var driver))
                        throw new Exception($"Missing report driver name [{reportDriverName}] in report {reportType}");
                    if (!driverReportItems.TryGetValue(driver, out var driverReportItem))
                        throw new Exception($"Missing report driver [{driver.Id}] in report {reportType}");
                    double[]? values = null;
                    switch (reportType)
                    {
                        case ExternalReportType.Bolt:
                            values = driverReportItem[0];
                            break;
                        case ExternalReportType.Uber:
                            values = driverReportItem[1];
                            break;
                        case ExternalReportType.Freenow:
                            values = driverReportItem[2];
                            break;
                    }
                    if (values == null) continue;
                    for (int i = 0; i < values.Length; i++)
                        values[i] += Math.Round(driverValues.Value[i], 2);
                }
            }
            foreach (var driverReportItemKvp in driverReportItems)
            {
                var values0 = driverReportItemKvp.Value[0];
                var values1 = driverReportItemKvp.Value[1];
                var values2 = driverReportItemKvp.Value[2];
                var values3 = driverReportItemKvp.Value[3];
                for (int i = 0; i < reportColumnsTemplates.Count; i++)
                    values3[i] = values0[i] + values1[i] + values2[i];
            }
            return driverReportItems;
        }

        private void AddReportValues(List<string[]> driverReportHeaders, Dictionary<Driver, List<double[]>> driverReportItems)
        {
            var settlementType1 = _dbContext.SettlementTypes.FirstOrDefault(st => st.Id == 10);
            var settlementType2 = _dbContext.SettlementTypes.FirstOrDefault(st => st.Id == 9);
            if (settlementType1 == null || settlementType2 == null)
                return;
            var columnNames = driverReportHeaders[3].ToList();
            var column1Idx = columnNames.IndexOf("Konto spółki");
            var column2Idx = columnNames.IndexOf("Utarg brutto");
            if (column1Idx < 0)
                throw new Exception($"Missing report header 'Konto spółki'");
            if (column2Idx < 0)
                throw new Exception($"Missing report header 'Utarg brutto'");
            driverReportHeaders.Add(new string[]
            {
                settlementType1.TypeName, settlementType2.TypeName
            });
            foreach (var driverReportItemKvp in driverReportItems)
            {
                var sourceValues = driverReportItemKvp.Value[3];
                driverReportItemKvp.Value.Add(new double[]
                {
                    sourceValues[column1Idx] * (settlementType1.Debit?-1.0:1.0) * settlementType1.Factor,
                    sourceValues[column2Idx] * (settlementType2.Debit?-1.0:1.0) * settlementType2.Factor
                });
            }
        }

        private void AddCarRentalCosts(List<string[]> driverReportHeaders, Dictionary<Driver, List<double[]>> driverReportItems, int tenantId, int yearNumber, int weekNumber)
        {
            var settlementType = _dbContext.SettlementTypes.FirstOrDefault(st => st.Id == 1);
            if (settlementType == null)
                return;
            var idx = driverReportHeaders.Count;
            driverReportHeaders.Add(new string[] { settlementType.TypeName });
            foreach (var driverReportItemKvp in driverReportItems)
                driverReportItemKvp.Value.Add(new double[] { 0 });
            var driverReportItemsDict = driverReportItems.ToDictionary(itemKvp => itemKvp.Key.Id, itemKvp => itemKvp.Value);

            var tariffPlanAssignments = _dbContext.TariffPlanAssignments.Where(fb => fb.TenantId.Equals(tenantId) && fb.YearNumber.Equals(yearNumber) && fb.WeekNumber.Equals(weekNumber)).Include(fb => fb.TariffPlan).ToList();
            foreach (var tariffPlanAssignment in tariffPlanAssignments)
            {
                var carRentalCost = tariffPlanAssignment.TariffPlan?.WeeklyRate;
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
                if (!driverReportItemsDict.TryGetValue(tariffPlanAssignment.DriverId, out var values))
                    throw new Exception($"Missing report driverId [{tariffPlanAssignment.DriverId}]");
                values[idx][0] += carRentalCost.Value * (settlementType.Debit ? -1.0 : 1.0) * settlementType.Factor * numberOfDays / 7.0;
            }
            var settlementItems = _dbContext.SettlementItems.Where(si => si.TenantId.Equals(tenantId) && si.YearNumber.Equals(yearNumber) && si.WeekNumber.Equals(weekNumber)
                && si.SettlementTypeId.Equals(settlementType.Id));
            foreach (var settlementItem in settlementItems)
            {
                if (!driverReportItemsDict.TryGetValue(settlementItem.DriverId, out var values))
                    throw new Exception($"Missing SettlementItem.Driver for [{settlementItem.Id}]");
                if (settlementItem.SettlementType == null)
                    throw new Exception($"Missing SettlementItemType for ID [{settlementItem.Id}]");
                values[idx][0] += settlementItem.Value * (settlementItem.SettlementType.Debit ? -1.0 : 1.0) * settlementItem.SettlementType.Factor;
            }
        }

        private void AddFuelCosts(List<string[]> driverReportHeaders, Dictionary<Driver, List<double[]>> driverReportItems, int tenantId, int yearNumber, int weekNumber)
        {
            var settlementType = _dbContext.SettlementTypes.FirstOrDefault(st => st.Id == 2);
            if (settlementType == null)
                return;
            var idx = driverReportHeaders.Count;
            driverReportHeaders.Add(new string[] { settlementType.TypeName });
            foreach (var driverReportItemKvp in driverReportItems)
                driverReportItemKvp.Value.Add(new double[] { 0 });
            var driverReportItemsDict = driverReportItems.ToDictionary(itemKvp => itemKvp.Key.Id, itemKvp => itemKvp.Value);

            var fuelBillsLookup = _dbContext.FuelBills.Where(fb => fb.TenantId.Equals(tenantId) && fb.YearNumber.Equals(yearNumber) && fb.WeekNumber.Equals(weekNumber)
                && fb.Status == FuelBillStatus.Acccepted).ToLookup(fb => fb.DriverId, fb => fb.Cost);
            foreach (var driverFuelBillsLookupItem in fuelBillsLookup)
            {
                if (!driverReportItemsDict.TryGetValue(driverFuelBillsLookupItem.Key, out var values))
                    throw new Exception($"Missing FuelBills.Driver for [{driverFuelBillsLookupItem.Key}]");
                values[idx][0] += driverFuelBillsLookupItem.Sum(item => item) * (settlementType.Debit ? -1.0 : 1.0) * settlementType.Factor;
            }
            var settlementItems = _dbContext.SettlementItems.Where(si => si.TenantId.Equals(tenantId) && si.YearNumber.Equals(yearNumber) && si.WeekNumber.Equals(weekNumber)
                && si.SettlementTypeId.Equals(settlementType.Id));
            foreach (var settlementItem in settlementItems)
            {
                if (!driverReportItemsDict.TryGetValue(settlementItem.DriverId, out var values))
                    throw new Exception($"Missing SettlementItem.Driver for [{settlementItem.Id}]");
                if (settlementItem.SettlementType == null)
                    throw new Exception($"Missing SettlementItemType for ID [{settlementItem.Id}]");
                values[idx][0] += settlementItem.Value * (settlementItem.SettlementType.Debit ? -1.0 : 1.0) * settlementItem.SettlementType.Factor;
            }
        }

        private void AddOtherSettlementItems(List<string[]> driverReportHeaders, Dictionary<Driver, List<double[]>> driverReportItems, int tenantId, int yearNumber, int weekNumber)
        {
            var settlementTypes = _dbContext.SettlementTypes.Where(
                st => st.IncludeInReport).ToList();
            var idx = driverReportHeaders.Count;
            driverReportHeaders.Add(settlementTypes.Select(st => st.TypeName).ToArray());
            foreach (var driverReportItemKvp in driverReportItems)
                driverReportItemKvp.Value.Add(new double[settlementTypes.Count]);
            var driverReportItemsDict = driverReportItems.ToDictionary(itemKvp => itemKvp.Key.Id, itemKvp => itemKvp.Value);
            var settlementTypeIdx = new Dictionary<int, int>();
            foreach (var settlementType in settlementTypes)
                settlementTypeIdx.Add(settlementType.Id, settlementTypeIdx.Count);
            var settlementItems = _dbContext.SettlementItems.Where(si => si.TenantId.Equals(tenantId)
                && si.YearNumber.Equals(yearNumber) && si.WeekNumber.Equals(weekNumber)
                && si.SettlementType != null && si.SettlementType.Id != 1 && si.SettlementType.Id != 2
                && si.SettlementType.Id != 9 && si.SettlementType.Id != 10).Include(si => si.SettlementType).ToList();
            var nsloopup = settlementItems.ToLookup(si => si.DriverId);
            foreach (var driverSettlementItems in nsloopup)
            {
                var driverId = driverSettlementItems.First().DriverId;
                if (!driverReportItemsDict.TryGetValue(driverId, out var values))
                    throw new Exception($"Missing SettlementItem.Driver ID [{driverId}]");
                var lastValues = values[idx];
                foreach (var settlementItem in driverSettlementItems)
                {
                    if (settlementItem.SettlementType == null || !settlementItem.SettlementType.IncludeInReport)
                        continue;
                    lastValues[settlementTypeIdx[settlementItem.SettlementType.Id]] += settlementItem.Value * (settlementItem.SettlementType.Debit ? -1.0 : 1.0) * settlementItem.SettlementType.Factor;
                }
            }
        }

        private void AddPartnershipFee(List<string[]> driverReportHeaders, Dictionary<Driver, List<double[]>> driverReportItems, Tenant tenant, int yearNumber, int weekNumber)
        {
            var settlementType = _dbContext.SettlementTypes.FirstOrDefault(st => st.Id == 19);
            if (settlementType == null)
                return;
            var billingWeek = _dbContext.BillingWeeks.FirstOrDefault(bw => bw.TenantId.Equals(tenant.Id)
                && bw.YearNumber.Equals(yearNumber) && bw.WeekNumber.Equals(weekNumber));
            if (billingWeek == null || billingWeek.Status != BillingWeekStatus.Closed || !billingWeek.ApplicationFeeUnitPrice.HasValue || !billingWeek.NumberOfDrivers.HasValue)
                return;
            if (driverReportHeaders.Count < 4)
                return;
            var columnNames = driverReportHeaders[3].ToList();
            var columnIdx = columnNames.IndexOf("Utarg brutto");
            if (columnIdx < 0)
                return;
            driverReportHeaders.Add(new[] { settlementType.TypeName });
            foreach (var driverReportItemKvp in driverReportItems)
            {
                if (driverReportItemKvp.Key.PartnershipFee.HasValue && driverReportItemKvp.Key.PartnershipFee.Value>0)
                {
                    var val = driverReportItemKvp.Value[3][columnIdx] * driverReportItemKvp.Key.PartnershipFee.Value / 100.00;
                    driverReportItemKvp.Value.Add(new double[] { val * (settlementType.Debit ? -1.0 : 1.0) * settlementType.Factor });
                }
                else
                    driverReportItemKvp.Value.Add(new double[] { 0.0 });
            }
        }

        private void AddApplicationFee(List<string[]> driverReportHeaders, Dictionary<Driver, List<double[]>> driverReportItems, Tenant tenant, int yearNumber, int weekNumber)
        {
            var settlementType = _dbContext.SettlementTypes.FirstOrDefault(st => st.Id == 8);
            if (settlementType == null)
                return;
            var billingWeek = _dbContext.BillingWeeks.FirstOrDefault(bw => bw.TenantId.Equals(tenant.Id)
                && bw.YearNumber.Equals(yearNumber) && bw.WeekNumber.Equals(weekNumber));
            if (billingWeek == null || billingWeek.Status != BillingWeekStatus.Closed || !billingWeek.ApplicationFeeUnitPrice.HasValue || !billingWeek.NumberOfDrivers.HasValue)
                return;
            driverReportHeaders.Add(new[] { settlementType.TypeName });
            foreach(var driverReportItemKvp in driverReportItems)
            {
                if (tenant.ApplicationFeeChargeDriver && driverReportItemKvp.Key.ApplicationFeeChargeDriver && driverReportItemKvp.Value.Any(item => item.Any(i => i > 0.0)))
                    driverReportItemKvp.Value.Add(new double[] { billingWeek.ApplicationFeeUnitPrice.Value * (settlementType.Debit ? -1.0 : 1.0) * settlementType.Factor });
                else
                    driverReportItemKvp.Value.Add(new double[] { 0.0 });
            }
        }

        private void AddTotal(List<string[]> driverReportHeaders, Dictionary<Driver, List<double[]>> driverReportItems)
        {
            var idx = driverReportHeaders.Count;
            driverReportHeaders.Add(new string[] { "TOTAL" });
            foreach (var driverReportItemKvp in driverReportItems)
                driverReportItemKvp.Value.Add(new double[] { driverReportItemKvp.Value.Skip(4).Sum(item => item.Sum()) });
        }
        #endregion

        public GetTenantFuelBillsResponse GetTenantFuelBills(GetTenantFuelBillsRequest request)
        {
            try
            {
                var tenant = CheckPriviledgesAndGetApplicationInstance(request);
                var fuelBills = _dbContext.FuelBills.Where(fb => fb.TenantId.Equals(tenant.Id)
                        && (!request.DriverId.HasValue || request.DriverId.Value.Equals(fb.DriverId))
                        && fb.YearNumber.Equals(request.YearNumber) && fb.WeekNumber.Equals(request.WeekNumber)).ToList();
                var billingWeek = _dbContext.BillingWeeks.FirstOrDefault(bw => bw.TenantId.Equals(tenant.Id)
                    && bw.YearNumber.Equals(request.YearNumber) && bw.WeekNumber.Equals(request.WeekNumber));
                return new GetTenantFuelBillsResponse()
                {
                    FuelBills = fuelBills,
                    BillingWeekStatus = billingWeek==null?BillingWeekStatus.NotExists:billingWeek.Status,
                    Result = true
                };
            }
            catch (Exception ex)
            {
                _tenantServiceLogger.LogError(ex, ex.Message);
                return new GetTenantFuelBillsResponse()
                {
                    Result = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        public UpdateTenantFuelBillResponse UpdateTenantFuelBill(UpdateTenantFuelBillRequest request)
        {
            try
            {
                var tenant = CheckPriviledgesAndGetApplicationInstance(request);
                var fuelBill = _dbContext.FuelBills.FirstOrDefault(b => b.Id.Equals(request.FuelBill.Id));
                if (fuelBill == null)
                    throw new Exception($"Fuel bill with ID={request.FuelBill.Id} does not exist");
                if (!fuelBill.TenantId.Equals(tenant.Id))
                    throw new Exception($"Appeared problem with ownership of fuel bill with ID={request.FuelBill.Id}");
                fuelBill.Date = request.FuelBill.Date;
                fuelBill.ReferenceNumber = request.FuelBill.ReferenceNumber;
                fuelBill.Cost = request.FuelBill.Cost;
                fuelBill.Status = request.FuelBill.Status;
                _dbContext.SaveChanges();
                return new UpdateTenantFuelBillResponse()
                {
                    Result = true,
                };
            }
            catch (Exception ex)
            {
                _tenantServiceLogger.LogError(ex, ex.Message);
                return new UpdateTenantFuelBillResponse()
                {
                    Result = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        public GetDriverFuelBillsInfosResponse GetDriverFuelBillsInfos(GetDriverFuelBillsInfosRequest request)
        {
            try
            {
                var tenant = CheckPriviledgesAndGetApplicationInstance(request);
                var drivers = _dbContext.Drivers.Include(d => d.FuelBills.Where(fb => fb.YearNumber.Equals(request.YearNumber) && fb.WeekNumber.Equals(request.WeekNumber))).Where(d => d.TenantId.Equals(tenant.Id) && d.FuelBills.Count(fb=>fb.YearNumber.Equals(request.YearNumber) && fb.WeekNumber.Equals(request.WeekNumber)) > 0);
                var driverFuelBillInfos = new List<DriverFuelBillsInfo>();
                foreach (var driver in drivers)
                {
                    driverFuelBillInfos.Add(
                        new DriverFuelBillsInfo()
                        {
                            DriverId = driver.Id,
                            DriverName = driver.CompanyName,
                            Requested = driver.FuelBills.Count(fb => fb.Status == FuelBillStatus.Requested),
                            Accepted = driver.FuelBills.Count(fb => fb.Status == FuelBillStatus.Acccepted),
                            Rejected = driver.FuelBills.Count(fb => fb.Status == FuelBillStatus.Rejected)
                        });
                }
                var billingWeek = _dbContext.BillingWeeks.FirstOrDefault(bw => bw.TenantId.Equals(tenant.Id)
                    && bw.YearNumber.Equals(request.YearNumber) && bw.WeekNumber.Equals(request.WeekNumber));
                return new GetDriverFuelBillsInfosResponse()
                {
                    DriverFuelBillsInfos = driverFuelBillInfos,
                    BillingWeekStatus = billingWeek==null?BillingWeekStatus.NotExists:billingWeek.Status,
                    Result = true
                };
            }
            catch (Exception ex)
            {
                _tenantServiceLogger.LogError(ex, ex.Message);
                return new GetDriverFuelBillsInfosResponse()
                {
                    Result = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        public DownloadTenantFuelBillPhotoResponse DownloadTenantFuelBillPhoto(DownloadTenantFuelBillPhotoRequest request)
        {
            try
            {
                var tenant = CheckPriviledgesAndGetApplicationInstance(request);
                var fuelBill = _dbContext.FuelBills.FirstOrDefault(fb => fb.Id == request.FuelBillId);
                if (fuelBill == null)
                    throw new Exception($"Fuel bill with ID={request.FuelBillId} does not exist");
                if (!fuelBill.TenantId.Equals(tenant.Id))
                    throw new Exception($"Appeared problem with ownership of fuel bill with ID={request.FuelBillId}");
                var photosFolder = Path.Combine(
                        _photosConfiguration.PhotosFolder,
                        $"{tenant.Id.ToString("#000")}",
                        $"{fuelBill.YearNumber.ToString("#0000")}.{fuelBill.WeekNumber.ToString("#00")}");
                var photoFilePath = Path.Combine(
                    photosFolder, $"{fuelBill.Id}.jpeg");
                if (!File.Exists(photoFilePath ))
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
                        return new DownloadTenantFuelBillPhotoResponse()
                        {
                            PhotoContent = ms.ToArray(),
                            Result = true
                        };
                    }
                }               
            }
            catch (Exception ex)
            {
                _tenantServiceLogger.LogError(ex, ex.Message);
                return new DownloadTenantFuelBillPhotoResponse()
                {
                    Result = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        public GetTariffPlansResponse GetTariffPlans(GetTariffPlansRequest request)
        {
            try
            {
                var tenant = CheckPriviledgesAndGetApplicationInstance(request);
                return new GetTariffPlansResponse()
                {
                    Result = true,
                    TariffPlans = request.IncludeDeleted
                                ? _dbContext.TariffPlans.Where(d => d.TenantId.Equals(tenant.Id)).ToList()
                                : _dbContext.TariffPlans.Where(d => d.TenantId.Equals(tenant.Id) && d.Enabled).ToList()
                };
            }
            catch (Exception ex)
            {
                _tenantServiceLogger.LogError(ex, ex.Message);
                return new GetTariffPlansResponse()
                {
                    Result = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        public AddTariffPlanResponse AddTariffPlan(AddTariffPlanRequest request)
        {
            try
            {
                var tenant = CheckPriviledgesAndGetApplicationInstance(request);
                request.TariffPlan.TenantId = tenant.Id;
                _dbContext.TariffPlans.Add(request.TariffPlan);
                _dbContext.SaveChanges();
                return new AddTariffPlanResponse()
                {
                    Result = true,
                    TariffPlanId = request.TariffPlan.Id
                };
            }
            catch (Exception ex)
            {
                _tenantServiceLogger.LogError(ex, ex.Message);
                return new AddTariffPlanResponse()
                {
                    Result = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        public UpdateTariffPlanResponse UpdateTariffPlan(UpdateTariffPlanRequest request)
        {
            try
            {
                var tenant = CheckPriviledgesAndGetApplicationInstance(request);
                var car = _dbContext.TariffPlans.FirstOrDefault(d => d.TenantId.Equals(tenant.Id) && d.Id.Equals(request.TariffPlan.Id));
                if (car == null)
                    return new UpdateTariffPlanResponse()
                    {
                        Result = false,
                        ErrorMessage = $"Car with key {request.TariffPlan.Id} does not exist"
                    };
                car.Name = request.TariffPlan.Name;
                car.WeeklyRate = request.TariffPlan.WeeklyRate;
                car.Enabled = request.TariffPlan.Enabled;  
                _dbContext.SaveChanges();
                return new UpdateTariffPlanResponse()
                {
                    Result = true
                };
            }
            catch (Exception ex)
            {
                _tenantServiceLogger.LogError(ex, ex.Message);
                return new UpdateTariffPlanResponse()
                {
                    Result = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        public DeleteTariffPlanResponse DeleteTariffPlan(DeleteTariffPlanRequest request)
        {
            try
            {
                var tenant = CheckPriviledgesAndGetApplicationInstance(request);
                var car = _dbContext.TariffPlans.FirstOrDefault(d => d.TenantId.Equals(tenant.Id) && d.Id.Equals(request.TariffPlanId));
                if (car == null)
                    return new DeleteTariffPlanResponse()
                    {
                        Result = false,
                        ErrorMessage = $"Car with key {request.TariffPlanId} does not exist"
                    };
                _dbContext.TariffPlans.Remove(car);
                _dbContext.SaveChanges();
                return new DeleteTariffPlanResponse()
                {
                    Result = true
                };
            }
            catch (Exception ex)
            {
                _tenantServiceLogger.LogError(ex, ex.Message);
                return new DeleteTariffPlanResponse()
                {
                    Result = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        public GetTariffPlanAssignmentsResponse GetTariffPlanAssignments(GetTariffPlanAssignmentsRequest request)
        {
            try
            {
                var tenant = CheckPriviledgesAndGetApplicationInstance(request);
                var billingWeek = _dbContext.BillingWeeks.FirstOrDefault(bw => bw.TenantId.Equals(tenant.Id)
                    && bw.YearNumber.Equals(request.YearNumber) && bw.WeekNumber.Equals(request.WeekNumber));
                return new GetTariffPlanAssignmentsResponse()
                {
                    Result = true,
                    BillingWeekStatus = billingWeek==null?BillingWeekStatus.NotExists:billingWeek.Status,
                    TariffPlanAssignments = _dbContext.TariffPlanAssignments.Where(d => d.TenantId.Equals(tenant.Id) 
                        && d.YearNumber.Equals(request.YearNumber) && d.WeekNumber.Equals(request.WeekNumber)).Include(d=>d.TariffPlan).Include(d=>d.Driver).ToList()
                };
            }
            catch (Exception ex)
            {
                _tenantServiceLogger.LogError(ex, ex.Message);
                return new GetTariffPlanAssignmentsResponse()
                {
                    Result = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        public AddTariffPlanAssignmentResponse AddTariffPlanAssignment(AddTariffPlanAssignmentRequest request)
        {
            try
            {
                var tenant = CheckPriviledgesAndGetApplicationInstance(request);
                request.TariffPlanAssignment.TenantId = tenant.Id;
                _dbContext.TariffPlanAssignments.Add(request.TariffPlanAssignment);
                _dbContext.SaveChanges();
                return new AddTariffPlanAssignmentResponse()
                {
                    Result = true,
                    TariffPlanAssignmentId = request.TariffPlanAssignment.Id
                };
            }
            catch (Exception ex)
            {
                _tenantServiceLogger.LogError(ex, ex.Message);
                return new AddTariffPlanAssignmentResponse()
                {
                    Result = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        public UpdateTariffPlanAssignmentResponse UpdateTariffPlanAssignment(UpdateTariffPlanAssignmentRequest request)
        {
            try
            {
                var tenant = CheckPriviledgesAndGetApplicationInstance(request);
                var tariffPlanAssignment = _dbContext.TariffPlanAssignments.FirstOrDefault(d => d.Id.Equals(request.TariffPlanAssignment.Id));
                if (tariffPlanAssignment == null)
                    throw new Exception($"Tariff plan assignment with key {request.TariffPlanAssignment.Id} does not exist");
                if (!tariffPlanAssignment.TenantId.Equals(tenant.Id))
                    throw new Exception($"Appeared problem with ownership of tariff plan assignment with ID={request.TariffPlanAssignment.Id}");
                tariffPlanAssignment.DriverId = request.TariffPlanAssignment.DriverId;
                tariffPlanAssignment.WeekNumber = request.TariffPlanAssignment.WeekNumber;
                tariffPlanAssignment.YearNumber = request.TariffPlanAssignment.YearNumber;
                tariffPlanAssignment.TariffPlanId = request.TariffPlanAssignment.TariffPlanId;
                tariffPlanAssignment.Cost = request.TariffPlanAssignment.Cost;
                tariffPlanAssignment.Monday = request.TariffPlanAssignment.Monday;
                tariffPlanAssignment.Tuesday = request.TariffPlanAssignment.Tuesday;
                tariffPlanAssignment.Wednesday = request.TariffPlanAssignment.Wednesday;
                tariffPlanAssignment.Thursday = request.TariffPlanAssignment.Thursday;
                tariffPlanAssignment.Friday = request.TariffPlanAssignment.Friday;
                tariffPlanAssignment.Saturday = request.TariffPlanAssignment.Saturday;
                tariffPlanAssignment.Sunday = request.TariffPlanAssignment.Sunday;
                tariffPlanAssignment.PeriodicType = request.TariffPlanAssignment.PeriodicType;
                _dbContext.SaveChanges();
                return new UpdateTariffPlanAssignmentResponse()
                {
                    Result = true
                };
            }
            catch (Exception ex)
            {
                _tenantServiceLogger.LogError(ex, ex.Message);
                return new UpdateTariffPlanAssignmentResponse()
                {
                    Result = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        public DeleteTariffPlanAssignmentResponse DeleteTariffPlanAssignment(DeleteTariffPlanAssignmentRequest request)
        {
            try
            {
                var tenant = CheckPriviledgesAndGetApplicationInstance(request);
                var tariffPlanAssignment = _dbContext.TariffPlanAssignments.FirstOrDefault(d => d.Id.Equals(request.TariffPlanAssignmentId));
                if (tariffPlanAssignment == null)
                    throw new Exception($"Tariff plan assignment with key {request.TariffPlanAssignmentId} does not exist");
                if (!tariffPlanAssignment.TenantId.Equals(tenant.Id))
                    throw new Exception($"Appeared problem with ownership of tariff plan assignment with ID={request.TariffPlanAssignmentId}");
                _dbContext.TariffPlanAssignments.Remove(tariffPlanAssignment);
                _dbContext.SaveChanges();
                return new DeleteTariffPlanAssignmentResponse()
                {
                    Result = true
                };
            }
            catch (Exception ex)
            {
                _tenantServiceLogger.LogError(ex, ex.Message);
                return new DeleteTariffPlanAssignmentResponse()
                {
                    Result = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        public CopyTariffPlanAssignmentsFromPreviousWeekResponse CopyTariffPlanAssignmentsFromPreviousWeek(CopyTariffPlanAssignmentsFromPreviousWeekRequest request)
        {
            try
            {
                var tenant = CheckPriviledgesAndGetApplicationInstance(request);
                if (_dbContext.TariffPlanAssignments.Any(tpa => tpa.TenantId.Equals(tenant.Id) 
                    && tpa.YearNumber.Equals(request.YearNumber) && tpa.WeekNumber.Equals(request.WeekNumber)))
                    throw new Exception($"Week {request.YearNumber}.{request.WeekNumber:#00} already has any tariff plan assignments");
                var weekNumber = request.WeekNumber - 1;
                var yearNumber = request.YearNumber;
                if (weekNumber<1)
                {
                    weekNumber = 52;
                    yearNumber--;
                }
                var tariffPlanAssignments = _dbContext.TariffPlanAssignments.Where(tpa => tpa.TenantId.Equals(tenant.Id) 
                    && tpa.YearNumber.Equals(yearNumber) && tpa.WeekNumber.Equals(weekNumber)
                    && tpa.PeriodicType!=PeriodicType.None);
                if (tariffPlanAssignments==null || tariffPlanAssignments.Count()==0)
                {
                    return new CopyTariffPlanAssignmentsFromPreviousWeekResponse()
                    {
                        Copied = false,
                        Result = true
                    };
                }
                foreach(var tariffPlanAssignment in tariffPlanAssignments)
                {
                    var newTariffPlanAssignment = new TariffPlanAssignment()
                    { 
                        YearNumber = request.YearNumber,
                        WeekNumber = request.WeekNumber,
                        TenantId = tariffPlanAssignment.TenantId,
                        DriverId = tariffPlanAssignment.DriverId,
                        TariffPlanId = tariffPlanAssignment.TariffPlanId,
                        PeriodicType = tariffPlanAssignment.PeriodicType,
                        Cost = tariffPlanAssignment.Cost,
                        Monday = tariffPlanAssignment.PeriodicType==PeriodicType.TariffPlanWithoutDetails?true:tariffPlanAssignment.Monday,
                        Tuesday = tariffPlanAssignment.PeriodicType == PeriodicType.TariffPlanWithoutDetails ? true : tariffPlanAssignment.Tuesday,
                        Wednesday = tariffPlanAssignment.PeriodicType == PeriodicType.TariffPlanWithoutDetails ? true : tariffPlanAssignment.Wednesday,
                        Thursday = tariffPlanAssignment.PeriodicType == PeriodicType.TariffPlanWithoutDetails ? true : tariffPlanAssignment.Thursday,
                        Friday = tariffPlanAssignment.PeriodicType == PeriodicType.TariffPlanWithoutDetails ? true : tariffPlanAssignment.Friday,
                        Saturday = tariffPlanAssignment.PeriodicType == PeriodicType.TariffPlanWithoutDetails ? true : tariffPlanAssignment.Saturday,
                        Sunday = tariffPlanAssignment.PeriodicType == PeriodicType.TariffPlanWithoutDetails ? true : tariffPlanAssignment.Sunday
                    };
                    _dbContext.TariffPlanAssignments.Add(newTariffPlanAssignment);
                }
                _dbContext.SaveChanges();
                return new CopyTariffPlanAssignmentsFromPreviousWeekResponse()
                {
                    Copied = true,
                    Result = true
                };
            }
            catch (Exception ex)
            {
                _tenantServiceLogger.LogError(ex, ex.Message);
                return new CopyTariffPlanAssignmentsFromPreviousWeekResponse()
                {
                    Result = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        public GetSettlementItemsResponse GetSettlementItems(GetSettlementItemsRequest request)
        {
            try
            {
                var tenant = CheckPriviledgesAndGetApplicationInstance(request);
                var billingWeek = _dbContext.BillingWeeks.FirstOrDefault(bw => bw.TenantId.Equals(tenant.Id)
                    && bw.YearNumber.Equals(request.YearNumber) && bw.WeekNumber.Equals(request.WeekNumber));
                return new GetSettlementItemsResponse()
                {
                    Result = true,
                    BillingWeekStatus = billingWeek==null?BillingWeekStatus.NotExists:billingWeek.Status,
                    SettlementItems = _dbContext.SettlementItems.Where(d => d.TenantId.Equals(tenant.Id)
                        && d.YearNumber.Equals(request.YearNumber) && d.WeekNumber.Equals(request.WeekNumber)).Include(d => d.SettlementType).Include(d => d.Driver).ToList()
                };
            }
            catch (Exception ex)
            {
                _tenantServiceLogger.LogError(ex, ex.Message);
                return new GetSettlementItemsResponse()
                {
                    Result = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        public AddSettlementItemResponse AddSettlementItem(AddSettlementItemRequest request)
        {
            try
            {
                var tenant = CheckPriviledgesAndGetApplicationInstance(request);
                request.SettlementItem.TenantId = tenant.Id;
                _dbContext.SettlementItems.Add(request.SettlementItem);
                _dbContext.SaveChanges();
                return new AddSettlementItemResponse()
                {
                    Result = true,
                    SettlementItemId = request.SettlementItem.Id
                };
            }
            catch (Exception ex)
            {
                _tenantServiceLogger.LogError(ex, ex.Message);
                return new AddSettlementItemResponse()
                {
                    Result = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        public UpdateSettlementItemResponse UpdateSettlementItem(UpdateSettlementItemRequest request)
        {
            try
            {
                var tenant = CheckPriviledgesAndGetApplicationInstance(request);
                var settlementItem = _dbContext.SettlementItems.FirstOrDefault(d => d.Id.Equals(request.SettlementItem.Id));
                if (settlementItem == null)
                    throw new Exception($"Settlement item with key {request.SettlementItem.Id} does not exist");
                if (!settlementItem.TenantId.Equals(tenant.Id))
                    throw new Exception($"Appeared problem with ownership of settlement item with ID={request.SettlementItem.Id}");
                settlementItem.DriverId = request.SettlementItem.DriverId;
                settlementItem.WeekNumber = request.SettlementItem.WeekNumber;
                settlementItem.YearNumber = request.SettlementItem.YearNumber;
                settlementItem.SettlementTypeId = request.SettlementItem.SettlementTypeId;
                settlementItem.Value =  request.SettlementItem.Value;
                settlementItem.Periodic = request.SettlementItem.Periodic;
                if (request.SettlementItem.NumberOfTimes.HasValue)
                    settlementItem.NumberOfTimes = request.SettlementItem.NumberOfTimes.Value;
                else
                    settlementItem.NumberOfTimes = null;
                _dbContext.SaveChanges();
                return new UpdateSettlementItemResponse()
                {
                    Result = true
                };
            }
            catch (Exception ex)
            {
                _tenantServiceLogger.LogError(ex, ex.Message);
                return new UpdateSettlementItemResponse()
                {
                    Result = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        public DeleteSettlementItemResponse DeleteSettlementItem(DeleteSettlementItemRequest request)
        {
            try
            {
                var tenant = CheckPriviledgesAndGetApplicationInstance(request);
                var settlementItem = _dbContext.SettlementItems.FirstOrDefault(d => d.Id.Equals(request.SettlementItemId));
                if (settlementItem == null)
                    throw new Exception($"Settlement item with key {request.SettlementItemId} does not exist");
                if (!settlementItem.TenantId.Equals(tenant.Id))
                    throw new Exception($"Appeared problem with ownership of settlement item with ID={request.SettlementItemId}");
                _dbContext.SettlementItems.Remove(settlementItem);
                _dbContext.SaveChanges();
                return new DeleteSettlementItemResponse()
                {
                    Result = true
                };
            }
            catch (Exception ex)
            {
                _tenantServiceLogger.LogError(ex, ex.Message);
                return new DeleteSettlementItemResponse()
                {
                    Result = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        public CopySettlementItemsFromPreviousWeekResponse CopySettlementItemsFromPreviousWeek(CopySettlementItemsFromPreviousWeekRequest request)
        {
            try
            {
                var tenant = CheckPriviledgesAndGetApplicationInstance(request);
                if (_dbContext.SettlementItems.Any(tpa => tpa.TenantId.Equals(tenant.Id)
                    && tpa.YearNumber.Equals(request.YearNumber) && tpa.WeekNumber.Equals(request.WeekNumber)))
                    throw new Exception($"Week {request.YearNumber}.{request.WeekNumber:#00} already has any settlement items");
                var weekNumber = request.WeekNumber - 1;
                var yearNumber = request.YearNumber;
                if (weekNumber < 1)
                {
                    weekNumber = 52;
                    yearNumber--;
                }
                var settlementItems = _dbContext.SettlementItems.Where(tpa => tpa.TenantId.Equals(tenant.Id)
                    && tpa.YearNumber.Equals(yearNumber) && tpa.WeekNumber.Equals(weekNumber)
                    && tpa.Periodic);
                if (settlementItems == null || settlementItems.Count() == 0)
                {
                    return new CopySettlementItemsFromPreviousWeekResponse()
                    {
                        Copied = false,
                        Result = true
                    };
                }
                foreach (var settlementItem in settlementItems)
                {
                    var newSettlementItem = new SettlementItem()
                    {
                        YearNumber = request.YearNumber,
                        WeekNumber = request.WeekNumber,
                        TenantId = settlementItem.TenantId,
                        DriverId = settlementItem.DriverId,
                        SettlementTypeId = settlementItem.SettlementTypeId,
                        Periodic = settlementItem.Periodic,
                        Value = settlementItem.Value
                    };
                    if (settlementItem.NumberOfTimes.HasValue)
                    {
                        if (settlementItem.NumberOfTimes.Value > 1)
                            newSettlementItem.NumberOfTimes = settlementItem.NumberOfTimes.Value - 1;
                        else if (settlementItem.NumberOfTimes.Value == 1) 
                            newSettlementItem.Periodic = false;
                    }
                    _dbContext.SettlementItems.Add(newSettlementItem);
                }
                _dbContext.SaveChanges();
                return new CopySettlementItemsFromPreviousWeekResponse()
                {
                    Copied = true,
                    Result = true
                };
            }
            catch (Exception ex)
            {
                _tenantServiceLogger.LogError(ex, ex.Message);
                return new CopySettlementItemsFromPreviousWeekResponse()
                {
                    Result = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        public GetSettlementTypesResponse GetSettlementTypes(GetSettlementTypesRequest request)
        {
            try
            {
                var tenant = CheckPriviledgesAndGetApplicationInstance(request);
                return new GetSettlementTypesResponse()
                {
                    Result = true,
                    SettlementTypes = _dbContext.SettlementTypes.Where(d => !d.TenantId.HasValue || d.TenantId.Equals(tenant.Id)).ToList()
                };
            }
            catch (Exception ex)
            {
                _tenantServiceLogger.LogError(ex, ex.Message);
                return new GetSettlementTypesResponse()
                {
                    Result = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        public AddSettlementTypeResponse AddSettlementType(AddSettlementTypeRequest request)
        {
            try
            {
                var tenant = CheckPriviledgesAndGetApplicationInstance(request);
                request.SettlementType.TenantId = tenant.Id;
                _dbContext.SettlementTypes.Add(request.SettlementType);
                _dbContext.SaveChanges();
                return new AddSettlementTypeResponse()
                {
                    Result = true,
                    SettlementTypeId = request.SettlementType.Id
                };
            }
            catch (Exception ex)
            {
                _tenantServiceLogger.LogError(ex, ex.Message);
                return new AddSettlementTypeResponse()
                {
                    Result = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        public UpdateSettlementTypeResponse UpdateSettlementType(UpdateSettlementTypeRequest request)
        {
            try
            {
                var tenant = CheckPriviledgesAndGetApplicationInstance(request);
                var settlementType = _dbContext.SettlementTypes.FirstOrDefault(d => d.Id.Equals(request.SettlementType.Id));
                if (settlementType == null)
                    throw new Exception($"Settlement type with key {request.SettlementType.Id} does not exist");
                if (!settlementType.TenantId.HasValue || !settlementType.TenantId.Equals(tenant.Id))
                    throw new Exception($"Appeared problem with ownership of settlement type with ID={request.SettlementType.Id}");
                settlementType.Debit = request.SettlementType.Debit;
                settlementType.Factor = request.SettlementType.Factor;
                settlementType.TypeName = request.SettlementType.TypeName;
                settlementType.Enabled = request.SettlementType.Enabled;
                _dbContext.SaveChanges();
                return new UpdateSettlementTypeResponse()
                {
                    Result = true
                };
            }
            catch (Exception ex)
            {
                _tenantServiceLogger.LogError(ex, ex.Message);
                return new UpdateSettlementTypeResponse()
                {
                    Result = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        public DeleteSettlementTypeResponse DeleteSettlementType(DeleteSettlementTypeRequest request)
        {
            try
            {
                var tenant = CheckPriviledgesAndGetApplicationInstance(request);
                var settlementType = _dbContext.SettlementTypes.FirstOrDefault(d => d.Id.Equals(request.SettlementTypeId));
                if (settlementType == null)
                    throw new Exception($"Settlement type with key {request.SettlementTypeId} does not exist");
                if (!settlementType.TenantId.HasValue || !settlementType.TenantId.Equals(tenant.Id))
                    throw new Exception($"Appeared problem with ownership of settlement type with ID={request.SettlementTypeId}");
                settlementType.Enabled = false;
                _dbContext.SaveChanges();
                return new DeleteSettlementTypeResponse()
                {
                    Result = true
                };
            }
            catch (Exception ex)
            {
                _tenantServiceLogger.LogError(ex, ex.Message);
                return new DeleteSettlementTypeResponse()
                {
                    Result = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        public ConfirmBillingWeekResponse ConfirmBillingWeek(ConfirmBillingWeekRequest request)
        {
            try
            {
                var tenant = CheckPriviledgesAndGetApplicationInstance(request);
                var billingWeek = _dbContext.BillingWeeks.FirstOrDefault(bw => bw.TenantId.Equals(tenant.Id)
                    && bw.YearNumber.Equals(request.YearNumber) && bw.WeekNumber.Equals(request.WeekNumber));
                if (billingWeek == null)
                {
                    billingWeek = new BillingWeek();
                    billingWeek.YearNumber = request.YearNumber;
                    billingWeek.WeekNumber = request.WeekNumber;
                    billingWeek.TenantId = tenant.Id;
                    _dbContext.BillingWeeks.Add(billingWeek);
                }
                billingWeek.Status = BillingWeekStatus.Closed;
                var billingDriverIds = GetBillingDriverIds(tenant.Id, request.YearNumber, request.WeekNumber);                
                billingWeek.ApplicationFeeUnitPrice = tenant.ApplicationFeeBaseValue;
                billingWeek.NumberOfDrivers = billingDriverIds.Count;
                if (tenant.ApplicationFeeChargeDriver)
                {
                    //TODO: include application fee added to Drivers
                }
                _dbContext.SaveChanges();
                return new ConfirmBillingWeekResponse()
                {
                    Result = true
                };
            } 
            catch(Exception ex)
            {
                _tenantServiceLogger.LogError(ex, ex.Message);
                return new ConfirmBillingWeekResponse()
                {
                    Result = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        public GetDriverDepositValueResponse GetDriverDepositValue(GetDriverDepositValueRequest request)
        {
            try
            {
                var tenant = CheckPriviledgesAndGetApplicationInstance(request);
                var driver = _dbContext.Drivers.FirstOrDefault(d => d.TenantId.Equals(tenant.Id) && d.Id.Equals(request.DriverId));
                if (driver == null)
                    throw new Exception($"Driver with key {request.DriverId} does not exist");
                var depositValue = driver.Deposit;
                var driverSettlements = _dbContext.SettlementItems.Where(item => item.DriverId.Equals(driver.Id) && item.SettlementTypeId == 7);
                if (driverSettlements != null && driverSettlements.Count() > 0)
                    depositValue += driverSettlements.Sum(item => item.Value);
                return new GetDriverDepositValueResponse()
                {
                    Result = true,
                    DepositValue = depositValue
                };
            }
            catch (Exception ex)
            {
                _tenantServiceLogger.LogError(ex, ex.Message);
                return new GetDriverDepositValueResponse()
                {
                    Result = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        private HashSet<int> GetBillingDriverIds(int tenantId, int yearNumber, int weekNumber)
        {
            //get drivers from reports
            var reportTemplate = ReportTemplatesHelper.GetReportTemplate(_dbContext, "Basic report", DateTime.Now);
            var reportColumnsTemplates = reportTemplate.DefReportColumns?.Where(c => string.IsNullOrWhiteSpace(c.DefValue)).ToList();
            if (reportColumnsTemplates == null || reportColumnsTemplates.Count < 1)
                throw new Exception("Missing report columns definitions in Basic report definition");
            var reportsValues = GetReportValues(tenantId, yearNumber, weekNumber);
            var driverReportItems = GetDriverReportItems(tenantId, reportColumnsTemplates, reportsValues);
            var driverIds = driverReportItems.Where(dri => dri.Value.Any(v1=>v1.Any(v2=>v2!=0.0))).Select(dri=>dri.Key.Id).ToHashSet();

            //get drivers from tariffplans
            var tariffPlanAssignmentDriverIds = _dbContext.TariffPlanAssignments.Where(tpa => tpa.TenantId.Equals(tenantId)
                && tpa.YearNumber.Equals(yearNumber) && tpa.WeekNumber.Equals(weekNumber)).Select(tpa=>tpa.DriverId).ToHashSet();
            foreach (var tariffPlanAssignmentDriverId in tariffPlanAssignmentDriverIds)
                driverIds.Add(tariffPlanAssignmentDriverId);
            //get drivers from fuell bils
            var fuelBillDriverIds = _dbContext.FuelBills.Where(fb => fb.TenantId.Equals(tenantId)
                && fb.YearNumber.Equals(yearNumber) && fb.WeekNumber.Equals(weekNumber) && fb.Status==FuelBillStatus.Acccepted).Select(fb => fb.DriverId).ToHashSet();
            foreach (var fuelBillDriverId in fuelBillDriverIds)
                driverIds.Add(fuelBillDriverId);
            //get drivers from settlements
            var settlementDriverIds = _dbContext.SettlementItems.Where(si => si.TenantId.Equals(tenantId)
                && si.YearNumber.Equals(yearNumber) && si.WeekNumber.Equals(weekNumber)).Select(fb => fb.DriverId).ToHashSet();
            foreach (var settlementDriverId in settlementDriverIds)
                driverIds.Add(settlementDriverId);
            return driverIds;
        }

        public string Ping(string ping)
        {
            return $"OK - {ping}";
        }

        public string Test()
        {
            return $"Tenants: {_dbContext.Tenants.Count()} Drivers:{_dbContext.Drivers.Count()}";
        }

        private Dictionary<string, Driver> GetDatabaseDriverNamesDict(int tenantId, ExternalReportType reportType = ExternalReportType.Unknown)
        {
#pragma warning disable CS8620
#pragma warning disable CS8714
            switch (reportType)
            {
                case ExternalReportType.Bolt:
                    return SplitAndToLowerDriverNames(_dbContext.Drivers.Where(d => d.TenantId == tenantId && !string.IsNullOrWhiteSpace(d.BoltName)).ToDictionary(d => d.BoltName, d => d));
                case ExternalReportType.Uber:
                    return SplitAndToLowerDriverNames(_dbContext.Drivers.Where(d => d.TenantId == tenantId && !string.IsNullOrWhiteSpace(d.UberName)).ToDictionary(d => d.UberName, d => d));
                case ExternalReportType.Freenow:
                    return SplitAndToLowerDriverNames(_dbContext.Drivers.Where(d => d.TenantId == tenantId && !string.IsNullOrWhiteSpace(d.FreenowName)).ToDictionary(d => d.FreenowName, d => d));
                default:
                    return SplitAndToLowerDriverNames(_dbContext.Drivers.Where(d => d.TenantId == tenantId).ToDictionary(d => d.CompanyName, d => d));
            }
#pragma warning restore CS8714
#pragma warning restore CS8620
        }

        private static Dictionary<string, Driver> SplitAndToLowerDriverNames(Dictionary<string, Driver> driversDict)
        {
            var res = new Dictionary<string, Driver>();
            foreach (var driverKvp in driversDict)
            {
                var driverNames = driverKvp.Key.Split(';', StringSplitOptions.RemoveEmptyEntries).Select(n => n.Trim().ToLower());
                foreach (var driverName in driverNames)
                {
                    if (!res.TryAdd(driverName, driverKvp.Value))
                        throw new Exception($"Duplicated driver name [{driverName}]");
                }
            }
            return res;
        }

        private Tenant CheckPriviledgesAndGetApplicationInstance(BaseRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.ApplicationInstanceId))
                throw new Exception("Invalid request");
            var applicationInstance = _dbContext.ApplicationInstances.FirstOrDefault(ai => request.ApplicationInstanceId.Equals(ai.Id));
            if (applicationInstance == null)
                throw new Exception($"ApplicationInstance {request.ApplicationInstanceId} does not exist");
            if (applicationInstance.TenantId == null || !applicationInstance.TenantId.HasValue) 
                throw new Exception("Missing priviledges to Tenant interface");
            var tenant = _dbContext.Tenants.FirstOrDefault(t => t.Id.Equals(applicationInstance.TenantId.Value));
            if (tenant == null)
                throw new Exception($"Tenant realted to {request.ApplicationInstanceId} does not exist");
            if (tenant.Blocked)
                throw new Exception($"Tenant {tenant.CompanyName} is blocked");
            if (tenant.Deleted)
                throw new Exception($"Tenant {tenant.CompanyName} does not exist");
            return tenant;
        }

        private void CollectExternalReportMetadata(int tenantId, ExternalReport externalReport)
        {
            if (externalReport.Status != ExternalReportStatus.Generated)
                return;
            var reportBody = ExternalReportBody.Deserialize(externalReport.ReportBody);
            if (reportBody == null)
                throw new Exception($"Invalid report body in {externalReport.ReportType} report for week {externalReport.YearNumber}.{externalReport.WeekNumber:#00}");
            ReportTemplatesHelper.SetReportBodyGlobalTemplates(_dbContext, reportBody, externalReport.ReportType, externalReport.DownloadDate);
            var reportDrivers = reportBody.GetDrivers()?.Select(d => d.ToLower());
            if (reportDrivers != null)
            {
                var databaseDrivers = GetDatabaseDriverNamesDict(tenantId, externalReport.ReportType);
                externalReport.NumberOfUnmatchedDrivers = reportDrivers.Count(rd => !databaseDrivers.ContainsKey(rd));
                externalReport.NotComfirmedDriversIdentifiers = databaseDrivers.Where(d => !d.Value.Confirmed && reportDrivers.Contains(d.Key)).Select(d => d.Value.Id).ToHashSet();
            }
        }
    }
}
