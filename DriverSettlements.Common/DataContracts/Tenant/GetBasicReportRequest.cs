namespace DriverSettlements.Common.DataContracts.Tenant
{
    public class GetBasicReportRequest:BaseRequest
    {
        public int YearNumber { get; set; }
        public int WeekNumber { get; set; }
    }
}