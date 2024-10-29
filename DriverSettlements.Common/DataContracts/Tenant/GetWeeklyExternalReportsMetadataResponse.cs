using DriverSettlements.Common.DataContracts;
using DriverSettlements.Common.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace DriverSettlements.Common.DataContracts.Tenant
{
    public class GetWeeklyExternalReportsMetadataResponse : BaseResponse
    {
        public List<ExternalReportMetadata>? Reports { get; set; }
        public List<string>? TenantReportsValueNames { get; set; } = null;
        public Dictionary<ExternalReportType, List<double>>? TenantReportsValues { get; set; } = null;
        public Dictionary<string, double> WeeklySettlementComponents { get; set; } = new Dictionary<string, double>();
        public BillingWeekStatus BillingWeekStatus { get; set; } = BillingWeekStatus.NotExists;  
        public WeeklySettlementsInfo? WeeklySettlementsInfo { get; set; }
    }
}
